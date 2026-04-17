using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IWorldRepository : IRepository<World>
{
    Task<IEnumerable<World>> GetAllByUserAsync(Guid userId);
}
