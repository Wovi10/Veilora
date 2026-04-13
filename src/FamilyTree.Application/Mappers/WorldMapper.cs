using FamilyTree.Application.DTOs.World;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Mappers;

public static class WorldMapper
{
    public static WorldDto ToDto(World world) => new()
    {
        Id = world.Id,
        Name = world.Name,
        Description = world.Description,
        CreatedAt = world.CreatedAt,
        UpdatedAt = world.UpdatedAt
    };

    public static World ToEntity(CreateWorldDto dto) => new()
    {
        Name = dto.Name,
        Description = dto.Description
    };

    public static void UpdateEntity(UpdateWorldDto dto, World world)
    {
        world.Name = dto.Name;
        world.Description = dto.Description;
    }
}
