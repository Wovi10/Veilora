namespace FamilyTree.Application.DTOs.Auth;

public record AuthResponseDto(string Token, string Email, string? DisplayName);
