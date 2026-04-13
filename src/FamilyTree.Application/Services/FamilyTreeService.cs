using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Application.Services;

public class FamilyTreeService(IFamilyTreeRepository familyTreeRepository, IEntityRepository entityRepository) : IFamilyTreeService
{
    public async Task<IEnumerable<FamilyTreeDto>> GetAllAsync()
    {
        var trees = await familyTreeRepository.GetAllAsync();
        return trees.Select(FamilyTreeMapper.ToDto);
    }

    public async Task<IEnumerable<FamilyTreeDto>> GetByWorldIdAsync(Guid worldId)
    {
        var trees = await familyTreeRepository.GetByWorldIdAsync(worldId);
        return trees.Select(FamilyTreeMapper.ToDto);
    }

    public async Task<FamilyTreeDto?> GetByIdAsync(Guid id)
    {
        var tree = await familyTreeRepository.GetByIdAsync(id);
        return tree is null ? null : FamilyTreeMapper.ToDto(tree);
    }

    public async Task<FamilyTreeWithEntitiesDto?> GetFamilyTreeWithEntitiesAsync(Guid id)
    {
        var tree = await familyTreeRepository.GetFamilyTreeWithEntitiesAsync(id);
        return tree is null ? null : FamilyTreeMapper.ToWithEntitiesDto(tree);
    }

    public async Task<FamilyTreeDto> CreateAsync(CreateFamilyTreeDto dto)
    {
        var tree = FamilyTreeMapper.ToEntity(dto);
        await familyTreeRepository.AddAsync(tree);
        await familyTreeRepository.SaveChangesAsync();
        return FamilyTreeMapper.ToDto(tree);
    }

    public async Task<FamilyTreeDto> UpdateAsync(Guid id, UpdateFamilyTreeDto dto)
    {
        var tree = await familyTreeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(FamilyTreeEntity), id);
        FamilyTreeMapper.UpdateEntity(dto, tree);
        await familyTreeRepository.UpdateAsync(tree);
        await familyTreeRepository.SaveChangesAsync();
        return FamilyTreeMapper.ToDto(tree);
    }

    public async Task DeleteAsync(Guid id)
    {
        var tree = await familyTreeRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(FamilyTreeEntity), id);
        await familyTreeRepository.DeleteAsync(tree);
        await familyTreeRepository.SaveChangesAsync();
    }

    public async Task AddEntityToFamilyTreeAsync(Guid familyTreeId, Guid entityId)
    {
        _ = await familyTreeRepository.GetByIdAsync(familyTreeId)
            ?? throw new NotFoundException(nameof(FamilyTreeEntity), familyTreeId);
        _ = await entityRepository.GetByIdAsync(entityId)
            ?? throw new NotFoundException(nameof(Entity), entityId);
        if (await familyTreeRepository.IsEntityInFamilyTreeAsync(familyTreeId, entityId))
            throw new BusinessException("Entity is already in this family tree.");
        await familyTreeRepository.AddEntityToFamilyTreeAsync(familyTreeId, entityId);
        await familyTreeRepository.SaveChangesAsync();
    }

    public async Task RemoveEntityFromFamilyTreeAsync(Guid familyTreeId, Guid entityId)
    {
        if (!await familyTreeRepository.IsEntityInFamilyTreeAsync(familyTreeId, entityId))
            throw new NotFoundException("EntityFamilyTree", $"{familyTreeId}/{entityId}");
        await familyTreeRepository.RemoveEntityFromFamilyTreeAsync(familyTreeId, entityId);
        await familyTreeRepository.SaveChangesAsync();
    }

    public async Task UpdateEntityPositionAsync(Guid familyTreeId, Guid entityId, double x, double y)
    {
        if (!await familyTreeRepository.IsEntityInFamilyTreeAsync(familyTreeId, entityId))
            throw new NotFoundException("EntityFamilyTree", $"{familyTreeId}/{entityId}");
        await familyTreeRepository.UpdateEntityPositionAsync(familyTreeId, entityId, x, y);
    }
}
