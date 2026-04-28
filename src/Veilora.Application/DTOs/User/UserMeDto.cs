namespace Veilora.Application.DTOs.User;

public record UserMeDto(
    Guid Id,
    string Email,
    string? DisplayName,
    string? BackupUserEmail,
    string? BackupUserDisplayName);
