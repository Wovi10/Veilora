using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Domain.Enums;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Person>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await GetAllAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();

        return await _dbSet
            .AsNoTracking()
            .Where(p => p.FirstName.ToLower().Contains(lowerSearchTerm) ||
                       p.LastName.ToLower().Contains(lowerSearchTerm) ||
                       (p.MaidenName != null && p.MaidenName.ToLower().Contains(lowerSearchTerm)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetPersonsByTreeIdAsync(Guid treeId)
    {
        return await _context.PersonTrees
            .AsNoTracking()
            .Where(pt => pt.TreeId == treeId)
            .Select(pt => pt.Person)
            .ToListAsync();
    }

    public async Task<IEnumerable<Person>> GetAncestorsAsync(Guid personId)
    {
        var ancestors = new List<Person>();
        var visitedIds = new HashSet<Guid>();
        await GetAncestorsRecursiveAsync(personId, ancestors, visitedIds);
        return ancestors;
    }

    private async Task GetAncestorsRecursiveAsync(Guid personId, List<Person> ancestors, HashSet<Guid> visitedIds)
    {
        if (visitedIds.Contains(personId))
        {
            return;
        }

        visitedIds.Add(personId);

        // Get parent relationships where personId is Person1 (child)
        var parentRelationships = await _context.Relationships
            .AsNoTracking()
            .Include(r => r.Person2)
            .Where(r => r.Person1Id == personId &&
                       (r.RelationshipType == RelationshipType.ParentChildBiological ||
                        r.RelationshipType == RelationshipType.ParentChildAdopted))
            .ToListAsync();

        foreach (var relationship in parentRelationships)
        {
            if (!ancestors.Any(a => a.Id == relationship.Person2Id))
            {
                ancestors.Add(relationship.Person2);
                await GetAncestorsRecursiveAsync(relationship.Person2Id, ancestors, visitedIds);
            }
        }
    }

    public async Task<IEnumerable<Person>> GetDescendantsAsync(Guid personId)
    {
        var descendants = new List<Person>();
        var visitedIds = new HashSet<Guid>();
        await GetDescendantsRecursiveAsync(personId, descendants, visitedIds);
        return descendants;
    }

    private async Task GetDescendantsRecursiveAsync(Guid personId, List<Person> descendants, HashSet<Guid> visitedIds)
    {
        if (visitedIds.Contains(personId))
        {
            return;
        }

        visitedIds.Add(personId);

        // Get child relationships where personId is Person2 (parent)
        var childRelationships = await _context.Relationships
            .AsNoTracking()
            .Include(r => r.Person1)
            .Where(r => r.Person2Id == personId &&
                       (r.RelationshipType == RelationshipType.ParentChildBiological ||
                        r.RelationshipType == RelationshipType.ParentChildAdopted))
            .ToListAsync();

        foreach (var relationship in childRelationships)
        {
            if (!descendants.Any(d => d.Id == relationship.Person1Id))
            {
                descendants.Add(relationship.Person1);
                await GetDescendantsRecursiveAsync(relationship.Person1Id, descendants, visitedIds);
            }
        }
    }
}

