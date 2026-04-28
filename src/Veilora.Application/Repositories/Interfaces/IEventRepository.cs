using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<IEnumerable<Event>> GetByWorldIdAsync(Guid worldId);
    Task<Event?> FindByNameAsync(string name, Guid worldId);
    Task<PagedResult<Event>> GetPagedAsync(EventCriteria criteria);
    Task<IEnumerable<(Guid Id, string Name)>> SearchByWorldAsync(Guid worldId, string term, int limit);
}
