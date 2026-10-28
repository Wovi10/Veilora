using Veilora.Application.DTOs.Auth;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services;
using Veilora.Application.Services.Interfaces;
using Veilora.Application.Settings;
using Veilora.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Veilora.UnitTests.Services;

[TestFixture]
public class AuthServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IPasswordResetTokenRepository> _passwordResetTokenRepositoryMock;
    private Mock<IEmailSender> _emailSenderMock;
    private PasswordResetSettings _passwordResetSettings;
    private AuthService _sut;

    [SetUp]
    public void SetUp()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordResetTokenRepositoryMock = new Mock<IPasswordResetTokenRepository>();
        _emailSenderMock = new Mock<IEmailSender>();
        _passwordResetSettings = new PasswordResetSettings("https://app.example.com", 60);

        _sut = new AuthService(
            _userRepositoryMock.Object,
            _passwordResetTokenRepositoryMock.Object,
            _emailSenderMock.Object,
            _passwordResetSettings);
    }

    private static User MakeUser(Guid id, string email, DateTime now) => new()
    {
        Id = id,
        Email = email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("OldPassword123"),
        CreatedAt = now,
        UpdatedAt = now
    };

    // --- RegisterAsync ---

    [Test]
    public async Task RegisterAsync_WhenEmailAlreadyRegistered_ThrowsInvalidOperationException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("taken@example.com"))
            .ReturnsAsync(MakeUser(Guid.NewGuid(), "taken@example.com", DateTime.UtcNow));

        var dto = new RegisterDto("taken@example.com", "Password123", "Name");
        var act = async () => await _sut.RegisterAsync(dto);

        await act.Should().ThrowAsync<InvalidOperationException>();
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task RegisterAsync_WhenEmailIsNew_CreatesUserAndReturnsDto()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("new@example.com")).ReturnsAsync((User?)null);
        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>())).ReturnsAsync((User u) => u);
        _userRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var dto = new RegisterDto("New@Example.com", "Password123", "Name");
        var result = await _sut.RegisterAsync(dto);

        result.Email.Should().Be("new@example.com");
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // --- ForgotPasswordAsync ---

    [Test]
    public async Task ForgotPasswordAsync_WhenUserNotFound_DoesNotCreateTokenOrSendEmail()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("ghost@example.com")).ReturnsAsync((User?)null);

        await _sut.ForgotPasswordAsync(new ForgotPasswordDto("ghost@example.com"));

        _passwordResetTokenRepositoryMock.Verify(r => r.AddAsync(It.IsAny<PasswordResetToken>()), Times.Never);
        _emailSenderMock.Verify(
            e => e.SendPasswordResetEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()),
            Times.Never);
    }

    [Test]
    public async Task ForgotPasswordAsync_WhenUserExists_InvalidatesOldTokensCreatesNewOneAndSendsEmail()
    {
        var user = MakeUser(Guid.NewGuid(), "known@example.com", DateTime.UtcNow);
        _userRepositoryMock.Setup(r => r.GetByEmailAsync("known@example.com")).ReturnsAsync(user);
        _passwordResetTokenRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        PasswordResetToken? savedToken = null;
        _passwordResetTokenRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<PasswordResetToken>()))
            .Callback<PasswordResetToken>(t => savedToken = t)
            .ReturnsAsync((PasswordResetToken t) => t);

        string? capturedLink = null;
        _emailSenderMock
            .Setup(e => e.SendPasswordResetEmailAsync("known@example.com", It.IsAny<string>(), 60))
            .Callback<string, string, int>((_, link, _) => capturedLink = link)
            .Returns(Task.CompletedTask);

        await _sut.ForgotPasswordAsync(new ForgotPasswordDto("known@example.com"));

        _passwordResetTokenRepositoryMock.Verify(r => r.InvalidateAllForUserAsync(user.Id), Times.Once);
        _passwordResetTokenRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        savedToken.Should().NotBeNull();
        savedToken!.UserId.Should().Be(user.Id);
        savedToken.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(60), TimeSpan.FromSeconds(5));
        capturedLink.Should().StartWith("https://app.example.com/reset-password?token=");
    }

    // --- ResetPasswordAsync ---

    [Test]
    public async Task ResetPasswordAsync_WhenTokenInvalidOrExpired_ThrowsBusinessException()
    {
        _passwordResetTokenRepositoryMock.Setup(r => r.GetValidByTokenHashAsync(It.IsAny<string>()))
            .ReturnsAsync((PasswordResetToken?)null);

        var act = async () => await _sut.ResetPasswordAsync(new ResetPasswordDto("bad-token", "NewPassword123"));

        await act.Should().ThrowAsync<BusinessException>();
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Test]
    public async Task ResetPasswordAsync_WhenUserForTokenNoLongerExists_ThrowsBusinessException()
    {
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            TokenHash = "hash",
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };
        _passwordResetTokenRepositoryMock.Setup(r => r.GetValidByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(token);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(token.UserId)).ReturnsAsync((User?)null);

        var act = async () => await _sut.ResetPasswordAsync(new ResetPasswordDto("some-token", "NewPassword123"));

        await act.Should().ThrowAsync<BusinessException>();
    }

    [Test]
    public async Task ResetPasswordAsync_WhenTokenValid_UpdatesPasswordAndInvalidatesTokens()
    {
        var now = DateTime.UtcNow;
        var user = MakeUser(Guid.NewGuid(), "known@example.com", now);
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = "hash",
            ExpiresAt = now.AddMinutes(30)
        };
        _passwordResetTokenRepositoryMock.Setup(r => r.GetValidByTokenHashAsync(It.IsAny<string>())).ReturnsAsync(token);
        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _userRepositoryMock.Setup(r => r.UpdateAsync(user)).Returns(Task.CompletedTask);
        _userRepositoryMock.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        await _sut.ResetPasswordAsync(new ResetPasswordDto("some-token", "NewPassword123"));

        BCrypt.Net.BCrypt.Verify("NewPassword123", user.PasswordHash).Should().BeTrue();
        _userRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        _userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _passwordResetTokenRepositoryMock.Verify(r => r.InvalidateAllForUserAsync(user.Id), Times.Once);
    }
}
