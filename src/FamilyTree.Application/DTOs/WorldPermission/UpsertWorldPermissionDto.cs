namespace FamilyTree.Application.DTOs.WorldPermission;

public record UpsertWorldPermissionDto(Guid UserId, bool CanEdit);
