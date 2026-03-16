using FamilyTree.Application.DTOs.Auth;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Services;

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
        var user = await userRepository.GetByEmailAsync(dto.Email.ToLowerInvariant());
        if (user is null) return null;
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

        return new UserInfoDto(user.Id, user.Email, user.DisplayName);
    }
}
