using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetByWorldIdAsync(Guid worldId);
    Task<Location?> FindByNameAsync(string name, Guid worldId);
    Task<PagedResult<Location>> GetPagedAsync(LocationCriteria criteria);
    Task<IEnumerable<(Guid Id, string Name)>> SearchByWorldAsync(Guid worldId, string term, int limit);
}
