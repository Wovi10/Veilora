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
        var chars    = await characterRepository.SearchByWorldAsync(criteria);
        var locs     = await locationRepository.SearchByWorldAsync(criteria);
        var entities = await entityRepository.SearchByWorldAsync(criteria);
        var trees    = await familyTreeRepository.SearchByWorldAsync(criteria);

        return new WorldSearchResultDto
        {
            Characters  = chars.Select(c    => new WorldSearchItemDto(c.Id, c.Name)).ToList(),
            Locations   = locs.Select(l     => new WorldSearchItemDto(l.Id, l.Name)).ToList(),
            Groups      = entities.Where(e => e.Type == EntityType.Group)  .Select(e => new WorldSearchItemDto(e.Id, e.Name)).ToList(),
            Events      = entities.Where(e => e.Type == EntityType.Event)  .Select(e => new WorldSearchItemDto(e.Id, e.Name)).ToList(),
            Concepts    = entities.Where(e => e.Type == EntityType.Concept).Select(e => new WorldSearchItemDto(e.Id, e.Name)).ToList(),
            FamilyTrees = trees.Select(ft   => new WorldSearchItemDto(ft.Id, ft.Name)).ToList(),
        };
    }
}
