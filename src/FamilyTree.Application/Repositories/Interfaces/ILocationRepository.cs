using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface ILocationRepository : IRepository<Location>
{
    Task<IEnumerable<Location>> GetByWorldIdAsync(Guid worldId);
}
