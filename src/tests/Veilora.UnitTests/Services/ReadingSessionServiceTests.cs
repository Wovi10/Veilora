using Veilora.Application.DTOs.ReadingNote;
using Veilora.Application.DTOs.ReadingSession;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services;
using Veilora.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Veilora.UnitTests.Services;

[TestFixture]
public class ReadingSessionServiceTests
{
    private Mock<IReadingSessionRepository> _sessionRepoMock;
    private Mock<IReadingNoteRepository> _noteRepoMock;
    private Mock<IWorldRepository> _worldRepoMock;
    private ReadingSessionService _sut;

    [SetUp]
    public void SetUp()
    {
        _sessionRepoMock = new Mock<IReadingSessionRepository>();
        _noteRepoMock = new Mock<IReadingNoteRepository>();
        _worldRepoMock = new Mock<IWorldRepository>();
        _sut = new ReadingSessionService(_sessionRepoMock.Object, _noteRepoMock.Object, _worldRepoMock.Object);
    }

    private static ReadingSession MakeSession(Guid? sessionId = null, Guid? userId = null, Guid? worldId = null, DateTime? endedAt = null)
    {
        var now = DateTime.UtcNow;
        return new ReadingSession
        {
            Id = sessionId ?? Guid.NewGuid(),
            WorldId = worldId ?? Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            StartedAt = now.AddMinutes(-30),
            EndedAt = endedAt,
            CreatedAt = now.AddMinutes(-30),
            UpdatedAt = now.AddMinutes(-30),
        };
    }

    private static ReadingNote MakeNote(Guid? noteId = null, Guid? sessionId = null, Guid? userId = null, string text = "A note", string[]? tags = null)
    {
        var now = DateTime.UtcNow;
        return new ReadingNote
        {
            Id = noteId ?? Guid.NewGuid(),
            SessionId = sessionId ?? Guid.NewGuid(),
            WorldId = Guid.NewGuid(),
            UserId = userId ?? Guid.NewGuid(),
            Text = text,
            Tags = tags ?? [],
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    private static World MakeWorld(Guid? worldId = null, string name = "Middle Earth") => new()
    {
        Id = worldId ?? Guid.NewGuid(),
        Name = name,
        CreatedById = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
    };

    // ── StartAsync ────────────────────────────────────────────────────────────

    [Test]
    public async Task StartAsync_WhenWorldNotFound_ThrowsNotFoundException()
    {
        var dto = new CreateReadingSessionDto(Guid.NewGuid());
        _worldRepoMock.Setup(r => r.GetByIdAsync(dto.WorldId)).ReturnsAsync((World?)null);

        var act = async () => await _sut.StartAsync(Guid.NewGuid(), dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<ReadingSession>()), Times.Never);
    }

    [Test]
    public async Task StartAsync_WhenUserAlreadyHasActiveSession_ThrowsBusinessException()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateReadingSessionDto(Guid.NewGuid());
        _worldRepoMock.Setup(r => r.GetByIdAsync(dto.WorldId)).ReturnsAsync(MakeWorld());
        _sessionRepoMock.Setup(r => r.GetCurrentByUserAsync(userId)).ReturnsAsync(MakeSession(userId: userId));

        var act = async () => await _sut.StartAsync(userId, dto);

        await act.Should().ThrowAsync<BusinessException>();
        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<ReadingSession>()), Times.Never);
    }

    [Test]
    public async Task StartAsync_WhenNoActiveSession_CreatesSessionAndReturnsMappedDto()
    {
        var userId = Guid.NewGuid();
        var worldId = Guid.NewGuid();
        var world = MakeWorld(worldId, "The Shire");
        var dto = new CreateReadingSessionDto(worldId);

        _worldRepoMock.Setup(r => r.GetByIdAsync(worldId)).ReturnsAsync(world);
        _sessionRepoMock.Setup(r => r.GetCurrentByUserAsync(userId)).ReturnsAsync((ReadingSession?)null);
        _sessionRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingSession>())).ReturnsAsync((ReadingSession s) => s);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.StartAsync(userId, dto);

        result.WorldId.Should().Be(worldId);
        result.WorldName.Should().Be("The Shire");
        result.IsActive.Should().BeTrue();
        result.NoteCount.Should().Be(0);
        result.EndedAt.Should().BeNull();
        _sessionRepoMock.Verify(r => r.AddAsync(It.IsAny<ReadingSession>()), Times.Once);
        _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task StartAsync_WhenNoActiveSession_SetsStartedAtToUtcNow()
    {
        var userId = Guid.NewGuid();
        var dto = new CreateReadingSessionDto(Guid.NewGuid());
        ReadingSession? captured = null;

        _worldRepoMock.Setup(r => r.GetByIdAsync(dto.WorldId)).ReturnsAsync(MakeWorld());
        _sessionRepoMock.Setup(r => r.GetCurrentByUserAsync(userId)).ReturnsAsync((ReadingSession?)null);
        _sessionRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingSession>()))
            .Callback<ReadingSession>(s => captured = s)
            .ReturnsAsync((ReadingSession s) => s);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var before = DateTime.UtcNow;
        await _sut.StartAsync(userId, dto);
        var after = DateTime.UtcNow;

        captured.Should().NotBeNull();
        captured!.StartedAt.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
    }

    // ── GetCurrentAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task GetCurrentAsync_WhenNoSession_ReturnsNull()
    {
        var userId = Guid.NewGuid();
        _sessionRepoMock.Setup(r => r.GetCurrentByUserAsync(userId)).ReturnsAsync((ReadingSession?)null);

        var result = await _sut.GetCurrentAsync(userId);

        result.Should().BeNull();
    }

    [Test]
    public async Task GetCurrentAsync_WhenSessionExists_ReturnsMappedDtoWithNoteCount()
    {
        var userId = Guid.NewGuid();
        var worldId = Guid.NewGuid();
        var session = MakeSession(userId: userId, worldId: worldId);
        var world = MakeWorld(worldId, "Narnia");
        var notes = new List<ReadingNote> { MakeNote(), MakeNote(), MakeNote() };

        _sessionRepoMock.Setup(r => r.GetCurrentByUserAsync(userId)).ReturnsAsync(session);
        _worldRepoMock.Setup(r => r.GetByIdAsync(worldId)).ReturnsAsync(world);
        _noteRepoMock.Setup(r => r.GetBySessionAsync(session.Id)).ReturnsAsync(notes);

        var result = await _sut.GetCurrentAsync(userId);

        result.Should().NotBeNull();
        result!.NoteCount.Should().Be(3);
        result.WorldName.Should().Be("Narnia");
        result.IsActive.Should().BeTrue();
    }

    [Test]
    public async Task GetCurrentAsync_WhenWorldNotFoundForSession_ReturnsUnknownWorldName()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);

        _sessionRepoMock.Setup(r => r.GetCurrentByUserAsync(userId)).ReturnsAsync(session);
        _worldRepoMock.Setup(r => r.GetByIdAsync(session.WorldId)).ReturnsAsync((World?)null);
        _noteRepoMock.Setup(r => r.GetBySessionAsync(session.Id)).ReturnsAsync([]);

        var result = await _sut.GetCurrentAsync(userId);

        result.Should().NotBeNull();
        result!.WorldName.Should().Be("Unknown");
    }

    // ── PauseAsync ────────────────────────────────────────────────────────────

    [Test]
    public async Task PauseAsync_WhenSessionNotFound_ThrowsNotFoundException()
    {
        var sessionId = Guid.NewGuid();
        _sessionRepoMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((ReadingSession?)null);

        var act = async () => await _sut.PauseAsync(sessionId, Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
        _sessionRepoMock.Verify(r => r.UpdateAsync(It.IsAny<ReadingSession>()), Times.Never);
    }

    [Test]
    public async Task PauseAsync_WhenUserDoesNotOwnSession_ThrowsBusinessException()
    {
        var session = MakeSession(userId: Guid.NewGuid());
        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var act = async () => await _sut.PauseAsync(session.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
        _sessionRepoMock.Verify(r => r.UpdateAsync(It.IsAny<ReadingSession>()), Times.Never);
    }

    [Test]
    public async Task PauseAsync_WhenUserOwnsSession_SetsEndedAtAndSaves()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingSession? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _sessionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ReadingSession>()))
            .Callback<ReadingSession>(s => captured = s)
            .Returns(Task.CompletedTask);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var before = DateTime.UtcNow;
        await _sut.PauseAsync(session.Id, userId);
        var after = DateTime.UtcNow;

        captured.Should().NotBeNull();
        captured!.EndedAt.Should().NotBeNull();
        captured.EndedAt!.Value.Should().BeOnOrAfter(before).And.BeOnOrBefore(after);
        _sessionRepoMock.Verify(r => r.UpdateAsync(It.IsAny<ReadingSession>()), Times.Once);
        _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task PauseAsync_WhenUserOwnsSession_PreservesSessionMetadata()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingSession? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _sessionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ReadingSession>()))
            .Callback<ReadingSession>(s => captured = s)
            .Returns(Task.CompletedTask);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.PauseAsync(session.Id, userId);

        captured!.Id.Should().Be(session.Id);
        captured.WorldId.Should().Be(session.WorldId);
        captured.UserId.Should().Be(session.UserId);
        captured.StartedAt.Should().Be(session.StartedAt);
    }

    // ── ResumeAsync ───────────────────────────────────────────────────────────

    [Test]
    public async Task ResumeAsync_WhenSessionNotFound_ThrowsNotFoundException()
    {
        var sessionId = Guid.NewGuid();
        _sessionRepoMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((ReadingSession?)null);

        var act = async () => await _sut.ResumeAsync(sessionId, Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task ResumeAsync_WhenUserDoesNotOwnSession_ThrowsBusinessException()
    {
        var session = MakeSession(userId: Guid.NewGuid(), endedAt: DateTime.UtcNow.AddHours(-1));
        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var act = async () => await _sut.ResumeAsync(session.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task ResumeAsync_WhenUserOwnsSession_ClearsEndedAtAndSaves()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId, endedAt: DateTime.UtcNow.AddHours(-1));
        ReadingSession? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _sessionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ReadingSession>()))
            .Callback<ReadingSession>(s => captured = s)
            .Returns(Task.CompletedTask);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.ResumeAsync(session.Id, userId);

        captured.Should().NotBeNull();
        captured!.EndedAt.Should().BeNull();
        _sessionRepoMock.Verify(r => r.UpdateAsync(It.IsAny<ReadingSession>()), Times.Once);
        _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ResumeAsync_WhenUserOwnsSession_PreservesSessionMetadata()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId, endedAt: DateTime.UtcNow.AddHours(-1));
        ReadingSession? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _sessionRepoMock.Setup(r => r.UpdateAsync(It.IsAny<ReadingSession>()))
            .Callback<ReadingSession>(s => captured = s)
            .Returns(Task.CompletedTask);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.ResumeAsync(session.Id, userId);

        captured!.Id.Should().Be(session.Id);
        captured.WorldId.Should().Be(session.WorldId);
        captured.UserId.Should().Be(session.UserId);
        captured.StartedAt.Should().Be(session.StartedAt);
    }

    // ── ClearAsync ────────────────────────────────────────────────────────────

    [Test]
    public async Task ClearAsync_WhenSessionNotFound_ThrowsNotFoundException()
    {
        var sessionId = Guid.NewGuid();
        _sessionRepoMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((ReadingSession?)null);

        var act = async () => await _sut.ClearAsync(sessionId, Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
        _sessionRepoMock.Verify(r => r.DeleteAsync(It.IsAny<ReadingSession>()), Times.Never);
    }

    [Test]
    public async Task ClearAsync_WhenUserDoesNotOwnSession_ThrowsBusinessException()
    {
        var session = MakeSession(userId: Guid.NewGuid());
        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var act = async () => await _sut.ClearAsync(session.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
        _sessionRepoMock.Verify(r => r.DeleteAsync(It.IsAny<ReadingSession>()), Times.Never);
    }

    [Test]
    public async Task ClearAsync_WhenUserOwnsSession_DeletesSessionAndSaves()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _sessionRepoMock.Setup(r => r.DeleteAsync(session)).Returns(Task.CompletedTask);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.ClearAsync(session.Id, userId);

        _sessionRepoMock.Verify(r => r.DeleteAsync(session), Times.Once);
        _sessionRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ClearAsync_WhenUserOwnsSession_ReturnsWorldId()
    {
        var userId = Guid.NewGuid();
        var worldId = Guid.NewGuid();
        var session = MakeSession(userId: userId, worldId: worldId);

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _sessionRepoMock.Setup(r => r.DeleteAsync(session)).Returns(Task.CompletedTask);
        _sessionRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.ClearAsync(session.Id, userId);

        result.Should().Be(worldId);
    }

    // ── GetNotesAsync ─────────────────────────────────────────────────────────

    [Test]
    public async Task GetNotesAsync_WhenSessionNotFound_ThrowsNotFoundException()
    {
        var sessionId = Guid.NewGuid();
        _sessionRepoMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((ReadingSession?)null);

        var act = async () => await _sut.GetNotesAsync(sessionId, Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
        _noteRepoMock.Verify(r => r.GetBySessionAsync(It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task GetNotesAsync_WhenUserDoesNotOwnSession_ThrowsBusinessException()
    {
        var session = MakeSession(userId: Guid.NewGuid());
        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var act = async () => await _sut.GetNotesAsync(session.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task GetNotesAsync_WhenUserOwnsSession_ReturnsMappedNoteDtos()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        var note1 = MakeNote(sessionId: session.Id, text: "First note", tags: ["hero"]);
        var note2 = MakeNote(sessionId: session.Id, text: "Second note", tags: []);

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.GetBySessionAsync(session.Id)).ReturnsAsync([note1, note2]);

        var result = (await _sut.GetNotesAsync(session.Id, userId)).ToList();

        result.Should().HaveCount(2);
        result[0].Text.Should().Be("First note");
        result[0].Tags.Should().BeEquivalentTo(["hero"]);
        result[0].SessionId.Should().Be(session.Id);
        _noteRepoMock.Verify(r => r.GetBySessionAsync(session.Id), Times.Once);
    }

    [Test]
    public async Task GetNotesAsync_WhenUserOwnsSession_ReturnsEmptyListWithoutThrowing()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.GetBySessionAsync(session.Id)).ReturnsAsync([]);

        var result = await _sut.GetNotesAsync(session.Id, userId);

        result.Should().BeEmpty();
    }

    // ── AddNoteAsync ──────────────────────────────────────────────────────────

    [Test]
    public async Task AddNoteAsync_WhenSessionNotFound_ThrowsNotFoundException()
    {
        var sessionId = Guid.NewGuid();
        _sessionRepoMock.Setup(r => r.GetByIdAsync(sessionId)).ReturnsAsync((ReadingSession?)null);

        var act = async () => await _sut.AddNoteAsync(sessionId, Guid.NewGuid(), new CreateReadingNoteDto("text"));

        await act.Should().ThrowAsync<NotFoundException>();
        _noteRepoMock.Verify(r => r.AddAsync(It.IsAny<ReadingNote>()), Times.Never);
    }

    [Test]
    public async Task AddNoteAsync_WhenUserDoesNotOwnSession_ThrowsBusinessException()
    {
        var session = MakeSession(userId: Guid.NewGuid());
        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);

        var act = async () => await _sut.AddNoteAsync(session.Id, Guid.NewGuid(), new CreateReadingNoteDto("text"));

        await act.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task AddNoteAsync_WhenNoteHasNoHashtags_CreatesNoteWithEmptyTags()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingNote? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingNote>()))
            .Callback<ReadingNote>(n => captured = n)
            .ReturnsAsync((ReadingNote n) => n);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddNoteAsync(session.Id, userId, new CreateReadingNoteDto("Just a plain note."));

        captured!.Tags.Should().BeEmpty();
    }

    [Test]
    public async Task AddNoteAsync_WhenNoteHasSingleHashtag_ExtractsSingleTag()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingNote? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingNote>()))
            .Callback<ReadingNote>(n => captured = n)
            .ReturnsAsync((ReadingNote n) => n);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddNoteAsync(session.Id, userId, new CreateReadingNoteDto("Met #Gandalf at the Prancing Pony."));

        captured!.Tags.Should().BeEquivalentTo(["gandalf"]);
    }

    [Test]
    public async Task AddNoteAsync_WhenNoteHasMultipleHashtags_ExtractsAllTags()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingNote? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingNote>()))
            .Callback<ReadingNote>(n => captured = n)
            .ReturnsAsync((ReadingNote n) => n);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddNoteAsync(session.Id, userId, new CreateReadingNoteDto("#Frodo and #Sam left #TheShire together."));

        captured!.Tags.Should().BeEquivalentTo(["frodo", "sam", "theshire"]);
    }

    [Test]
    public async Task AddNoteAsync_WhenNoteHasDuplicateHashtags_DeduplicatesTags()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingNote? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingNote>()))
            .Callback<ReadingNote>(n => captured = n)
            .ReturnsAsync((ReadingNote n) => n);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddNoteAsync(session.Id, userId, new CreateReadingNoteDto("This is a #spoiler moment. Big #spoiler."));

        captured!.Tags.Should().HaveCount(1);
        captured.Tags[0].Should().Be("spoiler");
    }

    [Test]
    public async Task AddNoteAsync_WhenNoteHasMixedCaseHashtags_NormalizesToLowercaseBeforeDedup()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingNote? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingNote>()))
            .Callback<ReadingNote>(n => captured = n)
            .ReturnsAsync((ReadingNote n) => n);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddNoteAsync(session.Id, userId, new CreateReadingNoteDto("Meet #GANDALF later with #Gandalf."));

        captured!.Tags.Should().HaveCount(1);
        captured.Tags[0].Should().Be("gandalf");
    }

    [Test]
    public async Task AddNoteAsync_WhenNoteIsValid_SetsSessionIdWorldIdAndUserId()
    {
        var userId = Guid.NewGuid();
        var session = MakeSession(userId: userId);
        ReadingNote? captured = null;

        _sessionRepoMock.Setup(r => r.GetByIdAsync(session.Id)).ReturnsAsync(session);
        _noteRepoMock.Setup(r => r.AddAsync(It.IsAny<ReadingNote>()))
            .Callback<ReadingNote>(n => captured = n)
            .ReturnsAsync((ReadingNote n) => n);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddNoteAsync(session.Id, userId, new CreateReadingNoteDto("A note."));

        captured!.SessionId.Should().Be(session.Id);
        captured.WorldId.Should().Be(session.WorldId);
        captured.UserId.Should().Be(userId);
    }

    // ── DeleteNoteAsync ───────────────────────────────────────────────────────

    [Test]
    public async Task DeleteNoteAsync_WhenNoteNotFound_ThrowsNotFoundException()
    {
        var noteId = Guid.NewGuid();
        _noteRepoMock.Setup(r => r.GetByIdAsync(noteId)).ReturnsAsync((ReadingNote?)null);

        var act = async () => await _sut.DeleteNoteAsync(noteId, Guid.NewGuid());

        await act.Should().ThrowAsync<NotFoundException>();
        _noteRepoMock.Verify(r => r.DeleteAsync(It.IsAny<ReadingNote>()), Times.Never);
    }

    [Test]
    public async Task DeleteNoteAsync_WhenUserDoesNotOwnNote_ThrowsBusinessException()
    {
        var note = MakeNote(userId: Guid.NewGuid());
        _noteRepoMock.Setup(r => r.GetByIdAsync(note.Id)).ReturnsAsync(note);

        var act = async () => await _sut.DeleteNoteAsync(note.Id, Guid.NewGuid());

        await act.Should().ThrowAsync<BusinessException>();
        _noteRepoMock.Verify(r => r.DeleteAsync(It.IsAny<ReadingNote>()), Times.Never);
    }

    [Test]
    public async Task DeleteNoteAsync_WhenUserOwnsNote_DeletesNoteAndSaves()
    {
        var userId = Guid.NewGuid();
        var note = MakeNote(userId: userId);

        _noteRepoMock.Setup(r => r.GetByIdAsync(note.Id)).ReturnsAsync(note);
        _noteRepoMock.Setup(r => r.DeleteAsync(note)).Returns(Task.CompletedTask);
        _noteRepoMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteNoteAsync(note.Id, userId);

        _noteRepoMock.Verify(r => r.DeleteAsync(note), Times.Once);
        _noteRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
