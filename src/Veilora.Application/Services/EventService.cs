using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Event;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

public class EventService(
    IEventRepository eventRepository,
    IWorldRepository worldRepository) : IEventService
{
    public async Task<IEnumerable<EventDto>> GetByWorldIdAsync(Guid worldId)
    {
        var events = await eventRepository.GetByWorldIdAsync(worldId);
        return events.Select(EventMapper.ToDto);
    }

    public async Task<EventDto?> GetByIdAsync(Guid id)
    {
        var ev = await eventRepository.GetByIdAsync(id);
        return ev is null ? null : EventMapper.ToDto(ev);
    }

    public async Task<EventDto> CreateAsync(CreateEventDto dto)
    {
        _ = await worldRepository.GetByIdAsync(dto.WorldId)
            ?? throw new NotFoundException(nameof(Event), dto.WorldId);

        var ev = EventMapper.ToEntity(dto);
        await eventRepository.AddAsync(ev);
        await eventRepository.SaveChangesAsync();
        return EventMapper.ToDto(ev);
    }

    public async Task<EventDto> UpdateAsync(Guid id, UpdateEventDto dto)
    {
        var ev = await eventRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Event), id);

        EventMapper.UpdateEntity(dto, ev);
        await eventRepository.UpdateAsync(ev);
        await eventRepository.SaveChangesAsync();
        return EventMapper.ToDto(ev);
    }

    public async Task DeleteAsync(Guid id)
    {
        var ev = await eventRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Event), id);
        await eventRepository.DeleteAsync(ev);
        await eventRepository.SaveChangesAsync();
    }

    public async Task<PagedResult<EventDto>> GetPagedAsync(EventCriteria criteria)
    {
        var paged = await eventRepository.GetPagedAsync(criteria);
        return new PagedResult<EventDto>(
            paged.Items.Select(EventMapper.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
    }
}
