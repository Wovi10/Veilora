using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class RelationshipRepository(ApplicationDbContext context) : Repository<Relationship>(context), IRelationshipRepository
{
    public async Task<IEnumerable<Relationship>> GetRelationshipsByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var entityIds = await _context.EntityFamilyTrees
            .Where(eft => eft.FamilyTreeId == familyTreeId)
            .Select(eft => eft.EntityId)
            .ToListAsync();
        return await _context.Relationships
            .AsNoTracking()
            .Where(r => entityIds.Contains(r.Entity1Id) && entityIds.Contains(r.Entity2Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Relationship>> GetEntityRelationshipsAsync(Guid entityId) =>
        await _context.Relationships
            .AsNoTracking()
            .Where(r => r.Entity1Id == entityId || r.Entity2Id == entityId)
            .ToListAsync();
}
