using Veilora.Application.Common;
using Veilora.Application.Criteria;
using FamilyTreeEntity = Veilora.Domain.Entities.FamilyTree;

namespace Veilora.Application.Repositories.Interfaces;

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
