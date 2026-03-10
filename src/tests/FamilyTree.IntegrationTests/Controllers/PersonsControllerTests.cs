using System.Net;
using System.Net.Http.Json;
using FamilyTree.Application.DTOs.Person;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace FamilyTree.IntegrationTests.Controllers;

[TestFixture]
public class PersonsControllerTests
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

    [Test]
    public async Task GetById_WhenPersonExists_Returns200WithPersonDto()
    {
        var personId = Guid.NewGuid();
        var expectedDto = new PersonDto
        {
            Id = personId, FirstName = "Jane", LastName = "Doe", Gender = "Female",
            CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };

        _factory.PersonServiceMock.Setup(s => s.GetByIdAsync(personId)).ReturnsAsync(expectedDto);

        var response = await _client.GetAsync($"/api/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PersonDto>();
        body!.Id.Should().Be(personId);
        body.FirstName.Should().Be("Jane");
        _factory.PersonServiceMock.Verify(s => s.GetByIdAsync(personId), Times.Once);
    }

    [Test]
    public async Task GetById_WhenPersonDoesNotExist_Returns404WithErrorMessage()
    {
        var personId = Guid.NewGuid();
        var message = $"Person with ID {personId} not found";

        _factory.PersonServiceMock
            .Setup(s => s.GetByIdAsync(personId))
            .ThrowsAsync(new NotFoundException(message));

        var response = await _client.GetAsync($"/api/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        body!.Message.Should().Be(message);
    }

    [Test]
    public async Task GetAll_Returns200WithPersonDtos()
    {
        var now = DateTime.UtcNow;
        var dtos = new List<PersonDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), FirstName = "John", LastName = "Doe", Gender = "Male", CreatedAt = now, UpdatedAt = now }
        };
        _factory.PersonServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtos);

        var response = await _client.GetAsync("/api/persons");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PersonDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task Search_WithValidTerm_Returns200WithResults()
    {
        var now = DateTime.UtcNow;
        var dto = new PersonDto { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now };
        _factory.PersonServiceMock.Setup(s => s.SearchAsync("Jane")).ReturnsAsync([dto]);

        var response = await _client.GetAsync("/api/persons/search?q=Jane");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PersonDto>>();
        body!.Should().HaveCount(1);
    }

    [Test]
    public async Task Search_WithMissingTerm_Returns400()
    {
        var response = await _client.GetAsync("/api/persons/search");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetAncestors_WhenPersonExists_Returns200()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var ancestor = new PersonDto { Id = Guid.NewGuid(), FirstName = "Bob", LastName = "Doe", Gender = "Male", CreatedAt = now, UpdatedAt = now };
        _factory.PersonServiceMock.Setup(s => s.GetAncestorsAsync(personId)).ReturnsAsync([ancestor]);

        var response = await _client.GetAsync($"/api/persons/{personId}/ancestors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PersonDto>>();
        body!.Should().HaveCount(1);
    }

    [Test]
    public async Task GetAncestors_WhenPersonDoesNotExist_Returns404()
    {
        var personId = Guid.NewGuid();
        _factory.PersonServiceMock
            .Setup(s => s.GetAncestorsAsync(personId))
            .ThrowsAsync(new NotFoundException($"Person with ID {personId} not found"));

        var response = await _client.GetAsync($"/api/persons/{personId}/ancestors");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetDescendants_WhenPersonExists_Returns200()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var descendant = new PersonDto { Id = Guid.NewGuid(), FirstName = "Alice", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now };
        _factory.PersonServiceMock.Setup(s => s.GetDescendantsAsync(personId)).ReturnsAsync([descendant]);

        var response = await _client.GetAsync($"/api/persons/{personId}/descendants");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PersonDto>>();
        body!.Should().HaveCount(1);
    }

    [Test]
    public async Task GetDescendants_WhenPersonDoesNotExist_Returns404()
    {
        var personId = Guid.NewGuid();
        _factory.PersonServiceMock
            .Setup(s => s.GetDescendantsAsync(personId))
            .ThrowsAsync(new NotFoundException($"Person with ID {personId} not found"));

        var response = await _client.GetAsync($"/api/persons/{personId}/descendants");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetRelationships_WhenPersonExists_Returns200()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var rel = new RelationshipDto { Id = Guid.NewGuid(), Person1Id = personId, Person2Id = Guid.NewGuid(), RelationshipType = "Spouse", CreatedAt = now, UpdatedAt = now };
        _factory.RelationshipServiceMock.Setup(s => s.GetPersonRelationshipsAsync(personId)).ReturnsAsync([rel]);

        var response = await _client.GetAsync($"/api/persons/{personId}/relationships");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<RelationshipDto>>();
        body!.Should().HaveCount(1);
    }

    [Test]
    public async Task GetRelationships_WhenPersonDoesNotExist_Returns404()
    {
        var personId = Guid.NewGuid();
        _factory.RelationshipServiceMock
            .Setup(s => s.GetPersonRelationshipsAsync(personId))
            .ThrowsAsync(new NotFoundException($"Person with ID {personId} not found"));

        var response = await _client.GetAsync($"/api/persons/{personId}/relationships");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Create_WithValidDto_Returns201WithLocation()
    {
        var now = DateTime.UtcNow;
        var dto = new CreatePersonDto { FirstName = "Jane", LastName = "Doe", Gender = "Female" };
        var returnedDto = new PersonDto { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now };
        _factory.PersonServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(returnedDto);

        var response = await _client.PostAsJsonAsync("/api/persons", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Contain(returnedDto.Id.ToString());
    }

    [Test]
    public async Task Update_WhenPersonExists_Returns200WithUpdatedDto()
    {
        var personId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new UpdatePersonDto { FirstName = "Janet", LastName = "Doe", Gender = "Female" };
        var returnedDto = new PersonDto { Id = personId, FirstName = "Janet", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now };
        _factory.PersonServiceMock.Setup(s => s.UpdateAsync(personId, dto)).ReturnsAsync(returnedDto);

        var response = await _client.PutAsJsonAsync($"/api/persons/{personId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<PersonDto>();
        body!.FirstName.Should().Be("Janet");
    }

    [Test]
    public async Task Update_WhenPersonDoesNotExist_Returns404()
    {
        var personId = Guid.NewGuid();
        var dto = new UpdatePersonDto { FirstName = "Janet", LastName = "Doe", Gender = "Female" };
        _factory.PersonServiceMock
            .Setup(s => s.UpdateAsync(personId, dto))
            .ThrowsAsync(new NotFoundException($"Person with ID {personId} not found"));

        var response = await _client.PutAsJsonAsync($"/api/persons/{personId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Delete_WhenPersonExists_Returns204()
    {
        var personId = Guid.NewGuid();
        _factory.PersonServiceMock.Setup(s => s.DeleteAsync(personId)).Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WhenPersonDoesNotExist_Returns404()
    {
        var personId = Guid.NewGuid();
        _factory.PersonServiceMock
            .Setup(s => s.DeleteAsync(personId))
            .ThrowsAsync(new NotFoundException($"Person with ID {personId} not found"));

        var response = await _client.DeleteAsync($"/api/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private record ErrorResponse(string Message);
}
