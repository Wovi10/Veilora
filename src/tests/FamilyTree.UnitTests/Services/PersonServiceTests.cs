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
}
