using FamilyTree.Application.DTOs.Person;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;
using FluentAssertions;
using Moq;

namespace FamilyTree.UnitTests.Services;

[TestFixture]
public class PersonServiceTests
{
    private Mock<IPersonRepository> _personRepositoryMock;
    private Mock<ITreeRepository> _treeRepositoryMock;
    private PersonService _sut;

    [SetUp]
    public void SetUp()
    {
        _personRepositoryMock = new Mock<IPersonRepository>();
        _treeRepositoryMock = new Mock<ITreeRepository>();
        _sut = new PersonService(_personRepositoryMock.Object, _treeRepositoryMock.Object);
    }

    [Test]
    public async Task GetByIdAsync_WhenPersonExists_ReturnsMappedDto()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var person = new Person
        {
            Id = personId, FirstName = "Jane", LastName = "Doe",
            Gender = Gender.Female, BirthDate = new DateTime(1990, 6, 15),
            CreatedAt = now, UpdatedAt = now
        };

        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);

        var result = await _sut.GetByIdAsync(personId);

        result.Should().NotBeNull();
        result!.Id.Should().Be(personId);
        result.FirstName.Should().Be("Jane");
        result.LastName.Should().Be("Doe");
        result.Gender.Should().Be("Female");
        result.BirthDate.Should().Be(person.BirthDate);
        _personRepositoryMock.Verify(r => r.GetByIdAsync(personId), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenPersonDoesNotExist_ThrowsNotFoundException()
    {
        var personId = Guid.NewGuid();
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.GetByIdAsync(personId);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage($"Person with ID {personId} not found");
    }

    [Test]
    public async Task GetAllAsync_WhenPersonsExist_ReturnsMappedDtos()
    {
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var id2 = Guid.NewGuid();
        var persons = new List<Person>
        {
            new() { Id = id1, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now },
            new() { Id = id2, FirstName = "Bob", LastName = "Smith", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now }
        };
        _personRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(persons);

        var result = (await _sut.GetAllAsync()).ToList();

        result.Should().HaveCount(2);
        result.Select(p => p.Id).Should().Contain([id1, id2]);
    }

    [Test]
    public async Task GetAllAsync_WhenNoPersonsExist_ReturnsEmptyCollection()
    {
        _personRepositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
    }

    [Test]
    public async Task GetPersonsByTreeIdAsync_ReturnsMappedDtos()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var persons = new List<Person>
        {
            new() { Id = id1, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now }
        };
        _personRepositoryMock.Setup(r => r.GetPersonsByTreeIdAsync(treeId)).ReturnsAsync(persons);

        var result = (await _sut.GetPersonsByTreeIdAsync(treeId)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(id1);
        _personRepositoryMock.Verify(r => r.GetPersonsByTreeIdAsync(treeId), Times.Once);
    }

    [Test]
    public async Task CreateAsync_CreatesPersonAndReturnsMappedDto()
    {
        var dto = new CreatePersonDto { FirstName = "Alice", LastName = "Smith", Gender = "Female" };
        _personRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Person>())).ReturnsAsync((Person p) => p);
        _personRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.CreateAsync(dto);

        result.FirstName.Should().Be("Alice");
        result.LastName.Should().Be("Smith");
        result.Gender.Should().Be("Female");
        _personRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Person>()), Times.Once);
        _personRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenPersonExists_UpdatesAndReturnsMappedDto()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var person = new Person { Id = personId, FirstName = "Old", LastName = "Name", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now };
        var dto = new UpdatePersonDto { FirstName = "New", LastName = "Name", Gender = "Female" };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _personRepositoryMock.Setup(r => r.UpdateAsync(person)).Returns(Task.CompletedTask);
        _personRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _sut.UpdateAsync(personId, dto);

        result.FirstName.Should().Be("New");
        result.Gender.Should().Be("Female");
        _personRepositoryMock.Verify(r => r.UpdateAsync(person), Times.Once);
        _personRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenPersonDoesNotExist_ThrowsNotFoundException()
    {
        var personId = Guid.NewGuid();
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.UpdateAsync(personId, new UpdatePersonDto { FirstName = "X", LastName = "Y", Gender = "Male" });

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
    }

    [Test]
    public async Task DeleteAsync_WhenPersonExists_DeletesAndSaves()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var person = new Person { Id = personId, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _personRepositoryMock.Setup(r => r.DeleteAsync(person)).Returns(Task.CompletedTask);
        _personRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.DeleteAsync(personId);

        _personRepositoryMock.Verify(r => r.DeleteAsync(person), Times.Once);
        _personRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenPersonDoesNotExist_ThrowsNotFoundException()
    {
        var personId = Guid.NewGuid();
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.DeleteAsync(personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
    }

    [Test]
    public async Task SearchAsync_ReturnsMappedResults()
    {
        var now = DateTime.UtcNow;
        var id1 = Guid.NewGuid();
        var persons = new List<Person>
        {
            new() { Id = id1, FirstName = "Alice", LastName = "Smith", Gender = Gender.Female, CreatedAt = now, UpdatedAt = now }
        };
        _personRepositoryMock.Setup(r => r.SearchAsync("Alice")).ReturnsAsync(persons);

        var result = (await _sut.SearchAsync("Alice")).ToList();

        result.Should().HaveCount(1);
        result[0].FirstName.Should().Be("Alice");
        _personRepositoryMock.Verify(r => r.SearchAsync("Alice"), Times.Once);
    }

    [Test]
    public async Task GetAncestorsAsync_WhenPersonExists_ReturnsMappedAncestors()
    {
        var personId = Guid.NewGuid();
        var ancestorId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var person = new Person { Id = personId, FirstName = "Child", LastName = "Doe", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now };
        var ancestors = new List<Person>
        {
            new() { Id = ancestorId, FirstName = "Parent", LastName = "Doe", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now }
        };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _personRepositoryMock.Setup(r => r.GetAncestorsAsync(personId)).ReturnsAsync(ancestors);

        var result = (await _sut.GetAncestorsAsync(personId)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(ancestorId);
        _personRepositoryMock.Verify(r => r.GetAncestorsAsync(personId), Times.Once);
    }

    [Test]
    public async Task GetAncestorsAsync_WhenPersonDoesNotExist_ThrowsNotFoundException()
    {
        var personId = Guid.NewGuid();
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.GetAncestorsAsync(personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
    }

    [Test]
    public async Task GetDescendantsAsync_WhenPersonExists_ReturnsMappedDescendants()
    {
        var personId = Guid.NewGuid();
        var descendantId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var person = new Person { Id = personId, FirstName = "Parent", LastName = "Doe", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now };
        var descendants = new List<Person>
        {
            new() { Id = descendantId, FirstName = "Child", LastName = "Doe", Gender = Gender.Male, CreatedAt = now, UpdatedAt = now }
        };
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync(person);
        _personRepositoryMock.Setup(r => r.GetDescendantsAsync(personId)).ReturnsAsync(descendants);

        var result = (await _sut.GetDescendantsAsync(personId)).ToList();

        result.Should().HaveCount(1);
        result[0].Id.Should().Be(descendantId);
        _personRepositoryMock.Verify(r => r.GetDescendantsAsync(personId), Times.Once);
    }

    [Test]
    public async Task GetDescendantsAsync_WhenPersonDoesNotExist_ThrowsNotFoundException()
    {
        var personId = Guid.NewGuid();
        _personRepositoryMock.Setup(r => r.GetByIdAsync(personId)).ReturnsAsync((Person?)null);

        var act = async () => await _sut.GetDescendantsAsync(personId);

        await act.Should().ThrowAsync<NotFoundException>().WithMessage($"Person with ID {personId} not found");
    }
}
