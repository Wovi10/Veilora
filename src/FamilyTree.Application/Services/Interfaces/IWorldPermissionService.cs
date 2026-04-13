using FamilyTree.Application.DTOs.WorldPermission;

namespace FamilyTree.Application.Services.Interfaces;

public interface IWorldPermissionService
{
    Task<IEnumerable<WorldPermissionDto>> GetByWorldAsync(Guid worldId);
    Task<WorldPermissionDto> UpsertAsync(Guid worldId, UpsertWorldPermissionDto dto);
    Task DeleteAsync(Guid worldId, Guid userId);
}
