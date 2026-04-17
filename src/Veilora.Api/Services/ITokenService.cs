using Veilora.Application.DTOs.Auth;

namespace Veilora.Api.Services;

public interface ITokenService
{
    string GenerateToken(UserInfoDto user);
}
