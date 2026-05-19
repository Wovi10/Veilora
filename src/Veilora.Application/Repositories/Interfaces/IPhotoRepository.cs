using Veilora.Domain.Entities;
using Veilora.Domain.Enums;

namespace Veilora.Application.Repositories.Interfaces;

public interface IPhotoRepository : IRepository<Photo>
{
    Task<IEnumerable<Photo>> GetByEntityAsync(PhotoTargetType type, Guid entityId);
    Task<int> CountByEntityAsync(PhotoTargetType type, Guid entityId);
    Task<int> GetMaxDisplayOrderAsync(PhotoTargetType type, Guid entityId);
    Task DeleteByEntityAsync(PhotoTargetType type, Guid entityId);
}
