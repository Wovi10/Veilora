namespace Veilora.Application.DTOs.Auth;

public record UserInfoDto(Guid Id, string Email, string? DisplayName);
