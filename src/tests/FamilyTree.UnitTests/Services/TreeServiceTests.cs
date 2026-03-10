using FamilyTree.Application.Exceptions;
using FamilyTree.Application.DTOs.Tree;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;
using FluentAssertions;
using Moq;

namespace FamilyTree.UnitTests.Services;

[TestFixture]
public class TreeServiceTests
{
    private Mock<ITreeRepository> _treeRepositoryMock;
    private Mock<IPersonRepository> _personRepositoryMock;
    private TreeService _sut;

    [SetUp]
    public void SetUp()
    {
        _treeRepositoryMock = new Mock<ITreeRepository>();
        _personRepositoryMock = new Mock<IPersonRepository>();
        _sut = new TreeService(_treeRepositoryMock.Object, _personRepositoryMock.Object);
    }

    [Test]
    public async Task GetAllAsync_WhenTreesExist_ReturnsMappedDtos()
    {
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var trees = new List<Tree>
        {
            new() { Id = id1, Name = "Smith Family", CreatedAt = now, UpdatedAt = now },
            new() { Id = id2, Name = "Jones Family", CreatedAt = now, UpdatedAt = now }
        };
        _treeRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(trees);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Select(t => t.Id).Should().Contain([id1, id2]);
    }

    [Test]
    public async Task GetAllAsync_WhenNoTreesExist_ReturnsEmptyCollection()
    {
        _treeRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetByIdAsync_WhenTreeExists_ReturnsMappedDto()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", Description = "Desc", CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);

        var result = await _sut.GetByIdAsync(treeId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(treeId);
        result.Name.Should().Be("Smith Family");
        result.Description.Should().Be("Desc");
    }

    [Test]
    public async Task GetByIdAsync_WhenTreeDoesNotExist_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((Tree?)null);

        var act = async () => await _sut.GetByIdAsync(treeId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Tree with ID {treeId} not found");
    }

    [Test]
    public async Task GetTreeWithPersonsAsync_WhenTreeExists_ReturnsDtoWithPersons()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        var persons = new List<Person>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Smith", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now }
        };
        _treeRepositoryMock.Setup(r => r.GetTreeWithPersonsAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetPersonsByTreeIdAsync(treeId)).ReturnsAsync(persons);

        var result = await _sut.GetTreeWithPersonsAsync(treeId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(treeId);
        result.Persons.Should().HaveCount(2);
    }

    [Test]
    public async Task GetTreeWithPersonsAsync_WhenTreeDoesNotExist_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        _treeRepositoryMock.Setup(r => r.GetTreeWithPersonsAsync(treeId)).ReturnsAsync((Tree?)null);

        var act = async () => await _sut.GetTreeWithPersonsAsync(treeId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Tree with ID {treeId} not found");
    }

    [Test]
    public async Task CreateAsync_CreatesTreeAndReturnsMappedDto()
    {
        var dto = new CreateTreeDto { Name = "New Tree", Description = "A new tree" };
        _treeRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Tree>())).ReturnsAsync((Tree t) => t);
        _treeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.Name.Should().Be("New Tree");
        result.Description.Should().Be("A new tree");
        _treeRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Tree>()), Times.Once);
        _treeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenTreeExists_UpdatesAndReturnsMappedDto()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Old Name", CreatedAt = now, UpdatedAt = now };
        var dto = new UpdateTreeDto { Name = "New Name", Description = "Updated" };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _treeRepositoryMock.Setup(r => r.UpdateAsync(tree)).Returns(Task.CompletedTask);
        _treeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(treeId, dto);

        result.Name.Should().Be("New Name");
        result.Description.Should().Be("Updated");
        _treeRepositoryMock.Verify(r => r.UpdateAsync(tree), Times.Once);
        _treeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenTreeDoesNotExist_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((Tree?)null);

        var act = async () => await _sut.UpdateAsync(treeId, new UpdateTreeDto { Name = "X" });

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Tree with ID {treeId} not found");
    }

    [Test]
    public async Task DeleteAsync_WhenTreeExists_DeletesAndSaves()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _treeRepositoryMock.Setup(r => r.DeleteAsync(tree)).Returns(Task.CompletedTask);
        _treeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(treeId);

        _treeRepositoryMock.Verify(r => r.DeleteAsync(tree), Times.Once);
        _treeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenTreeDoesNotExist_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((Tree?)null);

        var act = async () => await _sut.DeleteAsync(treeId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Tree with ID {treeId} not found");
    }

    [Test]
    public async Task AddPersonToTreeAsync_WhenValid_AddsAndSaves()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        var person = new Person { Id = personId, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _treeRepositoryMock.Setup(r => r.IsPersonInTreeAsync(treeId, personId)).ReturnsAsync(false);
        _treeRepositoryMock.Setup(r => r.AddPersonToTreeAsync(treeId, personId)).Returns(Task.CompletedTask);
        _treeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.AddPersonToTreeAsync(treeId, personId);

        _treeRepositoryMock.Verify(r => r.AddPersonToTreeAsync(treeId, personId), Times.Once);
        _treeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task AddPersonToTreeAsync_WhenTreeNotFound_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((Tree?)null);

        var act = async () => await _sut.AddPersonToTreeAsync(treeId, personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Tree with ID {treeId} not found");
        _treeRepositoryMock.Verify(r => r.AddPersonToTreeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task AddPersonToTreeAsync_WhenPersonNotFound_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.AddPersonToTreeAsync(treeId, personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
        _treeRepositoryMock.Verify(r => r.AddPersonToTreeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task AddPersonToTreeAsync_WhenAlreadyInTree_ThrowsBusinessException()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        var person = new Person { Id = personId, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _treeRepositoryMock.Setup(r => r.IsPersonInTreeAsync(treeId, personId)).ReturnsAsync(true);

        var act = async () => await _sut.AddPersonToTreeAsync(treeId, personId);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage($"Person with ID {personId} is already in tree {treeId}");
        _treeRepositoryMock.Verify(r => r.AddPersonToTreeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task RemovePersonFromTreeAsync_WhenValid_RemovesAndSaves()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        var person = new Person { Id = personId, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _treeRepositoryMock.Setup(r => r.IsPersonInTreeAsync(treeId, personId)).ReturnsAsync(true);
        _treeRepositoryMock.Setup(r => r.RemovePersonFromTreeAsync(treeId, personId)).Returns(Task.CompletedTask);
        _treeRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.RemovePersonFromTreeAsync(treeId, personId);

        _treeRepositoryMock.Verify(r => r.RemovePersonFromTreeAsync(treeId, personId), Times.Once);
        _treeRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task RemovePersonFromTreeAsync_WhenTreeNotFound_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync((Tree?)null);

        var act = async () => await _sut.RemovePersonFromTreeAsync(treeId, personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Tree with ID {treeId} not found");
        _treeRepositoryMock.Verify(r => r.RemovePersonFromTreeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task RemovePersonFromTreeAsync_WhenPersonNotFound_ThrowsNotFoundException()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.RemovePersonFromTreeAsync(treeId, personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
        _treeRepositoryMock.Verify(r => r.RemovePersonFromTreeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Test]
    public async Task RemovePersonFromTreeAsync_WhenNotInTree_ThrowsBusinessException()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var tree = new Tree { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        var person = new Person { Id = personId, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now };
        _treeRepositoryMock.Setup(r => r.GetByIdAsync(treeId)).ReturnsAsync(tree);
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _treeRepositoryMock.Setup(r => r.IsPersonInTreeAsync(treeId, personId)).ReturnsAsync(false);

        var act = async () => await _sut.RemovePersonFromTreeAsync(treeId, personId);

        await act.Should().ThrowAsync<BusinessException>()
            .WithMessage($"Person with ID {personId} is not in tree {treeId}");
        _treeRepositoryMock.Verify(r => r.RemovePersonFromTreeAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }
}
