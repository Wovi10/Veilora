using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

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
