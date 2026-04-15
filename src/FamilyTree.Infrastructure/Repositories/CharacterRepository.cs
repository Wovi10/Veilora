using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class CharacterRepository(ApplicationDbContext context) : Repository<Character>(context), ICharacterRepository
{
    public async Task<Character?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.Characters
            .Include(c => c.BirthPlaceLocation)
            .Include(c => c.DeathPlaceLocation)
            .Include(c => c.Locations).ThenInclude(l => l.Location)
            .Include(c => c.Affiliations).ThenInclude(a => a.Group)
            .Include(c => c.Languages).ThenInclude(l => l.Language)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Character>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Characters
            .AsNoTracking()
            .Where(c => c.WorldId == worldId)
            .ToListAsync();

    public async Task<IEnumerable<Character>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Characters
            .AsNoTracking()
            .Where(c => c.Name.ToLower().Contains(term)
                || (c.FirstName != null && c.FirstName.ToLower().Contains(term))
                || (c.LastName != null && c.LastName.ToLower().Contains(term)))
            .ToListAsync();
    }

    public async Task<IEnumerable<Character>> GetAncestorsAsync(Guid characterId)
    {
        var ancestors = new List<Character>();
        var visited = new HashSet<Guid>();
        await CollectAncestorsAsync(characterId, ancestors, visited);
        return ancestors;
    }

    private async Task CollectAncestorsAsync(Guid characterId, List<Character> ancestors, HashSet<Guid> visited)
    {
        if (!visited.Add(characterId)) return;
        var character = await _context.Characters.AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == characterId);
        if (character is null) return;
        if (character.Parent1Id.HasValue)
        {
            var parent1 = await _context.Characters.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == character.Parent1Id);
            if (parent1 is not null) { ancestors.Add(parent1); await CollectAncestorsAsync(parent1.Id, ancestors, visited); }
        }
        if (character.Parent2Id.HasValue)
        {
            var parent2 = await _context.Characters.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == character.Parent2Id);
            if (parent2 is not null) { ancestors.Add(parent2); await CollectAncestorsAsync(parent2.Id, ancestors, visited); }
        }
    }

    public async Task<IEnumerable<Character>> GetDescendantsAsync(Guid characterId)
    {
        var descendants = new List<Character>();
        var visited = new HashSet<Guid>();
        await CollectDescendantsAsync(characterId, descendants, visited);
        return descendants;
    }

    private async Task CollectDescendantsAsync(Guid characterId, List<Character> descendants, HashSet<Guid> visited)
    {
        if (!visited.Add(characterId)) return;
        var children = await _context.Characters.AsNoTracking()
            .Where(c => c.Parent1Id == characterId || c.Parent2Id == characterId)
            .ToListAsync();
        foreach (var child in children)
        {
            descendants.Add(child);
            await CollectDescendantsAsync(child.Id, descendants, visited);
        }
    }

    public async Task<IEnumerable<Character>> GetChildrenAsync(Guid characterId) =>
        await _context.Characters
            .AsNoTracking()
            .Where(c => c.Parent1Id == characterId || c.Parent2Id == characterId)
            .ToListAsync();

    public async Task<IEnumerable<Character>> GetByFamilyTreeIdAsync(Guid familyTreeId) =>
        await _context.CharacterFamilyTrees
            .AsNoTracking()
            .Where(cft => cft.FamilyTreeId == familyTreeId)
            .Select(cft => cft.Character)
            .ToListAsync();
}
