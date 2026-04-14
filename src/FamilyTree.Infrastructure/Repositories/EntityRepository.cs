using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

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
}
