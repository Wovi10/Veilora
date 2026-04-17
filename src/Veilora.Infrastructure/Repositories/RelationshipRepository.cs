using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class RelationshipRepository(ApplicationDbContext context) : Repository<Relationship>(context), IRelationshipRepository
{
    public async Task<IEnumerable<Relationship>> GetRelationshipsByFamilyTreeIdAsync(Guid familyTreeId)
    {
        var characterIds = await _context.CharacterFamilyTrees
            .Where(cft => cft.FamilyTreeId == familyTreeId)
            .Select(cft => cft.CharacterId)
            .ToListAsync();
        return await _context.Relationships
            .AsNoTracking()
            .Where(r => characterIds.Contains(r.Character1Id) && characterIds.Contains(r.Character2Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<Relationship>> GetEntityRelationshipsAsync(Guid characterId) =>
        await _context.Relationships
            .AsNoTracking()
            .Where(r => r.Character1Id == characterId || r.Character2Id == characterId)
            .ToListAsync();
}
