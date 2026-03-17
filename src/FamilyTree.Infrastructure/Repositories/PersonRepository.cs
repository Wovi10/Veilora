using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
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

        var person = await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == personId);

        if (person == null) return;

        foreach (var parentId in new[] { person.Parent1Id, person.Parent2Id })
        {
            if (parentId == null)
                continue;

            var parent = await _dbSet.AsNoTracking().FirstOrDefaultAsync(p => p.Id == parentId);
            if (parent != null && ancestors.All(a => a.Id != parentId))
            {
                ancestors.Add(parent);
                await GetAncestorsRecursiveAsync(parentId.Value, ancestors, visitedIds);
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

        var children = await _dbSet
            .AsNoTracking()
            .Where(p => p.Parent1Id == personId || p.Parent2Id == personId)
            .ToListAsync();

        foreach (var child in children)
        {
            if (!descendants.Any(d => d.Id == child.Id))
            {
                descendants.Add(child);
                await GetDescendantsRecursiveAsync(child.Id, descendants, visitedIds);
            }
        }
    }
}

