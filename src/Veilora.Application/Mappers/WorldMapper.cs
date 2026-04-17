using Veilora.Application.DTOs.World;
using Veilora.Domain.Entities;

namespace Veilora.Application.Mappers;

public static class WorldMapper
{
    public static WorldDto ToDto(World world) => new()
    {
        Id = world.Id,
        Name = world.Name,
        Author = world.Author,
        Description = world.Description,
        CreatedById = world.CreatedById,
        CreatedAt = world.CreatedAt,
        UpdatedAt = world.UpdatedAt
    };

    public static World ToEntity(CreateWorldDto dto, Guid? createdById = null) => new()
    {
        Name = dto.Name,
        Author = dto.Author,
        Description = dto.Description,
        CreatedById = createdById
    };

    public static void UpdateEntity(UpdateWorldDto dto, World world)
    {
        world.Name = dto.Name;
        world.Author = dto.Author;
        world.Description = dto.Description;
    }
}
