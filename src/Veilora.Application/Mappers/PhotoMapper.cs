using Veilora.Application.DTOs.Photo;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;

namespace Veilora.Application.Mappers;

public static class PhotoMapper
{
    public static PhotoDto ToDto(Photo photo) => new(
        photo.Id, photo.EntityId, photo.EntityType.ToString(),
        photo.Url, photo.DisplayOrder, photo.CreatedAt, photo.UpdatedAt);

    public static Photo ToEntity(CreatePhotoDto dto, PhotoTargetType type, Guid entityId, int displayOrder) => new()
    {
        EntityId = entityId,
        EntityType = type,
        Url = dto.Url,
        DisplayOrder = displayOrder
    };
}
