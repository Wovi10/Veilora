using FamilyTree.Application.DTOs.Entity;
using FamilyTree.Application.DTOs.Language;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Application.Services;

public class EntityService(
    IEntityRepository entityRepository,
    IWorldRepository worldRepository,
    IRelationshipRepository relationshipRepository) : IEntityService
{
    public async Task<IEnumerable<EntityDto>> GetAllAsync()
    {
        var entities = await entityRepository.GetAllAsync();
        return entities.Select(EntityMapper.ToDto);
    }

    public async Task<EntityDto?> GetByIdAsync(Guid id)
    {
        var entity = await entityRepository.GetByIdWithDetailsAsync(id);
        if (entity is null) return null;
        var dto = EntityMapper.ToDto(entity);
        return await EnrichWithRelationalDataAsync(dto, id);
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
        await ValidateParentsAreCharactersAsync(dto.Parent1Id, dto.Parent2Id);
        var entity = EntityMapper.ToEntity(dto);
        await entityRepository.AddAsync(entity);
        await entityRepository.SaveChangesAsync();
        return EntityMapper.ToDto(entity);
    }

    public async Task<EntityDto> UpdateAsync(Guid id, UpdateEntityDto dto)
    {
        var entity = await entityRepository.GetByIdWithDetailsAsync(id)
            ?? throw new NotFoundException(nameof(Entity), id);

        await ValidateParentsAreCharactersAsync(dto.Parent1Id, dto.Parent2Id);
        await ValidatePlaceReferencesAsync(dto.BirthPlaceEntityId, dto.DeathPlaceEntityId);
        await ValidateLocationIdsArePlacesAsync(dto.LocationIds);
        await ValidateAffiliationIdsAreGroupsAsync(dto.AffiliationIds);

        EntityMapper.UpdateEntity(dto, entity);

        // Junction sync: Locations (clear + re-add, EF tracks the diff)
        entity.Locations.Clear();
        foreach (var placeId in dto.LocationIds)
            entity.Locations.Add(new EntityLocation { CharacterId = entity.Id, PlaceId = placeId });

        // Junction sync: Affiliations
        entity.Affiliations.Clear();
        foreach (var groupId in dto.AffiliationIds)
            entity.Affiliations.Add(new EntityAffiliation { CharacterId = entity.Id, GroupId = groupId });

        // Junction sync: Languages
        entity.Languages.Clear();
        foreach (var languageId in dto.LanguageIds)
            entity.Languages.Add(new EntityLanguage { CharacterId = entity.Id, LanguageId = languageId });

        await entityRepository.UpdateAsync(entity);
        await entityRepository.SaveChangesAsync();

        // Spouse and child sync after main save
        await SyncSpousesAsync(entity.Id, dto.SpouseIds);
        await SyncChildrenAsync(entity.Id, dto.ChildIds);

        var updated = await entityRepository.GetByIdWithDetailsAsync(id)!;
        var updatedDto = EntityMapper.ToDto(updated!);
        return await EnrichWithRelationalDataAsync(updatedDto, id);
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

    // --- Helpers ---

    private async Task<EntityDto> EnrichWithRelationalDataAsync(EntityDto dto, Guid id)
    {
        var rels = await relationshipRepository.GetEntityRelationshipsAsync(id);
        var spouseRels = rels.Where(r => r.RelationshipType == RelationshipType.Spouse).ToList();
        var spouseIds = spouseRels.Select(r => r.Entity1Id == id ? r.Entity2Id : r.Entity1Id).ToList();

        var spouses = new List<EntityRefDto>();
        foreach (var spouseId in spouseIds)
        {
            var spouse = await entityRepository.GetByIdAsync(spouseId);
            if (spouse is not null) spouses.Add(new EntityRefDto(spouse.Id, spouse.Name));
        }

        var childEntities = await entityRepository.GetChildrenAsync(id);
        var children = childEntities.Select(c => new EntityRefDto(c.Id, c.Name)).ToList();

        return dto with { Spouses = spouses, Children = children };
    }

    private async Task SyncSpousesAsync(Guid characterId, IReadOnlyList<Guid> desiredSpouseIds)
    {
        var rels = await relationshipRepository.GetEntityRelationshipsAsync(characterId);
        var currentSpouseRels = rels.Where(r => r.RelationshipType == RelationshipType.Spouse).ToList();
        var currentSpouseIds = currentSpouseRels
            .Select(r => r.Entity1Id == characterId ? r.Entity2Id : r.Entity1Id)
            .ToHashSet();
        var desiredSet = desiredSpouseIds.ToHashSet();

        // Remove relationships no longer desired
        foreach (var rel in currentSpouseRels)
        {
            var otherId = rel.Entity1Id == characterId ? rel.Entity2Id : rel.Entity1Id;
            if (!desiredSet.Contains(otherId))
                await relationshipRepository.DeleteAsync(rel);
        }

        // Add new relationships
        foreach (var spouseId in desiredSet.Where(sid => !currentSpouseIds.Contains(sid)))
        {
            await relationshipRepository.AddAsync(new Relationship
            {
                Entity1Id = characterId,
                Entity2Id = spouseId,
                RelationshipType = RelationshipType.Spouse
            });
        }

        await relationshipRepository.SaveChangesAsync();
    }

    private async Task SyncChildrenAsync(Guid characterId, IReadOnlyList<Guid> desiredChildIds)
    {
        var currentChildren = (await entityRepository.GetChildrenAsync(characterId)).ToList();
        var currentChildIds = currentChildren.Select(c => c.Id).ToHashSet();
        var desiredSet = desiredChildIds.ToHashSet();

        // Remove children no longer in list (clear their parent reference)
        foreach (var child in currentChildren.Where(c => !desiredSet.Contains(c.Id)))
        {
            var tracked = await entityRepository.GetByIdAsync(child.Id);
            if (tracked is null) continue;
            // Load tracked version via context
            var trackedChild = await GetTrackedEntityAsync(child.Id);
            if (trackedChild is null) continue;
            if (trackedChild.Parent1Id == characterId) trackedChild.Parent1Id = null;
            else if (trackedChild.Parent2Id == characterId) trackedChild.Parent2Id = null;
            await entityRepository.UpdateAsync(trackedChild);
            await entityRepository.SaveChangesAsync();
        }

        // Add new children
        foreach (var childId in desiredSet.Where(cid => !currentChildIds.Contains(cid)))
        {
            var child = await entityRepository.GetByIdAsync(childId)
                ?? throw new NotFoundException(nameof(Entity), childId);
            if (child.Type != EntityType.Character)
                throw new ValidationException("ChildIds", "Children must be Character entities.");

            var trackedChild = await GetTrackedEntityAsync(childId);
            if (trackedChild is null) continue;

            if (trackedChild.Parent1Id is null)
                trackedChild.Parent1Id = characterId;
            else if (trackedChild.Parent2Id is null)
                trackedChild.Parent2Id = characterId;
            else
                throw new ValidationException("ChildIds", $"Character '{child.Name}' already has two parents assigned.");

            await entityRepository.UpdateAsync(trackedChild);
            await entityRepository.SaveChangesAsync();
        }
    }

    private async Task<Entity?> GetTrackedEntityAsync(Guid id) =>
        await entityRepository.GetByIdWithDetailsAsync(id);

    private async Task ValidateParentsAreCharactersAsync(Guid? parent1Id, Guid? parent2Id)
    {
        if (parent1Id is not null)
        {
            var parent1 = await entityRepository.GetByIdAsync(parent1Id.Value)
                ?? throw new NotFoundException(nameof(Entity), parent1Id.Value);
            if (parent1.Type != EntityType.Character)
                throw new ValidationException("Parent1Id", "Parent must be a Character.");
        }

        if (parent2Id is not null)
        {
            var parent2 = await entityRepository.GetByIdAsync(parent2Id.Value)
                ?? throw new NotFoundException(nameof(Entity), parent2Id.Value);
            if (parent2.Type != EntityType.Character)
                throw new ValidationException("Parent2Id", "Parent must be a Character.");
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
