using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IFamilyTreeRepository : IRepository<FamilyTreeEntity>
{
    Task<FamilyTreeEntity?> GetFamilyTreeWithEntitiesAsync(Guid familyTreeId);
    Task<IEnumerable<FamilyTreeEntity>> GetByWorldIdAsync(Guid worldId);
    Task AddCharacterToFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task RemoveCharacterFromFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task<bool> IsCharacterInFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task UpdateCharacterPositionAsync(Guid familyTreeId, Guid characterId, double x, double y);
}
