namespace Veilora.Application.DTOs.Auth;

public record AuthResponseDto(Guid Id, string Token, string Email, string? DisplayName);
