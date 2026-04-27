using Veilora.Application.DTOs.Auth;
using Veilora.Application.DTOs.User;

namespace Veilora.Application.Services.Interfaces;

public interface IUserService
{
    Task<UserInfoDto> UpdateDisplayNameAsync(Guid userId, UpdateDisplayNameDto dto);
    Task ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}
