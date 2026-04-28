using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Event;

namespace Veilora.Application.Services.Interfaces;

public interface IEventService
{
    Task<IEnumerable<EventDto>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<EventDto>> GetPagedAsync(EventCriteria criteria);
    Task<EventDto?> GetByIdAsync(Guid id);
    Task<EventDto> CreateAsync(CreateEventDto dto);
    Task<EventDto> UpdateAsync(Guid id, UpdateEventDto dto);
    Task DeleteAsync(Guid id);
}
