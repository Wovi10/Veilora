using System.Net;
using System.Net.Http.Json;
using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace FamilyTree.IntegrationTests.Controllers;

[TestFixture]
public class RelationshipsControllerTests
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
    public async Task GetAll_Returns200WithRelationshipDtos()
    {
        var now = DateTime.UtcNow;
        var dtos = new List<RelationshipDto>
        {
            new() { Id = Guid.NewGuid(), Entity1Id = Guid.NewGuid(), Entity2Id = Guid.NewGuid(), RelationshipType = "Spouse", CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), Entity1Id = Guid.NewGuid(), Entity2Id = Guid.NewGuid(), RelationshipType = "ParentChildBiological", CreatedAt = now, UpdatedAt = now }
        };
        _factory.RelationshipServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtos);

        var response = await _client.GetAsync("/api/relationships");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<RelationshipDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task GetById_WhenRelationshipExists_Returns200()
    {
        var relId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new RelationshipDto { Id = relId, Entity1Id = Guid.NewGuid(), Entity2Id = Guid.NewGuid(), RelationshipType = "Spouse", CreatedAt = now, UpdatedAt = now };
        _factory.RelationshipServiceMock.Setup(s => s.GetByIdAsync(relId)).ReturnsAsync(dto);

        var response = await _client.GetAsync($"/api/relationships/{relId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RelationshipDto>();
        body!.Id.Should().Be(relId);
    }

    [Test]
    public async Task GetById_WhenRelationshipDoesNotExist_Returns404()
    {
        var relId = Guid.NewGuid();
        _factory.RelationshipServiceMock
            .Setup(s => s.GetByIdAsync(relId))
            .ReturnsAsync((RelationshipDto?)null);

        var response = await _client.GetAsync($"/api/relationships/{relId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Create_WithValidDto_Returns201WithLocation()
    {
        var now = DateTime.UtcNow;
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var dto = new CreateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse" };
        var returnedDto = new RelationshipDto { Id = Guid.NewGuid(), Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse", CreatedAt = now, UpdatedAt = now };
        _factory.RelationshipServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateRelationshipDto>())).ReturnsAsync(returnedDto);

        var response = await _client.PostAsJsonAsync("/api/relationships", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        response.Headers.Location!.ToString().Should().Contain(returnedDto.Id.ToString());
    }

    [Test]
    public async Task Create_WhenEntityNotFound_Returns404()
    {
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var dto = new CreateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Spouse" };
        _factory.RelationshipServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateRelationshipDto>()))
            .ThrowsAsync(new NotFoundException($"Entity with ID {e1Id} not found"));

        var response = await _client.PostAsJsonAsync("/api/relationships", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Update_WhenRelationshipExists_Returns200()
    {
        var relId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var e1Id = Guid.NewGuid();
        var e2Id = Guid.NewGuid();
        var dto = new UpdateRelationshipDto { Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Partner" };
        var returnedDto = new RelationshipDto { Id = relId, Entity1Id = e1Id, Entity2Id = e2Id, RelationshipType = "Partner", CreatedAt = now, UpdatedAt = now };
        _factory.RelationshipServiceMock.Setup(s => s.UpdateAsync(relId, It.IsAny<UpdateRelationshipDto>())).ReturnsAsync(returnedDto);

        var response = await _client.PutAsJsonAsync($"/api/relationships/{relId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<RelationshipDto>();
        body!.RelationshipType.Should().Be("Partner");
    }

    [Test]
    public async Task Update_WhenRelationshipDoesNotExist_Returns404()
    {
        var relId = Guid.NewGuid();
        var dto = new UpdateRelationshipDto { Entity1Id = Guid.NewGuid(), Entity2Id = Guid.NewGuid(), RelationshipType = "Partner" };
        _factory.RelationshipServiceMock
            .Setup(s => s.UpdateAsync(relId, It.IsAny<UpdateRelationshipDto>()))
            .ThrowsAsync(new NotFoundException($"Relationship with ID {relId} not found"));

        var response = await _client.PutAsJsonAsync($"/api/relationships/{relId}", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Delete_WhenRelationshipExists_Returns204()
    {
        var relId = Guid.NewGuid();
        _factory.RelationshipServiceMock.Setup(s => s.DeleteAsync(relId)).Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/relationships/{relId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WhenRelationshipDoesNotExist_Returns404()
    {
        var relId = Guid.NewGuid();
        _factory.RelationshipServiceMock
            .Setup(s => s.DeleteAsync(relId))
            .ThrowsAsync(new NotFoundException($"Relationship with ID {relId} not found"));

        var response = await _client.DeleteAsync($"/api/relationships/{relId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
