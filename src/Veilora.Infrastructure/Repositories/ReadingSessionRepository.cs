using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class ReadingSessionRepository(ApplicationDbContext context)
    : Repository<ReadingSession>(context), IReadingSessionRepository
{
    public async Task<ReadingSession?> GetActiveByUserAsync(Guid userId) =>
        await _context.ReadingSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && s.EndedAt == null);

    public async Task<IEnumerable<ReadingSession>> GetByUserAsync(Guid userId) =>
        await _context.ReadingSessions
            .AsNoTracking()
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.StartedAt)
            .ToListAsync();

    public async Task<ReadingSession?> GetWithNotesAsync(Guid sessionId) =>
        await _context.ReadingSessions
            .AsNoTracking()
            .Include(s => s.Notes)
            .FirstOrDefaultAsync(s => s.Id == sessionId);
}
