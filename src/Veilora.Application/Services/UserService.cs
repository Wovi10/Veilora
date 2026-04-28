using Veilora.Application.DTOs.Auth;
using Veilora.Application.DTOs.User;
using Veilora.Application.Exceptions;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;

namespace Veilora.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IWorldRepository worldRepository,
    IFamilyTreeRepository familyTreeRepository) : IUserService
{
    public async Task<UserMeDto> GetMeAsync(Guid userId)
    {
        var user = await userRepository.GetWithBackupAsync(userId)
            ?? throw new NotFoundException("User not found.");
        return ToMeDto(user);
    }

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

    public async Task<UserMeDto> SetBackupUserAsync(Guid userId, string backupEmail)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        var backup = await userRepository.GetByEmailAsync(backupEmail.ToLowerInvariant())
            ?? throw new NotFoundException("No user found with that email address.");

        if (backup.Id == userId)
            throw new BusinessException("You cannot set yourself as your backup user.");

        user.BackupUserId = backup.Id;
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();

        user.BackupUser = backup;
        return ToMeDto(user);
    }

    public async Task<UserMeDto> RemoveBackupUserAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        user.BackupUserId = null;
        await userRepository.UpdateAsync(user);
        await userRepository.SaveChangesAsync();

        return ToMeDto(user);
    }

    public async Task DeleteAccountAsync(Guid userId)
    {
        var user = await userRepository.GetByIdAsync(userId)
            ?? throw new NotFoundException("User not found.");

        if (user.BackupUserId is { } backupId)
        {
            await worldRepository.TransferOwnershipAsync(userId, backupId);
            await familyTreeRepository.TransferOwnershipAsync(userId, backupId);
        }

        await userRepository.DeleteAsync(user);
        await userRepository.SaveChangesAsync();
    }

    private static UserMeDto ToMeDto(Domain.Entities.User user) => new(
        user.Id,
        user.Email,
        user.DisplayName,
        user.BackupUser?.Email,
        user.BackupUser?.DisplayName);
}
