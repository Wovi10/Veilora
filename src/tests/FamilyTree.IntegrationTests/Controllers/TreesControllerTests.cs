using System.Net;
using System.Net.Http.Json;
using FamilyTree.Application.DTOs.Person;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.DTOs.Tree;
using FamilyTree.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace FamilyTree.IntegrationTests.Controllers;

[TestFixture]
public class TreesControllerTests
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
    public async Task GetAll_Returns200WithTreeDtos()
    {
        var now = DateTime.UtcNow;
        var dtos = new List<TreeDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Smith Family", CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Jones Family", CreatedAt = now, UpdatedAt = now }
        };
        _factory.TreeServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtos);

        var response = await _client.GetAsync("/api/trees");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<TreeDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task GetById_WhenTreeExists_Returns200()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new TreeDto { Id = treeId, Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        _factory.TreeServiceMock.Setup(s => s.GetByIdAsync(treeId)).ReturnsAsync(dto);

        var response = await _client.GetAsync($"/api/trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TreeDto>();
        body!.Id.Should().Be(treeId);
    }

    [Test]
    public async Task GetById_WhenTreeDoesNotExist_Returns404()
    {
        var treeId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.GetByIdAsync(treeId))
            .ThrowsAsync(new NotFoundException($"Tree with ID {treeId} not found"));

        var response = await _client.GetAsync($"/api/trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetDetails_WhenTreeExists_Returns200WithPersons()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new TreeWithPersonsDto
        {
            Id = treeId,
            Name = "Smith Family",
            Persons = [new() { Person = new PersonDto { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now } }],
            CreatedAt = now,
            UpdatedAt = now
        };
        _factory.TreeServiceMock.Setup(s => s.GetTreeWithPersonsAsync(treeId)).ReturnsAsync(dto);

        var response = await _client.GetAsync($"/api/trees/{treeId}/details");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TreeWithPersonsDto>();
        body!.Persons.Should().HaveCount(1);
    }

    [Test]
    public async Task GetDetails_WhenTreeDoesNotExist_Returns404()
    {
        var treeId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.GetTreeWithPersonsAsync(treeId))
            .ThrowsAsync(new NotFoundException($"Tree with ID {treeId} not found"));

        var response = await _client.GetAsync($"/api/trees/{treeId}/details");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetPersons_Returns200WithPersonDtos()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var persons = new List<PersonDto>
        {
            new() { Id = Guid.NewGuid(), FirstName = "Jane", LastName = "Doe", Gender = "Female", CreatedAt = now, UpdatedAt = now }
        };
        _factory.PersonServiceMock.Setup(s => s.GetPersonsByTreeIdAsync(treeId)).ReturnsAsync(persons);

        var response = await _client.GetAsync($"/api/trees/{treeId}/persons");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<PersonDto>>();
        body!.Should().HaveCount(1);
    }

    [Test]
    public async Task GetRelationships_Returns200WithRelationshipDtos()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var rels = new List<RelationshipDto>
        {
            new() { Id = Guid.NewGuid(), Person1Id = Guid.NewGuid(), Person2Id = Guid.NewGuid(), RelationshipType = "Spouse", CreatedAt = now, UpdatedAt = now }
        };
        _factory.RelationshipServiceMock.Setup(s => s.GetRelationshipsByTreeIdAsync(treeId)).ReturnsAsync(rels);

        var response = await _client.GetAsync($"/api/trees/{treeId}/relationships");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<RelationshipDto>>();
        body!.Should().HaveCount(1);
    }

    [Test]
    public async Task Create_WithValidDto_Returns201WithLocation()
    {
        var now = DateTime.UtcNow;
        var dto = new CreateTreeDto { Name = "Smith Family" };
        var returnedDto = new TreeDto { Id = Guid.NewGuid(), Name = "Smith Family", CreatedAt = now, UpdatedAt = now };
        _factory.TreeServiceMock.Setup(s => s.CreateAsync(dto)).ReturnsAsync(returnedDto);

        var response = await _client.PostAsJsonAsync("/api/trees", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Contain(returnedDto.Id.ToString());
    }

    [Test]
    public async Task Update_WhenTreeExists_Returns200()
    {
        var treeId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new UpdateTreeDto { Name = "Smith Family Updated" };
        var returnedDto = new TreeDto { Id = treeId, Name = "Smith Family Updated", CreatedAt = now, UpdatedAt = now };
        _factory.TreeServiceMock.Setup(s => s.UpdateAsync(treeId, dto)).ReturnsAsync(returnedDto);

        var response = await _client.PutAsJsonAsync($"/api/trees/{treeId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<TreeDto>();
        body!.Name.Should().Be("Smith Family Updated");
    }

    [Test]
    public async Task Update_WhenTreeDoesNotExist_Returns404()
    {
        var treeId = Guid.NewGuid();
        var dto = new UpdateTreeDto { Name = "Smith Family Updated" };
        _factory.TreeServiceMock
            .Setup(s => s.UpdateAsync(treeId, dto))
            .ThrowsAsync(new NotFoundException($"Tree with ID {treeId} not found"));

        var response = await _client.PutAsJsonAsync($"/api/trees/{treeId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Delete_WhenTreeExists_Returns204()
    {
        var treeId = Guid.NewGuid();
        _factory.TreeServiceMock.Setup(s => s.DeleteAsync(treeId)).Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WhenTreeDoesNotExist_Returns404()
    {
        var treeId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.DeleteAsync(treeId))
            .ThrowsAsync(new NotFoundException($"Tree with ID {treeId} not found"));

        var response = await _client.DeleteAsync($"/api/trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddPerson_WhenValid_Returns204()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _factory.TreeServiceMock.Setup(s => s.AddPersonToTreeAsync(treeId, personId)).Returns(Task.CompletedTask);

        var response = await _client.PostAsync($"/api/trees/{treeId}/persons/{personId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task AddPerson_WhenTreeNotFound_Returns404()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.AddPersonToTreeAsync(treeId, personId))
            .ThrowsAsync(new NotFoundException($"Tree with ID {treeId} not found"));

        var response = await _client.PostAsync($"/api/trees/{treeId}/persons/{personId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task AddPerson_WhenAlreadyInTree_Returns422()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.AddPersonToTreeAsync(treeId, personId))
            .ThrowsAsync(new BusinessException($"Person with ID {personId} is already in tree {treeId}"));

        var response = await _client.PostAsync($"/api/trees/{treeId}/persons/{personId}", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async Task RemovePerson_WhenValid_Returns204()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _factory.TreeServiceMock.Setup(s => s.RemovePersonFromTreeAsync(treeId, personId)).Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/trees/{treeId}/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task RemovePerson_WhenTreeNotFound_Returns404()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.RemovePersonFromTreeAsync(treeId, personId))
            .ThrowsAsync(new NotFoundException($"Tree with ID {treeId} not found"));

        var response = await _client.DeleteAsync($"/api/trees/{treeId}/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task RemovePerson_WhenNotInTree_Returns422()
    {
        var treeId = Guid.NewGuid();
        var personId = Guid.NewGuid();
        _factory.TreeServiceMock
            .Setup(s => s.RemovePersonFromTreeAsync(treeId, personId))
            .ThrowsAsync(new BusinessException($"Person with ID {personId} is not in tree {treeId}"));

        var response = await _client.DeleteAsync($"/api/trees/{treeId}/persons/{personId}");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private record ErrorResponse(string Message);
}
