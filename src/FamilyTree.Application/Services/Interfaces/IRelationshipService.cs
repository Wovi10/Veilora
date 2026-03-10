using FamilyTree.Application.DTOs.Relationship;

namespace FamilyTree.Application.Services.Interfaces;

public interface IRelationshipService
{
    Task<IEnumerable<RelationshipDto>> GetAllAsync();
    Task<RelationshipDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<RelationshipDto>> GetRelationshipsByTreeIdAsync(Guid treeId);
    Task<RelationshipDto> CreateAsync(CreateRelationshipDto dto);
    Task<RelationshipDto> UpdateAsync(Guid id, UpdateRelationshipDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<RelationshipDto>> GetPersonRelationshipsAsync(Guid personId);
}

