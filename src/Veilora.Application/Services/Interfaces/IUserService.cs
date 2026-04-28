using Veilora.Application.DTOs.Auth;
using Veilora.Application.DTOs.User;

namespace Veilora.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserMeDto> GetMeAsync(Guid userId);
    Task<UserInfoDto> UpdateDisplayNameAsync(Guid userId, UpdateDisplayNameDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
    Task<UserMeDto> SetBackupUserAsync(Guid userId, string backupEmail);
    Task<UserMeDto> RemoveBackupUserAsync(Guid userId);
    Task DeleteAccountAsync(Guid userId);
}
