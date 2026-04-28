using Veilora.Application.DTOs.Entity;
using Veilora.Application.DTOs.Search;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;


namespace Veilora.Application.Services;

public class SearchService(
    IEntityRepository entityRepository,
    ICharacterRepository characterRepository,
    ILocationRepository locationRepository,
    IEventRepository eventRepository) : ISearchService
{
    private const int Limit = 8;

    public async Task<WorldSearchResultDto> SearchWorldAsync(Guid worldId, string query)
    {
        var entityResults    = await entityRepository.SearchByWorldAsync(worldId, query, Limit);
        var characterResults = await characterRepository.SearchByWorldAsync(worldId, query, Limit);
        var locationResults  = await locationRepository.SearchByWorldAsync(worldId, query, Limit);
        var eventResults     = await eventRepository.SearchByWorldAsync(worldId, query, Limit);

        return new WorldSearchResultDto
        {
            Entities = entityResults
                .Select(e => new EntitySearchItemDto { Id = e.Id, Name = e.Name, Type = e.Type })
                .ToList(),
            Characters = characterResults
                .Select(c => new EntityRefDto(c.Id, c.Name))
                .ToList(),
            Locations = locationResults
                .Select(l => new EntityRefDto(l.Id, l.Name))
                .ToList(),
            Events = eventResults
                .Select(e => new EntityRefDto(e.Id, e.Name))
                .ToList(),
        };
    }
}
