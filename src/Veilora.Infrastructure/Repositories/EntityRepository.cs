using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class EntityRepository(ApplicationDbContext context) : Repository<Entity>(context), IEntityRepository
{
    public async Task<IEnumerable<Entity>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Entities
            .AsNoTracking()
            .Where(e => e.Name.ToLower().Contains(term))
            .ToListAsync();
    }

    public async Task<IEnumerable<Entity>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Entities
            .AsNoTracking()
            .Where(e => e.WorldId == worldId)
            .ToListAsync();

    public async Task<IEnumerable<Entity>> GetByWorldIdAndTypeAsync(Guid worldId, string type)
    {
        var entityType = Enum.Parse<EntityType>(type);
        return await _context.Entities
            .AsNoTracking()
            .Where(e => e.WorldId == worldId && e.Type == entityType)
            .ToListAsync();
    }

    public async Task<PagedResult<Entity>> GetPagedAsync(EntityCriteria criteria)
    {
        var query = _context.Entities
            .AsNoTracking()
            .Where(e => e.WorldId == criteria.WorldId);
        if (criteria.Type is not null)
        {
            var entityType = Enum.Parse<EntityType>(criteria.Type);
            query = query.Where(e => e.Type == entityType);
        }
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
        return new PagedResult<Entity>(items, totalCount, criteria.Page, criteria.PageSize);
    }
}
