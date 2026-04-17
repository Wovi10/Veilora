using Veilora.Application.DTOs.Relationship;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;
using FluentAssertions;
using Moq;

namespace Veilora.UnitTests.Services;

[TestFixture]
public class RelationshipServiceTests
{
    private Mock<IRelationshipRepository> _relationshipRepositoryMock;
    private Mock<ICharacterRepository> _characterRepositoryMock;
    private RelationshipService _sut;

    [SetUp]
    public void SetUp()
    {
        _relationshipRepositoryMock = new Mock<IRelationshipRepository>();
        _characterRepositoryMock = new Mock<ICharacterRepository>();
        _sut = new RelationshipService(_relationshipRepositoryMock.Object, _characterRepositoryMock.Object);
    }

    private static Relationship MakeRelationship(Guid id, Guid c1Id, Guid c2Id, DateTime now) => new()
    {
        Id = id,
        Character1Id = c1Id,
        Character2Id = c2Id,
        RelationshipType = RelationshipType.Spouse,
        CreatedAt = now,
        UpdatedAt = now
    };

    private static Character MakeCharacter(Guid id, DateTime now) => new()
    {
        Id = id,
        Name = "Alice Smith",
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
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, c1Id, c2Id, now);
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);

        var result = await _sut.GetByIdAsync(relId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(relId);
        result.Character1Id.Should().Be(c1Id);
        result.Character2Id.Should().Be(c2Id);
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
    public async Task CreateAsync_WhenBothCharactersExist_CreatesAndReturnsDto()
    {
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new CreateRelationshipDto { Character1Id = c1Id, Character2Id = c2Id, RelationshipType = "Spouse" };
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c1Id)).ReturnsAsync(MakeCharacter(c1Id, now));
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c2Id)).ReturnsAsync(MakeCharacter(c2Id, now));
        _relationshipRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Relationship>())).ReturnsAsync((Relationship rel) => rel);
        _relationshipRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Character1Id.Should().Be(c1Id);
        result.Character2Id.Should().Be(c2Id);
        result.RelationshipType.Should().Be("Spouse");
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Once);
        _relationshipRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WhenCharacter1NotFound_ThrowsNotFoundException()
    {
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();
        var dto = new CreateRelationshipDto { Character1Id = c1Id, Character2Id = c2Id, RelationshipType = "Spouse" };
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c1Id)).ReturnsAsync((Character?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WhenCharacter2NotFound_ThrowsNotFoundException()
    {
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new CreateRelationshipDto { Character1Id = c1Id, Character2Id = c2Id, RelationshipType = "Spouse" };
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c1Id)).ReturnsAsync(MakeCharacter(c1Id, now));
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c2Id)).ReturnsAsync((Character?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WhenRelationshipAndCharactersExist_UpdatesAndReturnsDto()
    {
        var relId = Guid.NewGuid();
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, c1Id, c2Id, now);
        var dto = new UpdateRelationshipDto { Character1Id = c1Id, Character2Id = c2Id, RelationshipType = "Partner" };
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c1Id)).ReturnsAsync(MakeCharacter(c1Id, now));
        _characterRepositoryMock.Setup(r => r.GetByIdAsync(c2Id)).ReturnsAsync(MakeCharacter(c2Id, now));
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
        var c1Id = Guid.NewGuid();
        var c2Id = Guid.NewGuid();
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync((Relationship?)null);

        var act = async () => await _sut.UpdateAsync(relId, new UpdateRelationshipDto { Character1Id = c1Id, Character2Id = c2Id, RelationshipType = "Spouse" });

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
