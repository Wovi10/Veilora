using Veilora.Application.DTOs.Event;
using Veilora.Domain.Entities;

namespace Veilora.Application.Mappers;

public static class EventMapper
{
    public static EventDto ToDto(Event ev) => new()
    {
        Id = ev.Id,
        Name = ev.Name,
        WorldId = ev.WorldId,
        Description = ev.Description,
        CreatedAt = ev.CreatedAt,
        UpdatedAt = ev.UpdatedAt
    };

    public static Event ToEntity(CreateEventDto dto) => new()
    {
        Name = dto.Name,
        WorldId = dto.WorldId,
        Description = dto.Description
    };

    public static void UpdateEntity(UpdateEventDto dto, Event ev)
    {
        ev.Name = dto.Name;
        ev.Description = dto.Description;
    }
}
