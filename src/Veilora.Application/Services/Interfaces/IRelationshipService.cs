using Veilora.Application.DTOs.Relationship;

namespace Veilora.Application.Services.Interfaces;

public interface IRelationshipService
{
    Task<IEnumerable<RelationshipDto>> GetAllAsync();
    Task<RelationshipDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RelationshipDto>> GetRelationshipsByFamilyTreeIdAsync(Guid familyTreeId);
    Task<RelationshipDto> CreateAsync(CreateRelationshipDto dto);
    Task<RelationshipDto> UpdateAsync(Guid id, UpdateRelationshipDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<RelationshipDto>> GetEntityRelationshipsAsync(Guid entityId);
}
