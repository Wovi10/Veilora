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
            .Where(e => e.Name.ToLower().Contains(term)
                || (e.FirstName != null && e.FirstName.ToLower().Contains(term))
                || (e.LastName != null && e.LastName.ToLower().Contains(term)))
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

    public async Task<IEnumerable<Entity>> GetEntitiesByFamilyTreeIdAsync(Guid familyTreeId) =>
        await _context.EntityFamilyTrees
            .AsNoTracking()
            .Where(eft => eft.FamilyTreeId == familyTreeId)
            .Select(eft => eft.Entity)
            .ToListAsync();

    public async Task<IEnumerable<Entity>> GetAncestorsAsync(Guid entityId)
    {
        var ancestors = new List<Entity>();
        var visited = new HashSet<Guid>();
        await CollectAncestorsAsync(entityId, ancestors, visited);
        return ancestors;
    }

    private async Task CollectAncestorsAsync(Guid entityId, List<Entity> ancestors, HashSet<Guid> visited)
    {
        if (!visited.Add(entityId)) return;
        var entity = await _context.Entities.AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == entityId);
        if (entity is null) return;
        if (entity.Parent1Id.HasValue)
        {
            var parent1 = await _context.Entities.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == entity.Parent1Id);
            if (parent1 is not null) { ancestors.Add(parent1); await CollectAncestorsAsync(parent1.Id, ancestors, visited); }
        }
        if (entity.Parent2Id.HasValue)
        {
            var parent2 = await _context.Entities.AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == entity.Parent2Id);
            if (parent2 is not null) { ancestors.Add(parent2); await CollectAncestorsAsync(parent2.Id, ancestors, visited); }
        }
    }

    public async Task<IEnumerable<Entity>> GetDescendantsAsync(Guid entityId)
    {
        var descendants = new List<Entity>();
        var visited = new HashSet<Guid>();
        await CollectDescendantsAsync(entityId, descendants, visited);
        return descendants;
    }

    private async Task CollectDescendantsAsync(Guid entityId, List<Entity> descendants, HashSet<Guid> visited)
    {
        if (!visited.Add(entityId)) return;
        var children = await _context.Entities.AsNoTracking()
            .Where(e => e.Parent1Id == entityId || e.Parent2Id == entityId)
            .ToListAsync();
        foreach (var child in children)
        {
            descendants.Add(child);
            await CollectDescendantsAsync(child.Id, descendants, visited);
        }
    }
}
