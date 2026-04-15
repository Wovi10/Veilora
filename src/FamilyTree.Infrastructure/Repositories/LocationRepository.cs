using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class LocationRepository(ApplicationDbContext context) : Repository<Location>(context), ILocationRepository
{
    public async Task<IEnumerable<Location>> GetByWorldIdAsync(Guid worldId) =>
        await _context.Locations
            .AsNoTracking()
            .Where(l => l.WorldId == worldId)
            .OrderBy(l => l.Name)
            .ToListAsync();
}
