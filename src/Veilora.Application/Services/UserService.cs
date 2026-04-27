using Veilora.Application.DTOs.Auth;
using Veilora.Application.DTOs.User;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;

namespace Veilora.Application.Services;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<UserInfoDto> UpdateDisplayNameAsync(Guid userId, UpdateDisplayNameDto dto)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        user.DisplayName = dto.DisplayName?.Trim() is { Length: > 0 } name ? name : null;
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();

        return new UserInfoDto(user.Id, user.Email, user.DisplayName);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            throw new BusinessException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();
    }
}
