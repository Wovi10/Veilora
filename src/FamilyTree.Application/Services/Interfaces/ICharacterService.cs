using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTree.Application.DTOs.Character;

namespace FamilyTree.Application.Services.Interfaces;

public interface ICharacterService
{
    Task<CharacterDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<CharacterDto>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<CharacterDto>> GetPagedAsync(CharacterCriteria criteria);
    Task<IEnumerable<CharacterDto>> SearchAsync(string searchTerm);
    Task<IEnumerable<CharacterDto>> GetAncestorsAsync(Guid characterId);
    Task<IEnumerable<CharacterDto>> GetDescendantsAsync(Guid characterId);
    Task<IEnumerable<CharacterDto>> GetByFamilyTreeIdAsync(Guid familyTreeId);
    Task<CharacterDto> CreateAsync(CreateCharacterDto dto);
    Task<CharacterDto> UpdateAsync(Guid id, UpdateCharacterDto dto);
    Task DeleteAsync(Guid id);
}
