using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class DateSuffixRepository(ApplicationDbContext context) : Repository<DateSuffix>(context), IDateSuffixRepository
{
    public async Task<IEnumerable<DateSuffix>> GetByWorldIdAsync(Guid worldId) =>
        await _context.DateSuffixes
            .AsNoTracking()
            .Where(d => d.WorldId == worldId)
            .OrderBy(d => d.AnchorYear * (double)d.Scale)
            .ToListAsync();

    public async Task<DateSuffix?> GetDefaultForWorldAsync(Guid worldId) =>
        await _context.DateSuffixes
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.WorldId == worldId && d.IsDefault);

    public async Task ClearDefaultForWorldAsync(Guid worldId)
    {
        await _context.DateSuffixes
            .Where(d => d.WorldId == worldId && d.IsDefault)
            .ExecuteUpdateAsync(s => s.SetProperty(d => d.IsDefault, false));
    }
}
