using FamilyTree.Application.DTOs.Location;
using FamilyTree.Application.Exceptions;
using FamilyTree.Application.Mappers;
using FamilyTree.Application.Repositories.Interfaces;
using FamilyTree.Application.Services.Interfaces;
using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Services;

public class LocationService(
    ILocationRepository locationRepository,
    IWorldRepository worldRepository) : ILocationService
{
    public async Task<IEnumerable<LocationDto>> GetByWorldIdAsync(Guid worldId)
    {
        var locations = await locationRepository.GetByWorldIdAsync(worldId);
        return locations.Select(LocationMapper.ToDto);
    }

    public async Task<LocationDto?> GetByIdAsync(Guid id)
    {
        var location = await locationRepository.GetByIdAsync(id);
        return location is null ? null : LocationMapper.ToDto(location);
    }

    public async Task<LocationDto> CreateAsync(CreateLocationDto dto)
    {
        _ = await worldRepository.GetByIdAsync(dto.WorldId)
            ?? throw new NotFoundException(nameof(Location), dto.WorldId);

        var location = LocationMapper.ToEntity(dto);
        await locationRepository.AddAsync(location);
        await locationRepository.SaveChangesAsync();
        return LocationMapper.ToDto(location);
    }

    public async Task<LocationDto> UpdateAsync(Guid id, UpdateLocationDto dto)
    {
        var location = await locationRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Location), id);

        LocationMapper.UpdateEntity(dto, location);
        await locationRepository.UpdateAsync(location);
        await locationRepository.SaveChangesAsync();
        return LocationMapper.ToDto(location);
    }

    public async Task DeleteAsync(Guid id)
    {
        var location = await locationRepository.GetByIdAsync(id)
            ?? throw new NotFoundException(nameof(Location), id);
        await locationRepository.DeleteAsync(location);
        await locationRepository.SaveChangesAsync();
    }
}
