using System.Net;
using System.Net.Http.Json;
using Veilora.Application.DTOs.Entity;
using Veilora.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace Veilora.IntegrationTests.Controllers;

[TestFixture]
public class EntitiesControllerTests
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
    public async Task GetById_WhenEntityExists_Returns200WithEntityDto()
    {
        var entityId = Guid.NewGuid();
        var expectedDto = new EntityDto
        {
            Id = entityId,
            Name = "Jane Doe",
            Type = "Character",
            WorldId = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _factory.EntityServiceMock.Setup(s => s.GetByIdAsync(entityId)).ReturnsAsync(expectedDto);

        var response = await _client.GetAsync($"/api/entities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<EntityDto>();
        body!.Id.Should().Be(entityId);
        body.Name.Should().Be("Jane Doe");
        _factory.EntityServiceMock.Verify(s => s.GetByIdAsync(entityId), Times.Once);
    }

    [Test]
    public async Task GetById_WhenEntityDoesNotExist_Returns404()
    {
        var entityId = Guid.NewGuid();
        _factory.EntityServiceMock.Setup(s => s.GetByIdAsync(entityId)).ReturnsAsync((EntityDto?)null);

        var response = await _client.GetAsync($"/api/entities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetAll_Returns200WithEntityDtos()
    {
        var now = DateTime.UtcNow;
        var worldId = Guid.NewGuid();
        var dtos = new List<EntityDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Gandalf", Type = "Character", WorldId = worldId, CreatedAt = now, UpdatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Rivendell", Type = "Place", WorldId = worldId, CreatedAt = now, UpdatedAt = now }
        };
        _factory.EntityServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(dtos);

        var response = await _client.GetAsync("/api/entities");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<List<EntityDto>>();
        body!.Should().HaveCount(2);
    }

    [Test]
    public async Task Create_WithValidDto_Returns201WithEntityDto()
    {
        var worldId = Guid.NewGuid();
        var dto = new CreateEntityDto { Name = "Gandalf", Type = "Character", WorldId = worldId };
        var createdDto = new EntityDto
        {
            Id = Guid.NewGuid(),
            Name = "Gandalf",
            Type = "Character",
            WorldId = worldId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _factory.EntityServiceMock.Setup(s => s.CreateAsync(It.IsAny<CreateEntityDto>())).ReturnsAsync(createdDto);

        var response = await _client.PostAsJsonAsync("/api/entities", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var body = await response.Content.ReadFromJsonAsync<EntityDto>();
        body!.Name.Should().Be("Gandalf");
    }

    [Test]
    public async Task Delete_WhenEntityExists_Returns204()
    {
        var entityId = Guid.NewGuid();
        _factory.EntityServiceMock.Setup(s => s.DeleteAsync(entityId)).Returns(Task.CompletedTask);

        var response = await _client.DeleteAsync($"/api/entities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Test]
    public async Task Delete_WhenEntityDoesNotExist_Returns404()
    {
        var entityId = Guid.NewGuid();
        _factory.EntityServiceMock.Setup(s => s.DeleteAsync(entityId))
            .ThrowsAsync(new NotFoundException(nameof(EntityDto), entityId));

        var response = await _client.DeleteAsync($"/api/entities/{entityId}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
