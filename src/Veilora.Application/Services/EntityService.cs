using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Entity;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;

namespace Veilora.Application.Services;

public class EntityService(
    IEntityRepository entityRepository,
    IWorldRepository worldRepository) : IEntityService
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

    public async Task<PagedResult<EntityDto>> GetPagedAsync(EntityCriteria criteria)
    {
        var paged = await entityRepository.GetPagedAsync(criteria);
        return new PagedResult<EntityDto>(
            paged.Items.Select(EntityMapper.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
    }
}
