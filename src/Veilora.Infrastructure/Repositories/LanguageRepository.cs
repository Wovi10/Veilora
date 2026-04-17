using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class LanguageRepository(ApplicationDbContext context)
    : Repository<Language>(context), ILanguageRepository
{
    public async Task<IEnumerable<Language>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Languages
            .AsNoTracking()
            .Where(l => l.WorldId == worldId)
            .OrderBy(l => l.Name)
            .ToListAsync();

    public async Task<Language?> GetByNameAndWorldIdAsync(string name, Guid worldId) =>
        await _context.Languages
            .FirstOrDefaultAsync(l => l.WorldId == worldId && l.Name == name);
}
