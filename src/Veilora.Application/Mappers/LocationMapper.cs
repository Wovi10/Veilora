using Veilora.Application.DTOs.Location;
using Veilora.Domain.Entities;

namespace Veilora.Application.Mappers;

public static class LocationMapper
{
    public static LocationDto ToDto(Location location) => new()
    {
        Id = location.Id,
        Name = location.Name,
        WorldId = location.WorldId,
        Description = location.Description,
        CreatedAt = location.CreatedAt,
        UpdatedAt = location.UpdatedAt
    };

    public static Location ToEntity(CreateLocationDto dto) => new()
    {
        Name = dto.Name,
        WorldId = dto.WorldId,
        Description = dto.Description
    };

    public static void UpdateEntity(UpdateLocationDto dto, Location location)
    {
        location.Name = dto.Name;
        location.Description = dto.Description;
    }
}
