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

    public async Task<Location?> FindByNameAsync(string name, Guid worldId) =>
        await _context.Locations
            .Where(l => l.WorldId == worldId && l.Name.ToLower() == name.ToLower())
            .FirstOrDefaultAsync();
}
