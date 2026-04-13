using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;
using FluentAssertions;
using Moq;

namespace FamilyTree.UnitTests.Services;

[TestFixture]
public class RelationshipServiceTests
{
    private Mock<IRelationshipRepository> _relationshipRepositoryMock;
    private Mock<IEntityRepository> _entityRepositoryMock;
    private RelationshipService _sut;

    [SetUp]
    public void SetUp()
    {
        _relationshipRepositoryMock = new Mock<IRelationshipRepository>();
        _entityRepositoryMock = new Mock<IEntityRepository>();
        _sut = new RelationshipService(_relationshipRepositoryMock.Object, _entityRepositoryMock.Object);
    }

    private static Relationship MakeRelationship(Guid id, Guid e1Id, Guid e2Id, DateTime now) => new()
    {
        Id = id,
        Entity1Id = e1Id,
        Entity2Id = e2Id,
        RelationshipType = RelationshipType.Spouse,
        CreatedAt = now,
        UpdatedAt = now
    };

    private static Entity MakeEntity(Guid id, DateTime now) => new()
    {
        Id = id,
        Name = "Alice Smith",
        Type = EntityType.Character,
        WorldId = Guid.NewGuid(),
        CreatedAt = now,
        UpdatedAt = now
    };

    [Test]
    public async Task GetAllAsync_WhenRelationshipsExist_ReturnsMappedDtos()
    {
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var relationships = new List<Relationship>
        {
            MakeRelationship(id1, Guid.NewGuid(), Guid.NewGuid(), now),
            MakeRelationship(id2, Guid.NewGuid(), Guid.NewGuid(), now)
        };
        _relationshipRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(relationships);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Select(r => r.Id).Should().Contain([id1, id2]);
    }

    [Test]
    public async Task GetAllAsync_WhenNoRelationshipsExist_ReturnsEmptyCollection()
    {
        _relationshipRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetByIdAsync_WhenRelationshipExists_ReturnsMappedDto()
    {
        var relId = Guid.NewGuid();
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, e1Id, e2Id, now);
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);

        var result = await _sut.GetByIdAsync(relId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(relId);
        result.Entity1Id.Should().Be(e1Id);
        result.Entity2Id.Should().Be(e2Id);
        result.RelationshipType.Should().Be("Spouse");
    }

    [Test]
    public async Task GetByIdAsync_WhenRelationshipDoesNotExist_ReturnsNull()
    {
        var relId = Guid.NewGuid();
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync((Relationship?)null);

        var result = await _sut.GetByIdAsync(relId);

        result.Should().BeNull();
    }

    [Test]
    public async Task CreateAsync_WhenBothEntitiesExist_CreatesAndReturnsDto()
    {
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new CreateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse" };
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e1Id)).ReturnsAsync(MakeEntity(e1Id, now));
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e2Id)).ReturnsAsync(MakeEntity(e2Id, now));
        _relationshipRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Relationship>())).ReturnsAsync((Relationship rel) => rel);
        _relationshipRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Entity1Id.Should().Be(e1Id);
        result.Entity2Id.Should().Be(e2Id);
        result.RelationshipType.Should().Be("Spouse");
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Once);
        _relationshipRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WhenEntity1NotFound_ThrowsNotFoundException()
    {
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var dto = new CreateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse" };
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e1Id)).ReturnsAsync((Entity?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WhenEntity2NotFound_ThrowsNotFoundException()
    {
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new CreateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse" };
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e1Id)).ReturnsAsync(MakeEntity(e1Id, now));
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e2Id)).ReturnsAsync((Entity?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WhenRelationshipAndEntitiesExist_UpdatesAndReturnsDto()
    {
        var relId = Guid.NewGuid();
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, e1Id, e2Id, now);
        var dto = new UpdateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Partner" };
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e1Id)).ReturnsAsync(MakeEntity(e1Id, now));
        _entityRepositoryMock.Setup(r => r.GetByIdAsync(e2Id)).ReturnsAsync(MakeEntity(e2Id, now));
        _relationshipRepositoryMock.Setup(r => r.UpdateAsync(relationship)).Returns(Task.CompletedTask);
        _relationshipRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(relId, dto);

        result.RelationshipType.Should().Be("Partner");
        _relationshipRepositoryMock.Verify(r => r.UpdateAsync(relationship), Times.Once);
        _relationshipRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenRelationshipNotFound_ThrowsNotFoundException()
    {
        var relId = Guid.NewGuid();
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync((Relationship?)null);

        var act = async () => await _sut.UpdateAsync(relId, new UpdateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse" });

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Test]
    public async Task DeleteAsync_WhenRelationshipExists_DeletesAndSaves()
    {
        var relId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, Guid.NewGuid(), Guid.NewGuid(), now);
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);
        _relationshipRepositoryMock.Setup(r => r.DeleteAsync(relationship)).Returns(Task.CompletedTask);
        _relationshipRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(relId);

        _relationshipRepositoryMock.Verify(r => r.DeleteAsync(relationship), Times.Once);
        _relationshipRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenRelationshipDoesNotExist_ThrowsNotFoundException()
    {
        var relId = Guid.NewGuid();
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync((Relationship?)null);

        var act = async () => await _sut.DeleteAsync(relId);

        await act.Should().ThrowAsync<NotFoundException>();
    }
}
