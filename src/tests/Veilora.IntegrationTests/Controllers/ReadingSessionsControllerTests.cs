using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;
using Veilora.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace Veilora.IntegrationTests.Controllers;

[TestFixture]
public class ReadingSessionsControllerTests
{
    private CustomWebApplicationFactory _factory;
    private HttpClient _client;

    [SetUp]
    public void SetUp()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    private static ReadingSessionDto MakeSessionDto(Guid? sessionId = null, Guid? worldId = null, bool isActive = true) => new()
    {
        Id = sessionId ?? Guid.NewGuid(),
        WorldId = worldId ?? Guid.NewGuid(),
        WorldName = "Middle Earth",
        StartedAt = DateTime.UtcNow.AddMinutes(-10),
        EndedAt = isActive ? null : DateTime.UtcNow,
        NoteCount = 0,
        IsActive = isActive,
    };

    private static ReadingNoteDto MakeNoteDto(Guid? sessionId = null) => new()
    {
        Id = Guid.NewGuid(),
        SessionId = sessionId ?? Guid.NewGuid(),
        Text = "A reading note",
        Tags = ["hero"],
        CreatedAt = DateTime.UtcNow,
    };

    // ── POST /api/reading-sessions ────────────────────────────────────────────

    [Test]
    public async Task Start_WithValidDto_Returns200WithReadingSessionDto()
    {
        var dto = MakeSessionDto();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.StartAsync(TestAuthHandler.TestUserId, It.IsAny<CreateReadingSessionDto>()))
            .ReturnsAsync(dto);

        var response = await _client.PostAsJsonAsync("/api/reading-sessions", new CreateReadingSessionDto(dto.WorldId));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReadingSessionDto>();
        body!.IsActive.Should().BeTrue();
        body.WorldId.Should().Be(dto.WorldId);
    }

    [Test]
    public async Task Start_WhenWorldNotFound_Returns404()
    {
        _factory.ReadingSessionServiceMock
            .Setup(s => s.StartAsync(TestAuthHandler.TestUserId, It.IsAny<CreateReadingSessionDto>()))
            .ThrowsAsync(new NotFoundException("World", Guid.NewGuid()));

        var response = await _client.PostAsJsonAsync("/api/reading-sessions", new CreateReadingSessionDto(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Start_WhenUserAlreadyHasActiveSession_Returns422()
    {
        _factory.ReadingSessionServiceMock
            .Setup(s => s.StartAsync(TestAuthHandler.TestUserId, It.IsAny<CreateReadingSessionDto>()))
            .ThrowsAsync(new BusinessException("End your current session before starting a new one."));

        var response = await _client.PostAsJsonAsync("/api/reading-sessions", new CreateReadingSessionDto(Guid.NewGuid()));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── GET /api/reading-sessions/current ────────────────────────────────────

    [Test]
    public async Task GetCurrent_WhenSessionExists_Returns200WithDto()
    {
        var dto = MakeSessionDto();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.GetCurrentAsync(TestAuthHandler.TestUserId))
            .ReturnsAsync(dto);

        var response = await _client.GetAsync("/api/reading-sessions/current");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReadingSessionDto>();
        body.Should().NotBeNull();
        body!.Id.Should().Be(dto.Id);
    }

    [Test]
    public async Task GetCurrent_WhenNoSession_Returns204()
    {
        _factory.ReadingSessionServiceMock
            .Setup(s => s.GetCurrentAsync(TestAuthHandler.TestUserId))
            .ReturnsAsync((ReadingSessionDto?)null);

        var response = await _client.GetAsync("/api/reading-sessions/current");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // ── POST /api/reading-sessions/{id}/pause ────────────────────────────────

    [Test]
    public async Task Pause_WhenSessionExists_Returns204()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.PauseAsync(sessionId, TestAuthHandler.TestUserId))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsync($"/api/reading-sessions/{sessionId}/pause", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Pause_WhenSessionNotFound_Returns404()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.PauseAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new NotFoundException("ReadingSession", sessionId));

        var response = await _client.PostAsync($"/api/reading-sessions/{sessionId}/pause", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Pause_WhenUserDoesNotOwnSession_Returns422()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.PauseAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new BusinessException("Access denied."));

        var response = await _client.PostAsync($"/api/reading-sessions/{sessionId}/pause", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── POST /api/reading-sessions/{id}/resume ───────────────────────────────

    [Test]
    public async Task Resume_WhenSessionExists_Returns204()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.ResumeAsync(sessionId, TestAuthHandler.TestUserId))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsync($"/api/reading-sessions/{sessionId}/resume", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Resume_WhenSessionNotFound_Returns404()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.ResumeAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new NotFoundException("ReadingSession", sessionId));

        var response = await _client.PostAsync($"/api/reading-sessions/{sessionId}/resume", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Resume_WhenUserDoesNotOwnSession_Returns422()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.ResumeAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new BusinessException("Access denied."));

        var response = await _client.PostAsync($"/api/reading-sessions/{sessionId}/resume", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── DELETE /api/reading-sessions/{id} ────────────────────────────────────

    [Test]
    public async Task Clear_WhenSessionExists_Returns200WithWorldId()
    {
        var sessionId = Guid.NewGuid();
        var worldId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.ClearAsync(sessionId, TestAuthHandler.TestUserId))
            .ReturnsAsync(worldId);

        var response = await _client.DeleteAsync($"/api/reading-sessions/{sessionId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("worldId").GetGuid().Should().Be(worldId);
    }

    [Test]
    public async Task Clear_WhenSessionNotFound_Returns404()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.ClearAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new NotFoundException("ReadingSession", sessionId));

        var response = await _client.DeleteAsync($"/api/reading-sessions/{sessionId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Clear_WhenUserDoesNotOwnSession_Returns422()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.ClearAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new BusinessException("Access denied."));

        var response = await _client.DeleteAsync($"/api/reading-sessions/{sessionId}");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── GET /api/reading-sessions/{id}/notes ─────────────────────────────────

    [Test]
    public async Task GetNotes_WhenSessionExists_Returns200WithNoteList()
    {
        var sessionId = Guid.NewGuid();
        var notes = new List<ReadingNoteDto> { MakeNoteDto(sessionId), MakeNoteDto(sessionId) };
        _factory.ReadingSessionServiceMock
            .Setup(s => s.GetNotesAsync(sessionId, TestAuthHandler.TestUserId))
            .ReturnsAsync(notes);

        var response = await _client.GetAsync($"/api/reading-sessions/{sessionId}/notes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<ReadingNoteDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task GetNotes_WhenNoNotes_Returns200WithEmptyList()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.GetNotesAsync(sessionId, TestAuthHandler.TestUserId))
            .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/reading-sessions/{sessionId}/notes");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<ReadingNoteDto>>();
        body!.Should().BeEmpty();
    }

    [Test]
    public async Task GetNotes_WhenSessionNotFound_Returns404()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.GetNotesAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new NotFoundException("ReadingSession", sessionId));

        var response = await _client.GetAsync($"/api/reading-sessions/{sessionId}/notes");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetNotes_WhenUserDoesNotOwnSession_Returns422()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.GetNotesAsync(sessionId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new BusinessException("Access denied."));

        var response = await _client.GetAsync($"/api/reading-sessions/{sessionId}/notes");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── POST /api/reading-sessions/{id}/notes ────────────────────────────────

    [Test]
    public async Task AddNote_WithValidDto_Returns200WithReadingNoteDto()
    {
        var sessionId = Guid.NewGuid();
        var noteDto = new ReadingNoteDto
        {
            Id = Guid.NewGuid(),
            SessionId = sessionId,
            Text = "Bilbo leaves the Shire #journey #hobbit",
            Tags = ["journey", "hobbit"],
            CreatedAt = DateTime.UtcNow,
        };
        _factory.ReadingSessionServiceMock
            .Setup(s => s.AddNoteAsync(sessionId, TestAuthHandler.TestUserId, It.IsAny<CreateReadingNoteDto>()))
            .ReturnsAsync(noteDto);

        var response = await _client.PostAsJsonAsync($"/api/reading-sessions/{sessionId}/notes", new CreateReadingNoteDto(noteDto.Text));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<ReadingNoteDto>();
        body!.Tags.Should().BeEquivalentTo(["journey", "hobbit"]);
    }

    [Test]
    public async Task AddNote_PassesTextFromBodyToService()
    {
        var sessionId = Guid.NewGuid();
        CreateReadingNoteDto? captured = null;
        var noteDto = MakeNoteDto(sessionId);

        _factory.ReadingSessionServiceMock
            .Setup(s => s.AddNoteAsync(sessionId, TestAuthHandler.TestUserId, It.IsAny<CreateReadingNoteDto>()))
            .Callback<Guid, Guid, CreateReadingNoteDto>((_, _, dto) => captured = dto)
            .ReturnsAsync(noteDto);

        await _client.PostAsJsonAsync($"/api/reading-sessions/{sessionId}/notes", new CreateReadingNoteDto("Exact text #tag"));

        captured.Should().NotBeNull();
        captured!.Text.Should().Be("Exact text #tag");
    }

    [Test]
    public async Task AddNote_WhenSessionNotFound_Returns404()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.AddNoteAsync(sessionId, TestAuthHandler.TestUserId, It.IsAny<CreateReadingNoteDto>()))
            .ThrowsAsync(new NotFoundException("ReadingSession", sessionId));

        var response = await _client.PostAsJsonAsync($"/api/reading-sessions/{sessionId}/notes", new CreateReadingNoteDto("text"));

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddNote_WhenUserDoesNotOwnSession_Returns422()
    {
        var sessionId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.AddNoteAsync(sessionId, TestAuthHandler.TestUserId, It.IsAny<CreateReadingNoteDto>()))
            .ThrowsAsync(new BusinessException("Access denied."));

        var response = await _client.PostAsJsonAsync($"/api/reading-sessions/{sessionId}/notes", new CreateReadingNoteDto("text"));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    // ── DELETE /api/reading-sessions/notes/{noteId} ──────────────────────────

    [Test]
    public async Task DeleteNote_WhenNoteExists_Returns204()
    {
        var noteId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.DeleteNoteAsync(noteId, TestAuthHandler.TestUserId))
            .Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/reading-sessions/notes/{noteId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task DeleteNote_WhenNoteNotFound_Returns404()
    {
        var noteId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.DeleteNoteAsync(noteId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new NotFoundException("ReadingNote", noteId));

        var response = await _client.DeleteAsync($"/api/reading-sessions/notes/{noteId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteNote_WhenUserDoesNotOwnNote_Returns422()
    {
        var noteId = Guid.NewGuid();
        _factory.ReadingSessionServiceMock
            .Setup(s => s.DeleteNoteAsync(noteId, TestAuthHandler.TestUserId))
            .ThrowsAsync(new BusinessException("Access denied."));

        var response = await _client.DeleteAsync($"/api/reading-sessions/notes/{noteId}");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
