using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Location;
using Veilora.Application.Exceptions;
using Veilora.Application.Mappers;
using Veilora.Application.Repositories.Interfaces;
using Veilora.Application.Services.Interfaces;
using Veilora.Domain.Entities;

namespace Veilora.Application.Services;

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

    public async Task<PagedResult<LocationDto>> GetPagedAsync(LocationCriteria criteria)
    {
        var paged = await locationRepository.GetPagedAsync(criteria);
        return new PagedResult<LocationDto>(
            paged.Items.Select(LocationMapper.ToDto).ToList(),
            paged.TotalCount, paged.Page, paged.PageSize);
    }
}
