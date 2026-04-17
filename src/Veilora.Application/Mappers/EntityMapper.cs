using Veilora.Application.DTOs.Entity;
using Veilora.Domain.Entities;
using Veilora.Domain.Enums;

namespace Veilora.Application.Mappers;

public static class EntityMapper
{
    public static EntityDto ToDto(Entity entity) => new()
    {
        Id = entity.Id,
        Name = entity.Name,
        Type = entity.Type.ToString(),
        WorldId = entity.WorldId,
        Description = entity.Description,
        CreatedAt = entity.CreatedAt,
        UpdatedAt = entity.UpdatedAt
    };

    public static Entity ToEntity(CreateEntityDto dto) => new()
    {
        Name = dto.Name,
        Type = Enum.Parse<EntityType>(dto.Type),
        WorldId = dto.WorldId,
        Description = dto.Description
    };

    public static void UpdateEntity(UpdateEntityDto dto, Entity entity)
    {
        entity.Name = dto.Name;
        entity.Type = Enum.Parse<EntityType>(dto.Type);
        entity.Description = dto.Description;
    }
}
