using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Infrastructure.Repositories;

public class FamilyTreeRepository(ApplicationDbContext context) : Repository<FamilyTreeEntity>(context), IFamilyTreeRepository
{
    public async Task<FamilyTreeEntity?> GetFamilyTreeWithEntitiesAsync(Guid familyTreeId) =>
        await _context.FamilyTrees
            .AsNoTracking()
            .Include(ft => ft.EntityFamilyTrees)
                .ThenInclude(eft => eft.Entity)
            .FirstOrDefaultAsync(ft => ft.Id == familyTreeId);

    public async Task<IEnumerable<FamilyTreeEntity>> GetByWorldIdAsync(Guid worldId) =>
        await _context.FamilyTrees
            .AsNoTracking()
            .Where(ft => ft.WorldId == worldId)
            .ToListAsync();

    public async Task AddEntityToFamilyTreeAsync(Guid familyTreeId, Guid entityId)
    {
        var junction = new EntityFamilyTree { FamilyTreeId = familyTreeId, EntityId = entityId };
        await _context.EntityFamilyTrees.AddAsync(junction);
    }

    public async Task RemoveEntityFromFamilyTreeAsync(Guid familyTreeId, Guid entityId)
    {
        var junction = await _context.EntityFamilyTrees
            .FirstOrDefaultAsync(eft => eft.FamilyTreeId == familyTreeId && eft.EntityId == entityId);
        if (junction is not null) _context.EntityFamilyTrees.Remove(junction);
    }

    public async Task<bool> IsEntityInFamilyTreeAsync(Guid familyTreeId, Guid entityId) =>
        await _context.EntityFamilyTrees
            .AnyAsync(eft => eft.FamilyTreeId == familyTreeId && eft.EntityId == entityId);

    public async Task UpdateEntityPositionAsync(Guid familyTreeId, Guid entityId, double x, double y)
    {
        var junction = await _context.EntityFamilyTrees
            .FirstOrDefaultAsync(eft => eft.FamilyTreeId == familyTreeId && eft.EntityId == entityId);
        if (junction is null) return;
        junction.PositionX = x;
        junction.PositionY = y;
        await _context.SaveChangesAsync();
    }
}
