using FamilyTree.Application.DTOs.Auth;

namespace FamilyTree.Application.Services.Interfaces;

public interface IAuthService
{
    Task<UserInfoDto> RegisterAsync(RegisterDto dto);
    Task<UserInfoDto?> ValidateAsync(LoginDto dto);
}
