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
            .Include(ft => ft.CharacterFamilyTrees)
                .ThenInclude(cft => cft.Character)
                    .ThenInclude(c => c.BirthPlaceEntity)
            .Include(ft => ft.CharacterFamilyTrees)
                .ThenInclude(cft => cft.Character)
                    .ThenInclude(c => c.DeathPlaceEntity)
            .Include(ft => ft.CharacterFamilyTrees)
                .ThenInclude(cft => cft.Character)
                    .ThenInclude(c => c.Locations).ThenInclude(l => l.Place)
            .Include(ft => ft.CharacterFamilyTrees)
                .ThenInclude(cft => cft.Character)
                    .ThenInclude(c => c.Affiliations).ThenInclude(a => a.Group)
            .Include(ft => ft.CharacterFamilyTrees)
                .ThenInclude(cft => cft.Character)
                    .ThenInclude(c => c.Languages).ThenInclude(l => l.Language)
            .FirstOrDefaultAsync(ft => ft.Id == familyTreeId);

    public async Task<IEnumerable<FamilyTreeEntity>> GetByWorldIdAsync(Guid worldId) =>
        await _context.FamilyTrees
            .AsNoTracking()
            .Where(ft => ft.WorldId == worldId)
            .ToListAsync();

    public async Task AddCharacterToFamilyTreeAsync(Guid familyTreeId, Guid characterId)
    {
        var junction = new CharacterFamilyTree { FamilyTreeId = familyTreeId, CharacterId = characterId };
        await _context.CharacterFamilyTrees.AddAsync(junction);
    }

    public async Task RemoveCharacterFromFamilyTreeAsync(Guid familyTreeId, Guid characterId)
    {
        var junction = await _context.CharacterFamilyTrees
            .FirstOrDefaultAsync(cft => cft.FamilyTreeId == familyTreeId && cft.CharacterId == characterId);
        if (junction is not null) _context.CharacterFamilyTrees.Remove(junction);
    }

    public async Task<bool> IsCharacterInFamilyTreeAsync(Guid familyTreeId, Guid characterId) =>
        await _context.CharacterFamilyTrees
            .AnyAsync(cft => cft.FamilyTreeId == familyTreeId && cft.CharacterId == characterId);

    public async Task UpdateCharacterPositionAsync(Guid familyTreeId, Guid characterId, double x, double y)
    {
        var junction = await _context.CharacterFamilyTrees
            .FirstOrDefaultAsync(cft => cft.FamilyTreeId == familyTreeId && cft.CharacterId == characterId);
        if (junction is null) return;
        junction.PositionX = x;
        junction.PositionY = y;
        await _context.SaveChangesAsync();
    }
}
