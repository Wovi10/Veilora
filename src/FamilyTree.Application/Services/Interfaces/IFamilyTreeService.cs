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
    Task AddEntityToFamilyTreeAsync(Guid familyTreeId, Guid entityId);
    Task RemoveEntityFromFamilyTreeAsync(Guid familyTreeId, Guid entityId);
    Task UpdateEntityPositionAsync(Guid familyTreeId, Guid entityId, double x, double y);
}
