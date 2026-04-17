using Veilora.Application.DTOs.Auth;

namespace Veilora.Application.Services.Interfaces;

public interface IAuthService
{
    Task<UserInfoDto> RegisterAsync(RegisterDto dto);
    Task<UserInfoDto?> ValidateAsync(LoginDto dto);
}
