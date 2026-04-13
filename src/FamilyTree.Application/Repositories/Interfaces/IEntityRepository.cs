using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IEntityRepository : IRepository<Entity>
{
    Task<IEnumerable<Entity>> SearchAsync(string searchTerm);
    Task<IEnumerable<Entity>> GetByWorldIdAsync(Guid worldId);
    Task<IEnumerable<Entity>> GetByWorldIdAndTypeAsync(Guid worldId, string type);
    Task<IEnumerable<Entity>> GetEntitiesByFamilyTreeIdAsync(Guid familyTreeId);
    Task<IEnumerable<Entity>> GetAncestorsAsync(Guid entityId);
    Task<IEnumerable<Entity>> GetDescendantsAsync(Guid entityId);
}
