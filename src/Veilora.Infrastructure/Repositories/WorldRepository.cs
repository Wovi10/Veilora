using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class WorldRepository(ApplicationDbContext context) : Repository<World>(context), IWorldRepository
{
    public async Task<IEnumerable<World>> GetAllByUserAsync(Guid userId) =>
        await _dbSet.AsNoTracking().Where(w => w.CreatedById == userId).ToListAsync();
}
