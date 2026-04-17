using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class CharacterRepository(ApplicationDbContext context) : Repository<Character>(context), ICharacterRepository
{
    public async Task<Character?> GetByIdWithDetailsAsync(Guid id) =>
        await _context.Characters
            .Include(c => c.BirthDateSuffix)
            .Include(c => c.DeathDateSuffix)
            .Include(c => c.BirthPlaceLocation)
            .Include(c => c.DeathPlaceLocation)
            .Include(c => c.Locations).ThenInclude(l => l.Location)
            .Include(c => c.Affiliations).ThenInclude(a => a.Group)
            .Include(c => c.Languages).ThenInclude(l => l.Language)
            .FirstOrDefaultAsync(c => c.Id == id);

    public async Task<IEnumerable<Character>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Characters
            .AsNoTracking()
            .Include(c => c.BirthDateSuffix)
            .Include(c => c.DeathDateSuffix)
            .Include(c => c.Languages).ThenInclude(l => l.Language)
            .Where(c => c.WorldId == worldId)
            .OrderBy(c => c.LastName == null)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.BirthDate == null)
            .ThenBy(c => c.BirthDate)
            .ToListAsync();

    public async Task<PagedResult<Character>> GetPagedAsync(CharacterCriteria criteria)
    {
        var query = _context.Characters
            .AsNoTracking()
            .Include(c => c.BirthDateSuffix)
            .Include(c => c.DeathDateSuffix)
            .Where(c => c.WorldId == criteria.WorldId)
            .OrderBy(c => c.LastName == null)
            .ThenBy(c => c.LastName)
            .ThenBy(c => c.BirthDate == null)
            .ThenBy(c => c.BirthDate);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((criteria.Page - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToListAsync();
        return new PagedResult<Character>(items, totalCount, criteria.Page, criteria.PageSize);
    }

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
            .Include(cft => cft.Character).ThenInclude(c => c.BirthDateSuffix)
            .Include(cft => cft.Character).ThenInclude(c => c.DeathDateSuffix)
            .Select(cft => cft.Character)
            .ToListAsync();
}
