using Veilora.Application.DTOs.Photo;
using Veilora.Domain.Enums;

namespace Veilora.Application.Services.Interfaces;

public interface IPhotoService
{
    Task<IEnumerable<PhotoDto>> GetByEntityAsync(PhotoTargetType type, Guid entityId);
    Task<PhotoDto> AddAsync(PhotoTargetType type, Guid entityId, Guid requestingUserId, CreatePhotoDto dto);
    Task<PhotoDto> UpdateAsync(Guid photoId, Guid requestingUserId, UpdatePhotoDto dto);
    Task DeleteAsync(Guid photoId, Guid requestingUserId);
    Task ReorderAsync(PhotoTargetType type, Guid entityId, Guid requestingUserId, ReorderPhotosDto dto);
}
