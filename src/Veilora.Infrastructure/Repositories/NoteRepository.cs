using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class NoteRepository(ApplicationDbContext context) : Repository<Note>(context), INoteRepository
{
    public async Task<IEnumerable<Note>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Notes
            .AsNoTracking()
            .Where(n => n.WorldId == worldId)
            .ToListAsync();

    public async Task<IEnumerable<Note>> GetByEntityIdAsync(Guid entityId) =>
        await _context.Notes
            .AsNoTracking()
            .Where(n => n.EntityId == entityId)
            .ToListAsync();
}
