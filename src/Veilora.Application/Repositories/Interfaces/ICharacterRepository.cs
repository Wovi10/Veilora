using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<Character?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Character>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<Character>> GetPagedAsync(CharacterCriteria criteria);
    Task<IEnumerable<Character>> SearchAsync(string searchTerm);
    Task<IEnumerable<(Guid Id, string Name)>> SearchByWorldAsync(Guid worldId, string term, int limit);
    Task<IEnumerable<Character>> GetAncestorsAsync(Guid characterId);
    Task<IEnumerable<Character>> GetDescendantsAsync(Guid characterId);
    Task<IEnumerable<Character>> GetChildrenAsync(Guid characterId);
    Task<IEnumerable<Character>> GetByFamilyTreeIdAsync(Guid familyTreeId);
}
