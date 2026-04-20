using Veilora.Application.Criteria;
using Veilora.Application.DTOs.World;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Enums;

namespace Veilora.Application.Services;

public class WorldSearchService(
    ICharacterRepository characterRepository,
    ILocationRepository locationRepository,
    IEntityRepository entityRepository,
    IFamilyTreeRepository familyTreeRepository) : IWorldSearchService
{
    public async Task<WorldSearchResultDto> SearchAsync(WorldSearchCriteria criteria)
    {
        var charsTask      = characterRepository.SearchByWorldAsync(criteria);
        var locsTask       = locationRepository.SearchByWorldAsync(criteria);
        var entitiesTask   = entityRepository.SearchByWorldAsync(criteria);
        var treesTask      = familyTreeRepository.SearchByWorldAsync(criteria);

        await Task.WhenAll(charsTask, locsTask, entitiesTask, treesTask);

        var entities = await entitiesTask;

        return new WorldSearchResultDto
        {
            Characters  = (await charsTask).Select(c  => new WorldSearchItemDto(c.Id, c.Name)).ToList(),
            Locations   = (await locsTask).Select(l   => new WorldSearchItemDto(l.Id, l.Name)).ToList(),
            Groups      = entities.Where(e => e.Type == EntityType.Group)  .Select(e => new WorldSearchItemDto(e.Id, e.Name)).ToList(),
            Events      = entities.Where(e => e.Type == EntityType.Event)  .Select(e => new WorldSearchItemDto(e.Id, e.Name)).ToList(),
            Concepts    = entities.Where(e => e.Type == EntityType.Concept).Select(e => new WorldSearchItemDto(e.Id, e.Name)).ToList(),
            FamilyTrees = (await treesTask).Select(ft => new WorldSearchItemDto(ft.Id, ft.Name)).ToList(),
        };
    }
}
