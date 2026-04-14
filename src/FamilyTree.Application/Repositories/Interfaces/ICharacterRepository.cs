using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<Character?> GetByIdWithDetailsAsync(Guid id);
    Task<IEnumerable<Character>> GetByWorldIdAsync(Guid worldId);
    Task<IEnumerable<Character>> SearchAsync(string searchTerm);
    Task<IEnumerable<Character>> GetAncestorsAsync(Guid characterId);
    Task<IEnumerable<Character>> GetDescendantsAsync(Guid characterId);
    Task<IEnumerable<Character>> GetChildrenAsync(Guid characterId);
    Task<IEnumerable<Character>> GetByFamilyTreeIdAsync(Guid familyTreeId);
}
