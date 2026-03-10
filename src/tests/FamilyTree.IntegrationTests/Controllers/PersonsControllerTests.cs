using System.Net;
using System.Net.Http.Json;
using FamilyTree.Application.DTOs.Person;
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

    private record ErrorResponse(string Message);
}
