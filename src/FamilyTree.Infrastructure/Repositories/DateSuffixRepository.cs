using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class DateSuffixRepository(ApplicationDbContext context) : Repository<DateSuffix>(context), IDateSuffixRepository
{
    public async Task<IEnumerable<DateSuffix>> GetByWorldIdAsync(Guid worldId) =>
        await _context.DateSuffixes
            .AsNoTracking()
            .Where(d => d.WorldId == worldId)
            .OrderBy(d => d.Order)
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
