using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class LocationRepository(ApplicationDbContext context) : Repository<Location>(context), ILocationRepository
{
    public async Task<IEnumerable<Location>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Locations
            .AsNoTracking()
            .Where(l => l.WorldId == worldId)
            .OrderBy(l => l.Name)
            .ToListAsync();

    public async Task<Location?> FindByNameAsync(string name, Guid worldId) =>
        await _context.Locations
            .Where(l => l.WorldId == worldId && l.Name.ToLower() == name.ToLower())
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<(Guid Id, string Name)>> SearchByWorldAsync(Guid worldId, string term, int limit)
    {
        var lower = term.ToLower();
        return await _context.Locations
            .AsNoTracking()
            .Where(l => l.WorldId == worldId && l.Name.ToLower().Contains(lower))
            .OrderBy(l => l.Name)
            .Take(limit)
            .Select(l => new { l.Id, l.Name })
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.Id, x.Name)));
    }

    public async Task<PagedResult<Location>> GetPagedAsync(LocationCriteria criteria)
    {
        var query = _context.Locations
            .AsNoTracking()
            .Where(l => l.WorldId == criteria.WorldId);
        if (criteria.Name is not null)
        {
            var term = criteria.Name.ToLower();
            query = query.Where(l => l.Name.ToLower().Contains(term));
        }
        query = query.OrderBy(l => l.Name);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
        return new PagedResult<Location>(items, totalCount, criteria.Page, criteria.PageSize);
    }
}
