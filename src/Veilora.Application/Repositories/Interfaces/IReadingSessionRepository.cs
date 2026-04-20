using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IReadingSessionRepository : IRepository<ReadingSession>
{
    Task<ReadingSession?> GetActiveByUserAsync(Guid userId);
    Task<IEnumerable<ReadingSession>> GetByUserAsync(Guid userId);
    Task<ReadingSession?> GetWithNotesAsync(Guid sessionId);
}
