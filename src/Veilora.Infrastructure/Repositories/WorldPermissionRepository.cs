using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class WorldPermissionRepository(ApplicationDbContext context) : IWorldPermissionRepository
{
    public async Task<IEnumerable<WorldPermission>> GetByWorldAsync(Guid worldId) =>
        await context.WorldPermissions
            .AsNoTracking()
            .Include(wp => wp.User)
            .Where(wp => wp.WorldId == worldId)
            .ToListAsync();

    public async Task<WorldPermission?> GetAsync(Guid worldId, Guid userId) =>
        await context.WorldPermissions
            .FirstOrDefaultAsync(wp => wp.WorldId == worldId && wp.UserId == userId);

    public async Task AddAsync(WorldPermission permission) =>
        await context.WorldPermissions.AddAsync(permission);

    public Task DeleteAsync(WorldPermission permission)
    {
        context.WorldPermissions.Remove(permission);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync() =>
        await context.SaveChangesAsync();
}
