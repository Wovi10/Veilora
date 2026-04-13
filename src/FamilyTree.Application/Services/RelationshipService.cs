using FamilyTree.Application.DTOs.Relationship;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Services;

public class RelationshipService(IRelationshipRepository relationshipRepository, IEntityRepository entityRepository) : IRelationshipService
{
    public async Task<IEnumerable<RelationshipDto>> GetAllAsync()
    {
        var relationships = await relationshipRepository.GetAllAsync();
        return relationships.Select(RelationshipMapper.ToDto);
    }

    public async Task<RelationshipDto?> GetByIdAsync(Guid id)
    {
        var relationship = await relationshipRepository.GetByIdAsync(id);
        return relationship is null ? null : RelationshipMapper.ToDto(relationship);
    }

    public async Task<IEnumerable<RelationshipDto>> GetRelationshipsByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var relationships = await relationshipRepository.GetRelationshipsByFamilyTreeIdAsync(familyTreeId);
        return relationships.Select(RelationshipMapper.ToDto);
    }

    public async Task<RelationshipDto> CreateAsync(CreateRelationshipDto dto)
    {
        _ = await entityRepository.GetByIdAsync(dto.Entity1Id)
            ?? throw new NotFoundException(nameof(Entity), dto.Entity1Id);
        _ = await entityRepository.GetByIdAsync(dto.Entity2Id)
            ?? throw new NotFoundException(nameof(Entity), dto.Entity2Id);
        var relationship = RelationshipMapper.ToEntity(dto);
        await relationshipRepository.AddAsync(relationship);
        await relationshipRepository.SaveChangesAsync();
        return RelationshipMapper.ToDto(relationship);
    }

    public async Task<RelationshipDto> UpdateAsync(Guid id, UpdateRelationshipDto dto)
    {
        var relationship = await relationshipRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Relationship), id);
        _ = await entityRepository.GetByIdAsync(dto.Entity1Id)
            ?? throw new NotFoundException(nameof(Entity), dto.Entity1Id);
        _ = await entityRepository.GetByIdAsync(dto.Entity2Id)
            ?? throw new NotFoundException(nameof(Entity), dto.Entity2Id);
        RelationshipMapper.UpdateEntity(dto, relationship);
        await relationshipRepository.UpdateAsync(relationship);
        await relationshipRepository.SaveChangesAsync();
        return RelationshipMapper.ToDto(relationship);
    }

    public async Task DeleteAsync(Guid id)
    {
        var relationship = await relationshipRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Relationship), id);
        await relationshipRepository.DeleteAsync(relationship);
        await relationshipRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<RelationshipDto>> GetEntityRelationshipsAsync(Guid entityId)
    {
        var relationships = await relationshipRepository.GetEntityRelationshipsAsync(entityId);
        return relationships.Select(RelationshipMapper.ToDto);
    }
}
