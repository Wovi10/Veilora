using System.Net;
using System.Net.Http.Json;
using Veilora.Application.DTOs.Photo;
using Veilora.Application.Exceptions;
using Veilora.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Veilora.IntegrationTests.Controllers;

[TestFixture]
public class PhotosControllerTests
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

    private static PhotoDto MakePhotoDto(Guid characterId, int displayOrder = 0) => new(
        Guid.NewGuid(),
        characterId,
        "Character",
        "https://example.com/photo.jpg",
        displayOrder,
        DateTime.UtcNow,
        DateTime.UtcNow);

    // --- GET ---

    [Test]
    public async Task GetByEntity_WhenNoPhotosExist_Returns200WithEmptyArray()
    {
        var characterId = Guid.NewGuid();
        _factory.PhotoServiceMock
            .Setup(s => s.GetByEntityAsync(PhotoTargetType.Character, characterId))
            .ReturnsAsync([]);

        var response = await _client.GetAsync($"/api/photos/character/{characterId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PhotoDto>>();
        body!.Should().BeEmpty();
    }

    [Test]
    public async Task GetByEntity_WhenPhotosExist_Returns200WithDtos()
    {
        var characterId = Guid.NewGuid();
        var dtos = new List<PhotoDto>
        {
            MakePhotoDto(characterId, 0),
            MakePhotoDto(characterId, 1)
        };
        _factory.PhotoServiceMock
            .Setup(s => s.GetByEntityAsync(PhotoTargetType.Character, characterId))
            .ReturnsAsync(dtos);

        var response = await _client.GetAsync($"/api/photos/character/{characterId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PhotoDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task GetByEntity_WhenEntityTypeIsInvalid_Returns400()
    {
        var entityId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/photos/invalidtype/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // --- POST ---

    [Test]
    public async Task Add_WithValidUrl_Returns201WithPhotoDto()
    {
        var characterId = Guid.NewGuid();
        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        var returnedDto = MakePhotoDto(characterId, 0);
        _factory.PhotoServiceMock
            .Setup(s => s.AddAsync(PhotoTargetType.Character, characterId, It.IsAny<Guid>(), It.IsAny<CreatePhotoDto>()))
            .ReturnsAsync(returnedDto);

        var response = await _client.PostAsJsonAsync($"/api/photos/character/{characterId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<PhotoDto>();
        body!.Url.Should().Be("https://example.com/photo.jpg");
    }

    [Test]
    public async Task Add_WhenUrlIsInvalid_Returns400()
    {
        var characterId = Guid.NewGuid();
        var dto = new CreatePhotoDto("not-a-url");
        _factory.PhotoServiceMock
            .Setup(s => s.AddAsync(PhotoTargetType.Character, characterId, It.IsAny<Guid>(), It.IsAny<CreatePhotoDto>()))
            .ThrowsAsync(new ValidationException("Url", "URL must be a valid absolute URI."));

        var response = await _client.PostAsJsonAsync($"/api/photos/character/{characterId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task Add_WhenFivePhotosAlreadyExist_Returns422()
    {
        var characterId = Guid.NewGuid();
        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        _factory.PhotoServiceMock
            .Setup(s => s.AddAsync(PhotoTargetType.Character, characterId, It.IsAny<Guid>(), It.IsAny<CreatePhotoDto>()))
            .ThrowsAsync(new BusinessException("Maximum of 5 photos per entity."));

        var response = await _client.PostAsJsonAsync($"/api/photos/character/{characterId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task Add_WhenUserHasNoPermission_Returns422()
    {
        var characterId = Guid.NewGuid();
        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        _factory.PhotoServiceMock
            .Setup(s => s.AddAsync(PhotoTargetType.Character, characterId, It.IsAny<Guid>(), It.IsAny<CreatePhotoDto>()))
            .ThrowsAsync(new BusinessException("You do not have permission to modify photos in this world."));

        var response = await _client.PostAsJsonAsync($"/api/photos/character/{characterId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task Add_WhenEntityTypeIsInvalid_Returns400()
    {
        var entityId = Guid.NewGuid();
        var dto = new CreatePhotoDto("https://example.com/photo.jpg");

        var response = await _client.PostAsJsonAsync($"/api/photos/invalidtype/{entityId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // --- PUT (update) ---

    [Test]
    public async Task Update_WhenPhotoExists_Returns200WithUpdatedDto()
    {
        var characterId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var dto = new UpdatePhotoDto("https://example.com/updated.jpg");
        var returnedDto = new PhotoDto(photoId, characterId, "Character", "https://example.com/updated.jpg", 0, DateTime.UtcNow, DateTime.UtcNow);
        _factory.PhotoServiceMock
            .Setup(s => s.UpdateAsync(photoId, It.IsAny<Guid>(), It.IsAny<UpdatePhotoDto>()))
            .ReturnsAsync(returnedDto);

        var response = await _client.PutAsJsonAsync($"/api/photos/character/{characterId}/{photoId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PhotoDto>();
        body!.Url.Should().Be("https://example.com/updated.jpg");
    }

    [Test]
    public async Task Update_WhenPhotoNotFound_Returns404()
    {
        var characterId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var dto = new UpdatePhotoDto("https://example.com/updated.jpg");
        _factory.PhotoServiceMock
            .Setup(s => s.UpdateAsync(photoId, It.IsAny<Guid>(), It.IsAny<UpdatePhotoDto>()))
            .ThrowsAsync(new NotFoundException("Photo", photoId));

        var response = await _client.PutAsJsonAsync($"/api/photos/character/{characterId}/{photoId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Update_WhenEntityTypeIsInvalid_Returns400()
    {
        var entityId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        var dto = new UpdatePhotoDto("https://example.com/photo.jpg");

        var response = await _client.PutAsJsonAsync($"/api/photos/invalidtype/{entityId}/{photoId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // --- DELETE ---

    [Test]
    public async Task Delete_WhenPhotoExists_Returns204()
    {
        var characterId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        _factory.PhotoServiceMock
            .Setup(s => s.DeleteAsync(photoId, It.IsAny<Guid>()))
            .Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/photos/character/{characterId}/{photoId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WhenPhotoNotFound_Returns404()
    {
        var characterId = Guid.NewGuid();
        var photoId = Guid.NewGuid();
        _factory.PhotoServiceMock
            .Setup(s => s.DeleteAsync(photoId, It.IsAny<Guid>()))
            .ThrowsAsync(new NotFoundException("Photo", photoId));

        var response = await _client.DeleteAsync($"/api/photos/character/{characterId}/{photoId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Delete_WhenEntityTypeIsInvalid_Returns400()
    {
        var entityId = Guid.NewGuid();
        var photoId = Guid.NewGuid();

        var response = await _client.DeleteAsync($"/api/photos/invalidtype/{entityId}/{photoId}");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // --- PUT reorder ---

    [Test]
    public async Task Reorder_WhenValidOrder_Returns200WithReorderedPhotos()
    {
        var characterId = Guid.NewGuid();
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var reorderedDtos = new List<PhotoDto>
        {
            new(id2, characterId, "Character", "https://example.com/b.jpg", 0, DateTime.UtcNow, DateTime.UtcNow),
            new(id1, characterId, "Character", "https://example.com/a.jpg", 1, DateTime.UtcNow, DateTime.UtcNow)
        };
        _factory.PhotoServiceMock
            .Setup(s => s.ReorderAsync(PhotoTargetType.Character, characterId, It.IsAny<Guid>(), It.IsAny<ReorderPhotosDto>()))
            .Returns(Task.CompletedTask);
        _factory.PhotoServiceMock
            .Setup(s => s.GetByEntityAsync(PhotoTargetType.Character, characterId))
            .ReturnsAsync(reorderedDtos);

        var dto = new ReorderPhotosDto([id2, id1]);
        var response = await _client.PutAsJsonAsync($"/api/photos/character/{characterId}/reorder", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PhotoDto>>();
        body!.Should().HaveCount(2);
        body[0].Id.Should().Be(id2);
        body[1].Id.Should().Be(id1);
    }

    [Test]
    public async Task Reorder_WhenEntityTypeIsInvalid_Returns400()
    {
        var entityId = Guid.NewGuid();
        var dto = new ReorderPhotosDto([Guid.NewGuid()]);

        var response = await _client.PutAsJsonAsync($"/api/photos/invalidtype/{entityId}/reorder", dto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
