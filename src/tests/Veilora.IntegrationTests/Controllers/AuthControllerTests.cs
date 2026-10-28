using System.Net;
using System.Net.Http.Json;
using Veilora.Application.DTOs.Auth;
using Veilora.Application.Exceptions;
using FluentAssertions;
using Moq;

namespace Veilora.IntegrationTests.Controllers;

[TestFixture]
public class AuthControllerTests
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

    // --- forgot-password ---

    [Test]
    public async Task ForgotPassword_WhenCalled_Returns200WithGenericMessage()
    {
        _factory.AuthServiceMock
            .Setup(s => s.ForgotPasswordAsync(It.IsAny<ForgotPasswordDto>()))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsJsonAsync("/api/auth/forgot-password", new ForgotPasswordDto("someone@example.com"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        _factory.AuthServiceMock.Verify(
            s => s.ForgotPasswordAsync(It.Is<ForgotPasswordDto>(d => d.Email == "someone@example.com")),
            Times.Once);
    }

    // --- reset-password ---

    [Test]
    public async Task ResetPassword_WhenTokenValid_Returns200()
    {
        _factory.AuthServiceMock
            .Setup(s => s.ResetPasswordAsync(It.IsAny<ResetPasswordDto>()))
            .Returns(Task.CompletedTask);

        var response = await _client.PostAsJsonAsync("/api/auth/reset-password", new ResetPasswordDto("valid-token", "NewPassword123"));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Test]
    public async Task ResetPassword_WhenTokenInvalidOrExpired_Returns422()
    {
        _factory.AuthServiceMock
            .Setup(s => s.ResetPasswordAsync(It.IsAny<ResetPasswordDto>()))
            .ThrowsAsync(new BusinessException("Invalid or expired reset link."));

        var response = await _client.PostAsJsonAsync("/api/auth/reset-password", new ResetPasswordDto("bad-token", "NewPassword123"));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
