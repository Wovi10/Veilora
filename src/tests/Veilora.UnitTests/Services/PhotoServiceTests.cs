using Veilora.Application.DTOs.Photo;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Veilora.UnitTests.Services;

[TestFixture]
public class PhotoServiceTests
{
    private Mock<IPhotoRepository> _photoRepositoryMock;
    private Mock<ICharacterRepository> _characterRepositoryMock;
    private Mock<ILocationRepository> _locationRepositoryMock;
    private Mock<IEntityRepository> _entityRepositoryMock;
    private Mock<IWorldRepository> _worldRepositoryMock;
    private Mock<IWorldPermissionRepository> _worldPermissionRepositoryMock;
    private PhotoService _sut;

    private static readonly Guid WorldId = Guid.NewGuid();
    private static readonly Guid OwnerId = Guid.NewGuid();
    private static readonly Guid EditorId = Guid.NewGuid();
    private static readonly Guid ViewerId = Guid.NewGuid();

    [SetUp]
    public void SetUp()
    {
        _photoRepositoryMock = new Mock<IPhotoRepository>();
        _characterRepositoryMock = new Mock<ICharacterRepository>();
        _locationRepositoryMock = new Mock<ILocationRepository>();
        _entityRepositoryMock = new Mock<IEntityRepository>();
        _worldRepositoryMock = new Mock<IWorldRepository>();
        _worldPermissionRepositoryMock = new Mock<IWorldPermissionRepository>();

        _sut = new PhotoService(
            _photoRepositoryMock.Object,
            _characterRepositoryMock.Object,
            _locationRepositoryMock.Object,
            _entityRepositoryMock.Object,
            _worldRepositoryMock.Object,
            _worldPermissionRepositoryMock.Object);
    }

    // --- Helpers ---

    private static Photo MakePhoto(Guid id, Guid entityId, int displayOrder, DateTime now) => new()
    {
        Id = id,
        EntityId = entityId,
        EntityType = PhotoTargetType.Character,
        Url = "https://example.com/photo.jpg",
        DisplayOrder = displayOrder,
        CreatedAt = now,
        UpdatedAt = now
    };

    private static Character MakeCharacter(Guid id, Guid worldId, DateTime now) => new()
    {
        Id = id,
        Name = "Alice",
        WorldId = worldId,
        CreatedAt = now,
        UpdatedAt = now
    };

    private static World MakeWorld(Guid id, Guid ownerId, DateTime now) => new()
    {
        Id = id,
        Name = "Middle Earth",
        CreatedById = ownerId,
        CreatedAt = now,
        UpdatedAt = now
    };

    private void SetupOwnerPermissions(Guid characterId)
    {
        var now = DateTime.UtcNow;
        var character = MakeCharacter(characterId, WorldId, now);
        var world = MakeWorld(WorldId, OwnerId, now);
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(characterId)).ReturnsAsync(character);
        _worldRepositoryMock.Setup(r => r.GetByIdAsync(WorldId)).ReturnsAsync(world);
    }

    private void SetupEditorPermissions(Guid characterId)
    {
        var now = DateTime.UtcNow;
        var character = MakeCharacter(characterId, WorldId, now);
        var world = MakeWorld(WorldId, OwnerId, now);
        var permission = new WorldPermission { WorldId = WorldId, UserId = EditorId, CanEdit = true, CreatedAt = now, UpdatedAt = now };
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(characterId)).ReturnsAsync(character);
        _worldRepositoryMock.Setup(r => r.GetByIdAsync(WorldId)).ReturnsAsync(world);
        _worldPermissionRepositoryMock.Setup(r => r.GetAsync(WorldId, EditorId)).ReturnsAsync(permission);
    }

    private void SetupViewerPermissions(Guid characterId)
    {
        var now = DateTime.UtcNow;
        var character = MakeCharacter(characterId, WorldId, now);
        var world = MakeWorld(WorldId, OwnerId, now);
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(characterId)).ReturnsAsync(character);
        _worldRepositoryMock.Setup(r => r.GetByIdAsync(WorldId)).ReturnsAsync(world);
        _worldPermissionRepositoryMock.Setup(r => r.GetAsync(WorldId, ViewerId)).ReturnsAsync((WorldPermission?)null);
    }

    // --- GetByEntityAsync ---

    [Test]
    public async Task GetByEntityAsync_WhenPhotosExist_ReturnsMappedDtosOrderedByDisplayOrder()
    {
        var entityId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        // Repo returns display order 1 first, then 0 — service does not re-sort; ordering is the repository's responsibility
        var photos = new List<Photo>
        {
            MakePhoto(id2, entityId, 1, now),
            MakePhoto(id1, entityId, 0, now)
        };
        var character = MakeCharacter(entityId, WorldId, now);
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync(character);
        _photoRepositoryMock.Setup(r => r.GetByEntityAsync(PhotoTargetType.Character, entityId)).ReturnsAsync(photos);

        var result = (await _sut.GetByEntityAsync(PhotoTargetType.Character, entityId)).ToList();

        result.Should().HaveCount(2);
        // Assert the returned order matches the repository's return order exactly — service does not re-sort
        result[0].Id.Should().Be(id2);
        result[1].Id.Should().Be(id1);
        result.Should().AllSatisfy(p => p.EntityType.Should().Be("Character"));
    }

    [Test]
    public async Task GetByEntityAsync_WhenNoPhotosExist_ReturnsEmptyCollection()
    {
        var entityId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var character = MakeCharacter(entityId, WorldId, now);
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync(character);
        _photoRepositoryMock.Setup(r => r.GetByEntityAsync(PhotoTargetType.Character, entityId)).ReturnsAsync([]);

        var result = await _sut.GetByEntityAsync(PhotoTargetType.Character, entityId);

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetByEntityAsync_WhenEntityDoesNotExist_ThrowsNotFoundException()
    {
        var entityId = Guid.NewGuid();
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync((Character?)null);

        var act = async () => await _sut.GetByEntityAsync(PhotoTargetType.Character, entityId);

        await act.Should().ThrowAsync<NotFoundException>();
        _photoRepositoryMock.Verify(r => r.GetByEntityAsync(It.IsAny<PhotoTargetType>(), It.IsAny<Guid>()), Times.Never);
    }

    // --- AddAsync happy path ---

    [Test]
    public async Task AddAsync_WhenOwnerAddsPhoto_CreatesPhotoWithCorrectDisplayOrder()
    {
        var characterId = Guid.NewGuid();
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(2);
        _photoRepositoryMock.Setup(r => r.GetMaxDisplayOrderAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(1);
        _photoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Photo>())).ReturnsAsync((Photo p) => p);
        _photoRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        var result = await _sut.AddAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        result.Url.Should().Be("https://example.com/photo.jpg");
        result.DisplayOrder.Should().Be(2);
        result.EntityType.Should().Be("Character");
        _photoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Photo>()), Times.Once);
        _photoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddAsync_WhenEditorAddsPhoto_Succeeds()
    {
        var characterId = Guid.NewGuid();
        SetupEditorPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(0);
        // Repo returns -1 when no photos exist (MaxAsync over empty set returns null, which coalesces to -1)
        _photoRepositoryMock.Setup(r => r.GetMaxDisplayOrderAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(-1);
        _photoRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Photo>())).ReturnsAsync((Photo p) => p);
        _photoRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        var result = await _sut.AddAsync(PhotoTargetType.Character, characterId, EditorId, dto);

        result.Should().NotBeNull();
        result.DisplayOrder.Should().Be(0);
        _photoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Photo>()), Times.Once);
    }

    // --- AddAsync validation errors ---

    [Test]
    public async Task AddAsync_WhenFivePhotosAlreadyExist_ThrowsBusinessException()
    {
        var characterId = Guid.NewGuid();
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(5);

        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        var act = async () => await _sut.AddAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*5*");
        _photoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Photo>()), Times.Never);
    }

    [Test]
    public async Task AddAsync_WhenUrlIsBlank_ThrowsValidationException()
    {
        var characterId = Guid.NewGuid();
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(0);

        var dto = new CreatePhotoDto("   ");
        var act = async () => await _sut.AddAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task AddAsync_WhenUrlExceedsMaxLength_ThrowsValidationException()
    {
        var characterId = Guid.NewGuid();
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(0);

        var tooLongUrl = "https://example.com/" + new string('a', 1990);
        var dto = new CreatePhotoDto(tooLongUrl);
        var act = async () => await _sut.AddAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task AddAsync_WhenUrlIsNotAbsoluteUri_ThrowsValidationException()
    {
        var characterId = Guid.NewGuid();
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(0);

        var dto = new CreatePhotoDto("not-a-valid-url");
        var act = async () => await _sut.AddAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task AddAsync_WhenCharacterNotFound_ThrowsNotFoundException()
    {
        var characterId = Guid.NewGuid();
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(characterId)).ReturnsAsync((Character?)null);

        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        var act = async () => await _sut.AddAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _photoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Photo>()), Times.Never);
    }

    [Test]
    public async Task AddAsync_WhenUserHasNoPermission_ThrowsBusinessException()
    {
        var characterId = Guid.NewGuid();
        SetupViewerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.CountByEntityAsync(PhotoTargetType.Character, characterId)).ReturnsAsync(0);

        var dto = new CreatePhotoDto("https://example.com/photo.jpg");
        var act = async () => await _sut.AddAsync(PhotoTargetType.Character, characterId, ViewerId, dto);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*permission*");
        _photoRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Photo>()), Times.Never);
    }

    // --- UpdateAsync ---

    [Test]
    public async Task UpdateAsync_WhenPhotoExistsAndUserIsOwner_UpdatesUrlAndReturnsDto()
    {
        var photoId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var photo = new Photo
        {
            Id = photoId,
            EntityId = characterId,
            EntityType = PhotoTargetType.Character,
            Url = "https://example.com/old.jpg",
            DisplayOrder = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
        _photoRepositoryMock.Setup(r => r.GetByIdAsync(photoId)).ReturnsAsync(photo);
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.UpdateAsync(photo)).Returns(Task.CompletedTask);
        _photoRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new UpdatePhotoDto("https://example.com/new.jpg");
        var result = await _sut.UpdateAsync(photoId, OwnerId, dto);

        result.Url.Should().Be("https://example.com/new.jpg");
        _photoRepositoryMock.Verify(r => r.UpdateAsync(photo), Times.Once);
        _photoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenPhotoNotFound_ThrowsNotFoundException()
    {
        var photoId = Guid.NewGuid();
        _photoRepositoryMock.Setup(r => r.GetByIdAsync(photoId)).ReturnsAsync((Photo?)null);

        var dto = new UpdatePhotoDto("https://example.com/photo.jpg");
        var act = async () => await _sut.UpdateAsync(photoId, OwnerId, dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task UpdateAsync_WhenUserHasNoPermission_ThrowsBusinessException()
    {
        var photoId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var photo = new Photo
        {
            Id = photoId,
            EntityId = characterId,
            EntityType = PhotoTargetType.Character,
            Url = "https://example.com/old.jpg",
            DisplayOrder = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
        _photoRepositoryMock.Setup(r => r.GetByIdAsync(photoId)).ReturnsAsync(photo);
        SetupViewerPermissions(characterId);

        var dto = new UpdatePhotoDto("https://example.com/new.jpg");
        var act = async () => await _sut.UpdateAsync(photoId, ViewerId, dto);

        await act.Should().ThrowAsync<BusinessException>();
    }

    // --- DeleteAsync ---

    [Test]
    public async Task DeleteAsync_WhenPhotoExistsAndUserIsOwner_DeletesPhoto()
    {
        var photoId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var photo = new Photo
        {
            Id = photoId,
            EntityId = characterId,
            EntityType = PhotoTargetType.Character,
            Url = "https://example.com/photo.jpg",
            DisplayOrder = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
        _photoRepositoryMock.Setup(r => r.GetByIdAsync(photoId)).ReturnsAsync(photo);
        SetupOwnerPermissions(characterId);
        _photoRepositoryMock.Setup(r => r.DeleteAsync(photo)).Returns(Task.CompletedTask);
        _photoRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(photoId, OwnerId);

        _photoRepositoryMock.Verify(r => r.DeleteAsync(photo), Times.Once);
        _photoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenPhotoNotFound_ThrowsNotFoundException()
    {
        var photoId = Guid.NewGuid();
        _photoRepositoryMock.Setup(r => r.GetByIdAsync(photoId)).ReturnsAsync((Photo?)null);

        var act = async () => await _sut.DeleteAsync(photoId, OwnerId);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DeleteAsync_WhenUserHasNoPermission_ThrowsBusinessException()
    {
        var photoId = Guid.NewGuid();
        var characterId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var photo = new Photo
        {
            Id = photoId,
            EntityId = characterId,
            EntityType = PhotoTargetType.Character,
            Url = "https://example.com/photo.jpg",
            DisplayOrder = 0,
            CreatedAt = now,
            UpdatedAt = now
        };
        _photoRepositoryMock.Setup(r => r.GetByIdAsync(photoId)).ReturnsAsync(photo);
        SetupViewerPermissions(characterId);

        var act = async () => await _sut.DeleteAsync(photoId, ViewerId);

        await act.Should().ThrowAsync<BusinessException>();
        _photoRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<Photo>()), Times.Never);
    }

    // --- ReorderAsync ---

    [Test]
    public async Task ReorderAsync_WhenOrderedIdsMatchExistingPhotos_SetsDisplayOrderByIndex()
    {
        var characterId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var id3 = Guid.NewGuid();
        var photo1 = MakePhoto(id1, characterId, 0, now);
        var photo2 = MakePhoto(id2, characterId, 1, now);
        var photo3 = MakePhoto(id3, characterId, 2, now);

        SetupOwnerPermissions(characterId);
        _photoRepositoryMock
            .Setup(r => r.GetByEntityAsync(PhotoTargetType.Character, characterId))
            .ReturnsAsync([photo1, photo2, photo3]);
        _photoRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Photo>())).Returns(Task.CompletedTask);
        _photoRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new ReorderPhotosDto([id3, id1, id2]);
        await _sut.ReorderAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        photo3.DisplayOrder.Should().Be(0);
        photo1.DisplayOrder.Should().Be(1);
        photo2.DisplayOrder.Should().Be(2);
        _photoRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Photo>()), Times.Exactly(3));
        _photoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task ReorderAsync_WhenOrderedIdsDoNotMatchExistingPhotos_ThrowsBusinessException()
    {
        var characterId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();

        SetupOwnerPermissions(characterId);
        _photoRepositoryMock
            .Setup(r => r.GetByEntityAsync(PhotoTargetType.Character, characterId))
            .ReturnsAsync([MakePhoto(id1, characterId, 0, now)]);

        var dto = new ReorderPhotosDto([id1, id2]);
        var act = async () => await _sut.ReorderAsync(PhotoTargetType.Character, characterId, OwnerId, dto);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage("*IDs*");
        _photoRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
