using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<Character?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Character>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<Character>> GetPagedAsync(CharacterCriteria criteria);
    Task<IEnumerable<Character>> SearchAsync(string searchTerm);
    Task<IEnumerable<Character>> GetAncestorsAsync(Guid characterId);
    Task<IEnumerable<Character>> GetDescendantsAsync(Guid characterId);
    Task<IEnumerable<Character>> GetChildrenAsync(Guid characterId);
    Task<IEnumerable<Character>> GetByFamilyTreeIdAsync(Guid familyTreeId);
}
