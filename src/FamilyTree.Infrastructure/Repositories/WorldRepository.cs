using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Domain.Entities;
using FamilyTree.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FamilyTree.Infrastructure.Repositories;

public class WorldRepository(ApplicationDbContext context) : Repository<World>(context), IWorldRepository
{
    public async Task<IEnumerable<World>> GetAllByUserAsync(Guid userId) =>
        await _dbSet.AsNoTracking().Where(w => w.CreatedById == userId).ToListAsync();
}
