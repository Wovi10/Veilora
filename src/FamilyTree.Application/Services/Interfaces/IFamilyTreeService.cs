using FamilyTree.Application.DTOs.FamilyTree;

namespace FamilyTree.Application.Services.Interfaces;

public interface IFamilyTreeService
{
    Task<IEnumerable<FamilyTreeDto>> GetAllAsync();
    Task<IEnumerable<FamilyTreeDto>> GetByWorldIdAsync(Guid worldId);
    Task<FamilyTreeDto?> GetByIdAsync(Guid id);
    Task<FamilyTreeWithEntitiesDto?> GetFamilyTreeWithEntitiesAsync(Guid id);
    Task<FamilyTreeDto> CreateAsync(CreateFamilyTreeDto dto);
    Task<FamilyTreeDto> UpdateAsync(Guid id, UpdateFamilyTreeDto dto);
    Task DeleteAsync(Guid id);
    Task AddCharacterToFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task RemoveCharacterFromFamilyTreeAsync(Guid familyTreeId, Guid characterId);
    Task UpdateCharacterPositionAsync(Guid familyTreeId, Guid characterId, double x, double y);
}
