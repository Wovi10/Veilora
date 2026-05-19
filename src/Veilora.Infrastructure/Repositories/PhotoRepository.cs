using Veilora.Application.Repositories.Interfaces;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;
using Veilora.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Veilora.Infrastructure.Repositories;

public class PhotoRepository(ApplicationDbContext context) : Repository<Photo>(context), IPhotoRepository
{
    public async Task<IEnumerable<Photo>> GetByEntityAsync(PhotoTargetType type, Guid entityId) =>
        await _dbSet
            .AsNoTracking()
            .Where(p => p.EntityType == type && p.EntityId == entityId)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync();

    public async Task<int> CountByEntityAsync(PhotoTargetType type, Guid entityId) =>
        await _dbSet.CountAsync(p => p.EntityType == type && p.EntityId == entityId);

    public async Task<int> GetMaxDisplayOrderAsync(PhotoTargetType type, Guid entityId) =>
        (int?)await _dbSet
            .Where(p => p.EntityType == type && p.EntityId == entityId)
            .MaxAsync(p => (int?)p.DisplayOrder) ?? -1;

    public async Task DeleteByEntityAsync(PhotoTargetType type, Guid entityId)
    {
        var photos = await _dbSet
            .Where(p => p.EntityType == type && p.EntityId == entityId)
            .ToListAsync();
        _dbSet.RemoveRange(photos);
    }
}
