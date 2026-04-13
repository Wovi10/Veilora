using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IFamilyTreeRepository : IRepository<FamilyTreeEntity>
{
    Task<FamilyTreeEntity?> GetFamilyTreeWithEntitiesAsync(Guid familyTreeId);
    Task<IEnumerable<FamilyTreeEntity>> GetByWorldIdAsync(Guid worldId);
    Task AddEntityToFamilyTreeAsync(Guid familyTreeId, Guid entityId);
    Task RemoveEntityFromFamilyTreeAsync(Guid familyTreeId, Guid entityId);
    Task<bool> IsEntityInFamilyTreeAsync(Guid familyTreeId, Guid entityId);
    Task UpdateEntityPositionAsync(Guid familyTreeId, Guid entityId, double x, double y);
}
