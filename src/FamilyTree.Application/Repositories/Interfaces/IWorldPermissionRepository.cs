using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IWorldPermissionRepository
{
    Task<IEnumerable<WorldPermission>> GetByWorldAsync(Guid worldId);
    Task<WorldPermission?> GetAsync(Guid worldId, Guid userId);
    Task AddAsync(WorldPermission permission);
    Task DeleteAsync(WorldPermission permission);
    Task<int> SaveChangesAsync();
}
