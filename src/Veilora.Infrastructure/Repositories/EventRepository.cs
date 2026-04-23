using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class EventRepository(ApplicationDbContext context) : Repository<Event>(context), IEventRepository
{
    public async Task<IEnumerable<Event>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Events
            .AsNoTracking()
            .Where(e => e.WorldId == worldId)
            .OrderBy(e => e.Name)
            .ToListAsync();

    public async Task<Event?> FindByNameAsync(string name, Guid worldId) =>
        await _context.Events
            .Where(e => e.WorldId == worldId && e.Name.ToLower() == name.ToLower())
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<(Guid Id, string Name)>> SearchByWorldAsync(Guid worldId, string term, int limit)
    {
        var lower = term.ToLower();
        return await _context.Events
            .AsNoTracking()
            .Where(e => e.WorldId == worldId && e.Name.ToLower().Contains(lower))
            .OrderBy(e => e.Name)
            .Take(limit)
            .Select(e => new { e.Id, e.Name })
            .ToListAsync()
            .ContinueWith(t => t.Result.Select(x => (x.Id, x.Name)));
    }

    public async Task<PagedResult<Event>> GetPagedAsync(EventCriteria criteria)
    {
        var query = _context.Events
            .AsNoTracking()
            .Where(e => e.WorldId == criteria.WorldId);
        if (criteria.Name is not null)
        {
            var term = criteria.Name.ToLower();
            query = query.Where(e => e.Name.ToLower().Contains(term));
        }
        query = query.OrderBy(e => e.Name);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
        return new PagedResult<Event>(items, totalCount, criteria.Page, criteria.PageSize);
    }
}
