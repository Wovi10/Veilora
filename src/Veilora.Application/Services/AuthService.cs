using System.Security.Cryptography;
using System.Text;
using Veilora.Application.DTOs.Auth;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Application.Settings;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IEmailSender emailSender,
    PasswordResetSettings passwordResetSettings) : IAuthService
{
    public async Task<UserInfoDto> RegisterAsync(RegisterDto dto)
    {
        if (await userRepository.GetByEmailAsync(dto.Email.ToLowerInvariant()) is not null)
            throw new InvalidOperationException("Email is already registered.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = dto.Email.ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            DisplayName = dto.DisplayName
        };

        await userRepository.AddAsync(user);
        await userRepository.SaveChangesAsync();

        return new UserInfoDto(user.Id, user.Email, user.DisplayName);
    }

    public async Task<UserInfoDto?> ValidateAsync(LoginDto dto)
    {
        var input = dto.UsernameOrEmail;
        var user = input.Contains('@')
            ? await userRepository.GetByEmailAsync(input.ToLowerInvariant())
            : await userRepository.GetByDisplayNameAsync(input);

        if (user is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

        return new UserInfoDto(user.Id, user.Email, user.DisplayName);
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        var user = await userRepository.GetByEmailAsync(dto.Email.ToLowerInvariant());
        if (user is null) return;

        var rawToken = GenerateToken();

        await passwordResetTokenRepository.InvalidateAllForUserAsync(user.Id);
        await passwordResetTokenRepository.AddAsync(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            TokenHash = Hash(rawToken),
            ExpiresAt = DateTime.UtcNow.AddMinutes(passwordResetSettings.TokenExpiryMinutes)
        });
        await passwordResetTokenRepository.SaveChangesAsync();

        var resetLink = $"{passwordResetSettings.ClientBaseUrl.TrimEnd('/')}/reset-password?token={rawToken}";
        await emailSender.SendPasswordResetEmailAsync(user.Email, resetLink, passwordResetSettings.TokenExpiryMinutes);
    }

    public async Task ResetPasswordAsync(ResetPasswordDto dto)
    {
        var token = await passwordResetTokenRepository.GetValidByTokenHashAsync(Hash(dto.Token))
            ?? throw new BusinessException("Invalid or expired reset link.");

        var user = await userRepository.GetByIdAsync(token.UserId)
            ?? throw new BusinessException("Invalid or expired reset link.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();

        await passwordResetTokenRepository.InvalidateAllForUserAsync(user.Id);
    }

    private static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
