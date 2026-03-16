using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class TreeRepository : Repository<Tree>, ITreeRepository
{
    public TreeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Tree?> GetTreeWithPersonsAsync(Guid treeId)
    {
        return await _dbSet
            .AsNoTracking()
            .Include(t => t.PersonTrees)
            .ThenInclude(pt => pt.Person)
            .FirstOrDefaultAsync(t => t.Id == treeId);
    }

    public async Task AddPersonToTreeAsync(Guid treeId, Guid personId)
    {
        var personTree = new PersonTree
        {
            TreeId = treeId,
            PersonId = personId
        };

        await _context.PersonTrees.AddAsync(personTree);
    }

    public async Task RemovePersonFromTreeAsync(Guid treeId, Guid personId)
    {
        var personTree = await _context.PersonTrees
            .FirstOrDefaultAsync(pt => pt.TreeId == treeId && pt.PersonId == personId);

        if (personTree != null)
        {
            _context.PersonTrees.Remove(personTree);
        }
    }

    public async Task<bool> IsPersonInTreeAsync(Guid treeId, Guid personId)
    {
        return await _context.PersonTrees
            .AsNoTracking()
            .AnyAsync(pt => pt.TreeId == treeId && pt.PersonId == personId);
    }

    public async Task UpdatePersonPositionAsync(Guid treeId, Guid personId, double x, double y)
    {
        var personTree = await _context.PersonTrees
            .FirstOrDefaultAsync(pt => pt.TreeId == treeId && pt.PersonId == personId);

        if (personTree != null)
        {
            personTree.PositionX = x;
            personTree.PositionY = y;
            await _context.SaveChangesAsync();
        }
    }
}

