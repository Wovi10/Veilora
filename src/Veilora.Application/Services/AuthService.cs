using Veilora.Application.DTOs.Auth;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public class AuthService(IUserRepository userRepository) : IAuthService
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
}
