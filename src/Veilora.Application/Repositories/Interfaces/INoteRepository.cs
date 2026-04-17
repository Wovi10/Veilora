using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface INoteRepository : IRepository<Note>
{
    Task<IEnumerable<Note>> GetByWorldIdAsync(Guid worldId);
    Task<IEnumerable<Note>> GetByEntityIdAsync(Guid entityId);
}
