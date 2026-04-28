using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class WorldRepository(ApplicationDbContext context) : Repository<World>(context), IWorldRepository
{
    public async Task<IEnumerable<World>> GetAllByUserAsync(Guid userId) =>
        await _dbSet.AsNoTracking().Where(w => w.CreatedById == userId).ToListAsync();

    public async Task TransferOwnershipAsync(Guid fromUserId, Guid toUserId) =>
        await _dbSet.Where(w => w.CreatedById == fromUserId)
            .ExecuteUpdateAsync(s => s.SetProperty(w => w.CreatedById, (Guid?)toUserId));
}
