using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IEntityRepository : IRepository<Entity>
{
    Task<IEnumerable<Entity>> SearchAsync(string searchTerm);
    Task<IEnumerable<Entity>> GetByWorldIdAsync(Guid worldId);
    Task<IEnumerable<Entity>> GetByWorldIdAndTypeAsync(Guid worldId, string type);
    Task<PagedResult<Entity>> GetPagedAsync(EntityCriteria criteria);
    Task<IEnumerable<(Guid Id, string Name, string Type)>> SearchByWorldAsync(Guid worldId, string term, int limit);
}
