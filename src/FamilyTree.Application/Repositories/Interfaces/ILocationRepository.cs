using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetByWorldIdAsync(Guid worldId);
    Task<Location?> FindByNameAsync(string name, Guid worldId);
    Task<PagedResult<Location>> GetPagedAsync(LocationCriteria criteria);
}
