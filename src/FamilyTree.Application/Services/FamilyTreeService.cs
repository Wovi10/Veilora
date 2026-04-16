using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTree.Application.DTOs.FamilyTree;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Application.Services;

public class FamilyTreeService(
    IFamilyTreeRepository familyTreeRepository,
    ICharacterRepository characterRepository) : IFamilyTreeService
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

    public async Task<PagedResult<FamilyTreeDto>> GetPagedAsync(FamilyTreeCriteria criteria)
    {
        var paged = await familyTreeRepository.GetPagedAsync(criteria);
        return new PagedResult<FamilyTreeDto>(
            paged.Items.Select(FamilyTreeMapper.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
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

    public async Task AddCharacterToFamilyTreeAsync(Guid familyTreeId, Guid characterId)
    {
        _ = await familyTreeRepository.GetByIdAsync(familyTreeId)
            ?? throw new NotFoundException(nameof(FamilyTreeEntity), familyTreeId);
        _ = await characterRepository.GetByIdAsync(characterId)
            ?? throw new NotFoundException(nameof(Character), characterId);
        if (await familyTreeRepository.IsCharacterInFamilyTreeAsync(familyTreeId, characterId))
            throw new BusinessException("Character is already in this family tree.");
        await familyTreeRepository.AddCharacterToFamilyTreeAsync(familyTreeId, characterId);
        await familyTreeRepository.SaveChangesAsync();
    }

    public async Task RemoveCharacterFromFamilyTreeAsync(Guid familyTreeId, Guid characterId)
    {
        if (!await familyTreeRepository.IsCharacterInFamilyTreeAsync(familyTreeId, characterId))
            throw new NotFoundException("CharacterFamilyTree", $"{familyTreeId}/{characterId}");
        await familyTreeRepository.RemoveCharacterFromFamilyTreeAsync(familyTreeId, characterId);
        await familyTreeRepository.SaveChangesAsync();
    }

    public async Task UpdateCharacterPositionAsync(Guid familyTreeId, Guid characterId, double x, double y)
    {
        if (!await familyTreeRepository.IsCharacterInFamilyTreeAsync(familyTreeId, characterId))
            throw new NotFoundException("CharacterFamilyTree", $"{familyTreeId}/{characterId}");
        await familyTreeRepository.UpdateCharacterPositionAsync(familyTreeId, characterId, x, y);
    }
}
