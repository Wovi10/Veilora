using FamilyTree.Application.DTOs.Character;
using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Services;

public class CharacterService(
    ICharacterRepository characterRepository,
    IEntityRepository entityRepository,
    IWorldRepository worldRepository,
    IRelationshipRepository relationshipRepository) : ICharacterService
{
    public async Task<CharacterDto?> GetByIdAsync(Guid id)
    {
        var character = await characterRepository.GetByIdWithDetailsAsync(id);
        if (character is null) return null;
        var dto = CharacterMapper.ToDto(character);
        return await EnrichWithRelationalDataAsync(dto, id);
    }

    public async Task<IEnumerable<CharacterDto>> GetByWorldIdAsync(Guid worldId)
    {
        var characters = await characterRepository.GetByWorldIdAsync(worldId);
        return characters.Select(CharacterMapper.ToDto);
    }

    public async Task<IEnumerable<CharacterDto>> SearchAsync(string searchTerm)
    {
        var characters = await characterRepository.SearchAsync(searchTerm);
        return characters.Select(CharacterMapper.ToDto);
    }

    public async Task<IEnumerable<CharacterDto>> GetAncestorsAsync(Guid characterId)
    {
        var ancestors = await characterRepository.GetAncestorsAsync(characterId);
        return ancestors.Select(CharacterMapper.ToDto);
    }

    public async Task<IEnumerable<CharacterDto>> GetDescendantsAsync(Guid characterId)
    {
        var descendants = await characterRepository.GetDescendantsAsync(characterId);
        return descendants.Select(CharacterMapper.ToDto);
    }

    public async Task<IEnumerable<CharacterDto>> GetByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var characters = await characterRepository.GetByFamilyTreeIdAsync(familyTreeId);
        return characters.Select(CharacterMapper.ToDto);
    }

    public async Task<CharacterDto> CreateAsync(CreateCharacterDto dto)
    {
        _ = await worldRepository.GetByIdAsync(dto.WorldId)
            ?? throw new NotFoundException(nameof(Character), dto.WorldId);
        await ValidateParentsAreCharactersAsync(dto.Parent1Id, dto.Parent2Id);
        var character = CharacterMapper.ToEntity(dto);
        await characterRepository.AddAsync(character);
        await characterRepository.SaveChangesAsync();
        return CharacterMapper.ToDto(character);
    }

    public async Task<CharacterDto> UpdateAsync(Guid id, UpdateCharacterDto dto)
    {
        var character = await characterRepository.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Character), id);

        await ValidateParentsAreCharactersAsync(dto.Parent1Id, dto.Parent2Id);
        await ValidatePlaceReferencesAsync(dto.BirthPlaceEntityId, dto.DeathPlaceEntityId);
        await ValidateLocationIdsArePlacesAsync(dto.LocationIds);
        await ValidateAffiliationIdsAreGroupsAsync(dto.AffiliationIds);

        CharacterMapper.UpdateCharacter(dto, character);

        character.Locations.Clear();
        foreach (var placeId in dto.LocationIds)
            character.Locations.Add(new EntityLocation { CharacterId = character.Id, PlaceId = placeId });

        character.Affiliations.Clear();
        foreach (var groupId in dto.AffiliationIds)
            character.Affiliations.Add(new EntityAffiliation { CharacterId = character.Id, GroupId = groupId });

        character.Languages.Clear();
        foreach (var languageId in dto.LanguageIds)
            character.Languages.Add(new EntityLanguage { CharacterId = character.Id, LanguageId = languageId });

        await characterRepository.UpdateAsync(character);
        await characterRepository.SaveChangesAsync();

        await SyncSpousesAsync(character.Id, dto.SpouseIds);
        await SyncChildrenAsync(character.Id, dto.ChildIds);

        var updated = await characterRepository.GetByIdWithDetailsAsync(id)!;
        var updatedDto = CharacterMapper.ToDto(updated!);
        return await EnrichWithRelationalDataAsync(updatedDto, id);
    }

    public async Task DeleteAsync(Guid id)
    {
        var character = await characterRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Character), id);
        await characterRepository.DeleteAsync(character);
        await characterRepository.SaveChangesAsync();
    }

    // --- Helpers ---

    private async Task<CharacterDto> EnrichWithRelationalDataAsync(CharacterDto dto, Guid id)
    {
        var rels = await relationshipRepository.GetEntityRelationshipsAsync(id);
        var spouseRels = rels.Where(r => r.RelationshipType == RelationshipType.Spouse).ToList();
        var spouseIds = spouseRels
            .Select(r => r.Character1Id == id ? r.Character2Id : r.Character1Id)
            .ToList();

        var spouses = new List<EntityRefDto>();
        foreach (var spouseId in spouseIds)
        {
            var spouse = await characterRepository.GetByIdAsync(spouseId);
            if (spouse is not null) spouses.Add(new EntityRefDto(spouse.Id, spouse.Name));
        }

        var childCharacters = await characterRepository.GetChildrenAsync(id);
        var children = childCharacters.Select(c => new EntityRefDto(c.Id, c.Name)).ToList();

        return dto with { Spouses = spouses, Children = children };
    }

    private async Task SyncSpousesAsync(Guid characterId, IReadOnlyList<Guid> desiredSpouseIds)
    {
        var rels = await relationshipRepository.GetEntityRelationshipsAsync(characterId);
        var currentSpouseRels = rels.Where(r => r.RelationshipType == RelationshipType.Spouse).ToList();
        var currentSpouseIds = currentSpouseRels
            .Select(r => r.Character1Id == characterId ? r.Character2Id : r.Character1Id)
            .ToHashSet();
        var desiredSet = desiredSpouseIds.ToHashSet();

        foreach (var rel in currentSpouseRels)
        {
            var otherId = rel.Character1Id == characterId ? rel.Character2Id : rel.Character1Id;
            if (!desiredSet.Contains(otherId))
                await relationshipRepository.DeleteAsync(rel);
        }

        foreach (var spouseId in desiredSet.Where(sid => !currentSpouseIds.Contains(sid)))
        {
            await relationshipRepository.AddAsync(new Relationship
            {
                Character1Id = characterId,
                Character2Id = spouseId,
                RelationshipType = RelationshipType.Spouse
            });
        }

        await relationshipRepository.SaveChangesAsync();
    }

    private async Task SyncChildrenAsync(Guid characterId, IReadOnlyList<Guid> desiredChildIds)
    {
        var currentChildren = (await characterRepository.GetChildrenAsync(characterId)).ToList();
        var currentChildIds = currentChildren.Select(c => c.Id).ToHashSet();
        var desiredSet = desiredChildIds.ToHashSet();

        foreach (var child in currentChildren.Where(c => !desiredSet.Contains(c.Id)))
        {
            var trackedChild = await characterRepository.GetByIdWithDetailsAsync(child.Id);
            if (trackedChild is null) continue;
            if (trackedChild.Parent1Id == characterId) trackedChild.Parent1Id = null;
            else if (trackedChild.Parent2Id == characterId) trackedChild.Parent2Id = null;
            await characterRepository.UpdateAsync(trackedChild);
            await characterRepository.SaveChangesAsync();
        }

        foreach (var childId in desiredSet.Where(cid => !currentChildIds.Contains(cid)))
        {
            var trackedChild = await characterRepository.GetByIdWithDetailsAsync(childId)
                ?? throw new NotFoundException(nameof(Character), childId);

            if (trackedChild.Parent1Id is null)
                trackedChild.Parent1Id = characterId;
            else if (trackedChild.Parent2Id is null)
                trackedChild.Parent2Id = characterId;
            else
                throw new ValidationException("ChildIds", $"Character '{trackedChild.Name}' already has two parents assigned.");

            await characterRepository.UpdateAsync(trackedChild);
            await characterRepository.SaveChangesAsync();
        }
    }

    private async Task ValidateParentsAreCharactersAsync(Guid? parent1Id, Guid? parent2Id)
    {
        if (parent1Id is not null)
        {
            _ = await characterRepository.GetByIdAsync(parent1Id.Value)
                ?? throw new NotFoundException(nameof(Character), parent1Id.Value);
        }
        if (parent2Id is not null)
        {
            _ = await characterRepository.GetByIdAsync(parent2Id.Value)
                ?? throw new NotFoundException(nameof(Character), parent2Id.Value);
        }
    }

    private async Task ValidatePlaceReferencesAsync(Guid? birthPlaceId, Guid? deathPlaceId)
    {
        if (birthPlaceId is not null)
        {
            var place = await entityRepository.GetByIdAsync(birthPlaceId.Value)
                ?? throw new NotFoundException(nameof(Entity), birthPlaceId.Value);
            if (place.Type != EntityType.Place)
                throw new ValidationException("BirthPlaceEntityId", "Birth place must be a Place entity.");
        }
        if (deathPlaceId is not null)
        {
            var place = await entityRepository.GetByIdAsync(deathPlaceId.Value)
                ?? throw new NotFoundException(nameof(Entity), deathPlaceId.Value);
            if (place.Type != EntityType.Place)
                throw new ValidationException("DeathPlaceEntityId", "Death place must be a Place entity.");
        }
    }

    private async Task ValidateLocationIdsArePlacesAsync(IReadOnlyList<Guid> ids)
    {
        foreach (var id in ids)
        {
            var entity = await entityRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Entity), id);
            if (entity.Type != EntityType.Place)
                throw new ValidationException("LocationIds", $"Entity '{entity.Name}' is not a Place.");
        }
    }

    private async Task ValidateAffiliationIdsAreGroupsAsync(IReadOnlyList<Guid> ids)
    {
        foreach (var id in ids)
        {
            var entity = await entityRepository.GetByIdAsync(id)
                ?? throw new NotFoundException(nameof(Entity), id);
            if (entity.Type != EntityType.Group)
                throw new ValidationException("AffiliationIds", $"Entity '{entity.Name}' is not a Group.");
        }
    }
}
