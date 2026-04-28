using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IReadingNoteRepository : IRepository<ReadingNote>
{
    Task<IEnumerable<ReadingNote>> GetBySessionAsync(Guid sessionId);
}
