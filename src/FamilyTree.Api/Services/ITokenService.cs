using FamilyTree.Application.DTOs.Auth;

namespace FamilyTree.Api.Services;

public interface ITokenService
{
    string GenerateToken(UserInfoDto user);
}
