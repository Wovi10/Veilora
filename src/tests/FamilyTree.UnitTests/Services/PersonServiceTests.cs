using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;
using FluentAssertions;
using Moq;

namespace FamilyTree.UnitTests.Services;

[TestFixture]
public class EntityServiceTests
{
    private Mock<IEntityRepository> _entityRepositoryMock;
    private Mock<IWorldRepository> _worldRepositoryMock;
    private EntityService _sut;

    [SetUp]
    public void SetUp()
    {
        _entityRepositoryMock = new Mock<IEntityRepository>();
        _worldRepositoryMock = new Mock<IWorldRepository>();
        _sut = new EntityService(_entityRepositoryMock.Object, _worldRepositoryMock.Object);
    }

    private static Entity MakeEntity(Guid id, DateTime now) => new()
    {
        Id = id,
        Name = "The Shire",
        Type = EntityType.Place,
        WorldId = Guid.NewGuid(),
        CreatedAt = now,
        UpdatedAt = now
    };

    [Test]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsMappedDto()
    {
        var entityId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var entity = MakeEntity(entityId, now);

        _entityRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync(entity);

        var result = await _sut.GetByIdAsync(entityId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(entityId);
        result.Name.Should().Be("The Shire");
        result.Type.Should().Be("Place");
        _entityRepositoryMock.Verify(r => r.GetByIdAsync(entityId), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        var entityId = Guid.NewGuid();
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync((Entity?)null);

        var result = await _sut.GetByIdAsync(entityId);

        result.Should().BeNull();
    }

    [Test]
    public async Task GetAllAsync_WhenEntitiesExist_ReturnsMappedDtos()
    {
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var entities = new List<Entity>
        {
            MakeEntity(id1, now),
            MakeEntity(id2, now)
        };
        _entityRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Select(e => e.Id).Should().Contain([id1, id2]);
    }

    [Test]
    public async Task GetAllAsync_WhenNoEntitiesExist_ReturnsEmptyCollection()
    {
        _entityRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Test]
    public async Task CreateAsync_WhenWorldExists_CreatesEntityAndReturnsMappedDto()
    {
        var worldId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var world = new World { Id = worldId, Name = "Middle Earth", CreatedAt = now, UpdatedAt = now };
        var dto = new CreateEntityDto { Name = "Rivendell", Type = "Place", WorldId = worldId };
        _worldRepositoryMock.Setup(r => r.GetByIdAsync(worldId)).ReturnsAsync(world);
        _entityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Entity>())).ReturnsAsync((Entity e) => e);
        _entityRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("Rivendell");
        result.Type.Should().Be("Place");
        _entityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Entity>()), Times.Once);
        _entityRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WhenWorldNotFound_ThrowsNotFoundException()
    {
        var worldId = Guid.NewGuid();
        var dto = new CreateEntityDto { Name = "Rivendell", Type = "Place", WorldId = worldId };
        _worldRepositoryMock.Setup(r => r.GetByIdAsync(worldId)).ReturnsAsync((World?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _entityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Entity>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WhenEntityExists_UpdatesAndReturnsMappedDto()
    {
        var entityId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var entity = MakeEntity(entityId, now);
        var dto = new UpdateEntityDto { Name = "Rivendell", Type = "Place" };
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync(entity);
        _entityRepositoryMock.Setup(r => r.UpdateAsync(entity)).Returns(Task.CompletedTask);
        _entityRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(entityId, dto);

        result.Name.Should().Be("Rivendell");
        _entityRepositoryMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        _entityRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenEntityDoesNotExist_ThrowsNotFoundException()
    {
        var entityId = Guid.NewGuid();
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync((Entity?)null);

        var act = async () => await _sut.UpdateAsync(entityId, new UpdateEntityDto { Name = "X", Type = "Place" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DeleteAsync_WhenEntityExists_DeletesAndSaves()
    {
        var entityId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var entity = MakeEntity(entityId, now);
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync(entity);
        _entityRepositoryMock.Setup(r => r.DeleteAsync(entity)).Returns(Task.CompletedTask);
        _entityRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(entityId);

        _entityRepositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
        _entityRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenEntityDoesNotExist_ThrowsNotFoundException()
    {
        var entityId = Guid.NewGuid();
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(entityId)).ReturnsAsync((Entity?)null);

        var act = async () => await _sut.DeleteAsync(entityId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
