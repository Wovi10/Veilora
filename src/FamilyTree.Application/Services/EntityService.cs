using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Services;

public class EntityService(IEntityRepository entityRepository, IWorldRepository worldRepository) : IEntityService
{
    public async Task<IEnumerable<EntityDto>> GetAllAsync()
    {
        var entities = await entityRepository.GetAllAsync();
        return entities.Select(EntityMapper.ToDto);
    }

    public async Task<EntityDto?> GetByIdAsync(Guid id)
    {
        var entity = await entityRepository.GetByIdAsync(id);
        return entity is null ? null : EntityMapper.ToDto(entity);
    }

    public async Task<IEnumerable<EntityDto>> GetByWorldIdAsync(Guid worldId)
    {
        var entities = await entityRepository.GetByWorldIdAsync(worldId);
        return entities.Select(EntityMapper.ToDto);
    }

    public async Task<IEnumerable<EntityDto>> GetByWorldIdAndTypeAsync(Guid worldId, string type)
    {
        var entities = await entityRepository.GetByWorldIdAndTypeAsync(worldId, type);
        return entities.Select(EntityMapper.ToDto);
    }

    public async Task<IEnumerable<EntityDto>> GetEntitiesByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var entities = await entityRepository.GetEntitiesByFamilyTreeIdAsync(familyTreeId);
        return entities.Select(EntityMapper.ToDto);
    }

    public async Task<EntityDto> CreateAsync(CreateEntityDto dto)
    {
        _ = await worldRepository.GetByIdAsync(dto.WorldId)
            ?? throw new NotFoundException(nameof(World), dto.WorldId);
        var entity = EntityMapper.ToEntity(dto);
        await entityRepository.AddAsync(entity);
        await entityRepository.SaveChangesAsync();
        return EntityMapper.ToDto(entity);
    }

    public async Task<EntityDto> UpdateAsync(Guid id, UpdateEntityDto dto)
    {
        var entity = await entityRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Entity), id);
        EntityMapper.UpdateEntity(dto, entity);
        await entityRepository.UpdateAsync(entity);
        await entityRepository.SaveChangesAsync();
        return EntityMapper.ToDto(entity);
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await entityRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Entity), id);
        await entityRepository.DeleteAsync(entity);
        await entityRepository.SaveChangesAsync();
    }

    public async Task<IEnumerable<EntityDto>> SearchAsync(string searchTerm)
    {
        var entities = await entityRepository.SearchAsync(searchTerm);
        return entities.Select(EntityMapper.ToDto);
    }

    public async Task<IEnumerable<EntityDto>> GetAncestorsAsync(Guid entityId)
    {
        var ancestors = await entityRepository.GetAncestorsAsync(entityId);
        return ancestors.Select(EntityMapper.ToDto);
    }

    public async Task<IEnumerable<EntityDto>> GetDescendantsAsync(Guid entityId)
    {
        var descendants = await entityRepository.GetDescendantsAsync(entityId);
        return descendants.Select(EntityMapper.ToDto);
    }
}
