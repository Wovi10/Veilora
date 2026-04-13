using System.Net;
using System.Net.Http.Json;
using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTree.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace FamilyTree.IntegrationTests.Controllers;

[TestFixture]
public class FamilyTreesControllerTests
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
    public async Task GetAll_Returns200WithFamilyTreeDtos()
    {
        var now = DateTime.UtcNow;
        var worldId = Guid.NewGuid();
        var dtos = new List<FamilyTreeDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Smith Family", WorldId = worldId, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Jones Family", WorldId = worldId, CreatedAt = now, UpdatedAt = now }
        };
        _factory.FamilyTreeServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtos);

        var response = await _client.GetAsync("/api/family-trees");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<FamilyTreeDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task GetById_WhenFamilyTreeExists_Returns200WithDto()
    {
        var treeId = Guid.NewGuid();
        var worldId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var dto = new FamilyTreeDto { Id = treeId, Name = "Smith Family", WorldId = worldId, CreatedAt = now, UpdatedAt = now };
        _factory.FamilyTreeServiceMock.Setup(s => s.GetByIdAsync(treeId)).ReturnsAsync(dto);

        var response = await _client.GetAsync($"/api/family-trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<FamilyTreeDto>();
        body!.Id.Should().Be(treeId);
        body.Name.Should().Be("Smith Family");
    }

    [Test]
    public async Task GetById_WhenFamilyTreeDoesNotExist_Returns404()
    {
        var treeId = Guid.NewGuid();
        _factory.FamilyTreeServiceMock.Setup(s => s.GetByIdAsync(treeId)).ReturnsAsync((FamilyTreeDto?)null);

        var response = await _client.GetAsync($"/api/family-trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task Create_WithValidDto_Returns201WithFamilyTreeDto()
    {
        var worldId = Guid.NewGuid();
        var dto = new CreateFamilyTreeDto { Name = "New Tree", WorldId = worldId };
        var createdDto = new FamilyTreeDto
        {
            Id = Guid.NewGuid(),
            Name = "New Tree",
            WorldId = worldId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _factory.FamilyTreeServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateFamilyTreeDto>())).ReturnsAsync(createdDto);

        var response = await _client.PostAsJsonAsync("/api/family-trees", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<FamilyTreeDto>();
        body!.Name.Should().Be("New Tree");
    }

    [Test]
    public async Task Delete_WhenFamilyTreeExists_Returns204()
    {
        var treeId = Guid.NewGuid();
        _factory.FamilyTreeServiceMock.Setup(s => s.DeleteAsync(treeId)).Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/family-trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WhenFamilyTreeDoesNotExist_Returns404()
    {
        var treeId = Guid.NewGuid();
        _factory.FamilyTreeServiceMock.Setup(s => s.DeleteAsync(treeId))
            .ThrowsAsync(new NotFoundException("FamilyTree", treeId));

        var response = await _client.DeleteAsync($"/api/family-trees/{treeId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
