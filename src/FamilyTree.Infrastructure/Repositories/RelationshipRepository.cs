using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class RelationshipRepository : Repository<Relationship>, IRelationshipRepository
{
    public RelationshipRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Relationship>> GetRelationshipsByTreeIdAsync(Guid treeId)
    {
        // Get all person IDs in the tree
        var personIdsInTree = await _context.PersonTrees
            .AsNoTracking()
            .Where(pt => pt.TreeId == treeId)
            .Select(pt => pt.PersonId)
            .ToListAsync();

        // Get relationships where both persons are in the tree
        return await _dbSet
            .AsNoTracking()
            .Where(r => personIdsInTree.Contains(r.Person1Id) &&
                       personIdsInTree.Contains(r.Person2Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Relationship>> GetPersonRelationshipsAsync(Guid personId)
    {
        return await _dbSet
            .AsNoTracking()
            .Where(r => r.Person1Id == personId || r.Person2Id == personId)
            .ToListAsync();
    }
}

