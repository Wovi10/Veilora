using Veilora.Application.DTOs.WorldPermission;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public class WorldPermissionService(IWorldPermissionRepository repository) : IWorldPermissionService
{
    public async Task<IEnumerable<WorldPermissionDto>> GetByWorldAsync(Guid worldId)
    {
        var permissions = await repository.GetByWorldAsync(worldId);
        return permissions.Select(ToDto);
    }

    public async Task<WorldPermissionDto> UpsertAsync(Guid worldId, UpsertWorldPermissionDto dto)
    {
        var existing = await repository.GetAsync(worldId, dto.UserId);
        if (existing is not null)
        {
            existing.CanEdit = dto.CanEdit;
            await repository.SaveChangesAsync();
            return ToDto(existing);
        }

        var permission = new WorldPermission
        {
            WorldId = worldId,
            UserId = dto.UserId,
            CanEdit = dto.CanEdit,
        };
        await repository.AddAsync(permission);
        await repository.SaveChangesAsync();
        return ToDto(permission);
    }

    public async Task DeleteAsync(Guid worldId, Guid userId)
    {
        var permission = await repository.GetAsync(worldId, userId);
        if (permission is null) return;
        await repository.DeleteAsync(permission);
        await repository.SaveChangesAsync();
    }

    private static WorldPermissionDto ToDto(WorldPermission wp) =>
        new(wp.Id, wp.WorldId, wp.UserId, wp.User?.DisplayName ?? wp.User?.Email, wp.CanEdit);
}
