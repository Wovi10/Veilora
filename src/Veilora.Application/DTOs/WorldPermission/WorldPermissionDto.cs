namespace Veilora.Application.DTOs.WorldPermission;

public record WorldPermissionDto(Guid Id, Guid WorldId, Guid UserId, string? Username, bool CanEdit);
