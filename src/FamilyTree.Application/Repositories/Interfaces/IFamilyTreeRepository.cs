using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IFamilyTreeRepository : IRepository<FamilyTreeEntity>
{
    Task<FamilyTreeEntity?> GetFamilyTreeWithEntitiesAsync(Guid familyTreeId);
    Task<IEnumerable<FamilyTreeEntity>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<FamilyTreeEntity>> GetPagedAsync(FamilyTreeCriteria criteria);
    Task AddCharacterToFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task RemoveCharacterFromFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task<bool> IsCharacterInFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task UpdateCharacterPositionAsync(Guid familyTreeId, Guid characterId, double x, double y);
}
