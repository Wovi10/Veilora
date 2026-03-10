using FamilyTree.Application.Exceptions;
using FamilyTree.Application.DTOs.Relationship;
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
    private Mock<IPersonRepository> _personRepositoryMock;
    private RelationshipService _sut;

    [SetUp]
    public void SetUp()
    {
        _relationshipRepositoryMock = new Mock<IRelationshipRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _sut = new RelationshipService(_relationshipRepositoryMock.Object, _personRepositoryMock.Object);
    }

    private static Relationship MakeRelationship(Guid id, Guid p1Id, Guid p2Id, DateTime now) => new()
    {
        Id = id, Person1Id = p1Id, Person2Id = p2Id,
        RelationshipType = RelationshipType.Spouse,
        CreatedAt = now, UpdatedAt = now
    };

    private static Person MakePerson(Guid id, DateTime now) => new()
    {
        Id = id, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female,
        CreatedAt = now, UpdatedAt = now
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
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, p1Id, p2Id, now);
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);

        var result = await _sut.GetByIdAsync(relId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(relId);
        result.Person1Id.Should().Be(p1Id);
        result.Person2Id.Should().Be(p2Id);
        result.RelationshipType.Should().Be("Spouse");
    }

    [Test]
    public async Task GetByIdAsync_WhenRelationshipDoesNotExist_ThrowsNotFoundException()
    {
        var relId = Guid.NewGuid();
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync((Relationship?)null);

        var act = async () => await _sut.GetByIdAsync(relId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Relationship with ID {relId} not found");
    }

    [Test]
    public async Task CreateAsync_WhenBothPersonsExist_CreatesAndReturnsDto()
    {
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new CreateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Spouse" };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p1Id)).ReturnsAsync(MakePerson(p1Id, now));
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p2Id)).ReturnsAsync(MakePerson(p2Id, now));
        _relationshipRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Relationship>())).ReturnsAsync((Relationship rel) => rel);
        _relationshipRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Person1Id.Should().Be(p1Id);
        result.Person2Id.Should().Be(p2Id);
        result.RelationshipType.Should().Be("Spouse");
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Once);
        _relationshipRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task CreateAsync_WhenPerson1NotFound_ThrowsNotFoundException()
    {
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var dto = new CreateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Spouse" };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p1Id)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {p1Id} not found");
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Never);
    }

    [Test]
    public async Task CreateAsync_WhenPerson2NotFound_ThrowsNotFoundException()
    {
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new CreateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Spouse" };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p1Id)).ReturnsAsync(MakePerson(p1Id, now));
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p2Id)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {p2Id} not found");
        _relationshipRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Relationship>()), Times.Never);
    }

    [Test]
    public async Task UpdateAsync_WhenRelationshipAndPersonsExist_UpdatesAndReturnsDto()
    {
        var relId = Guid.NewGuid();
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, p1Id, p2Id, now);
        var dto = new UpdateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Partner" };
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p1Id)).ReturnsAsync(MakePerson(p1Id, now));
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p2Id)).ReturnsAsync(MakePerson(p2Id, now));
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
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync((Relationship?)null);

        var act = async () => await _sut.UpdateAsync(relId, new UpdateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Spouse" });

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Relationship with ID {relId} not found");
    }

    [Test]
    public async Task UpdateAsync_WhenPerson1NotFound_ThrowsNotFoundException()
    {
        var relId = Guid.NewGuid();
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, p1Id, p2Id, now);
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p1Id)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.UpdateAsync(relId, new UpdateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Spouse" });

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {p1Id} not found");
    }

    [Test]
    public async Task UpdateAsync_WhenPerson2NotFound_ThrowsNotFoundException()
    {
        var relId = Guid.NewGuid();
        var p1Id = Guid.NewGuid();
        var p2Id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relationship = MakeRelationship(relId, p1Id, p2Id, now);
        _relationshipRepositoryMock.Setup(r => r.GetByIdAsync(relId)).ReturnsAsync(relationship);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p1Id)).ReturnsAsync(MakePerson(p1Id, now));
        _personRepositoryMock.Setup(r => r.GetByIdAsync(p2Id)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.UpdateAsync(relId, new UpdateRelationshipDto { Person1Id = p1Id, Person2Id = p2Id, RelationshipType = "Spouse" });

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {p2Id} not found");
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

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Relationship with ID {relId} not found");
    }

    [Test]
    public async Task GetRelationshipsByTreeIdAsync_ReturnsMappedDtos()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var relationships = new List<Relationship>
        {
            MakeRelationship(id1, Guid.NewGuid(), Guid.NewGuid(), now)
        };
        _relationshipRepositoryMock.Setup(r => r.GetRelationshipsByTreeIdAsync(treeId)).ReturnsAsync(relationships);

        var result = (await _sut.GetRelationshipsByTreeIdAsync(treeId)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(id1);
        _relationshipRepositoryMock.Verify(r => r.GetRelationshipsByTreeIdAsync(treeId), Times.Once);
    }

    [Test]
    public async Task GetPersonRelationshipsAsync_WhenPersonExists_ReturnsMappedDtos()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var relId = Guid.NewGuid();
        var relationships = new List<Relationship>
        {
            MakeRelationship(relId, personId, Guid.NewGuid(), now)
        };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(MakePerson(personId, now));
        _relationshipRepositoryMock.Setup(r => r.GetPersonRelationshipsAsync(personId)).ReturnsAsync(relationships);

        var result = (await _sut.GetPersonRelationshipsAsync(personId)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(relId);
        _relationshipRepositoryMock.Verify(r => r.GetPersonRelationshipsAsync(personId), Times.Once);
    }

    [Test]
    public async Task GetPersonRelationshipsAsync_WhenPersonNotFound_ThrowsNotFoundException()
    {
        var personId = Guid.NewGuid();
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.GetPersonRelationshipsAsync(personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
    }
}
