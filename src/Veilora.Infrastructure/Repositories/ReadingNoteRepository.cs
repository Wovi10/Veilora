using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class ReadingNoteRepository(ApplicationDbContext context)
    : Repository<ReadingNote>(context), IReadingNoteRepository
{
    public async Task<IEnumerable<ReadingNote>> GetBySessionAsync(Guid sessionId) =>
        await _context.ReadingNotes
            .AsNoTracking()
            .Where(n => n.SessionId == sessionId)
            .OrderBy(n => n.CreatedAt)
            .ToListAsync();
}
