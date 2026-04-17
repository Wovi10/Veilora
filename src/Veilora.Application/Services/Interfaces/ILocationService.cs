using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Location;

namespace Veilora.Application.Services.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<LocationDto>> GetPagedAsync(LocationCriteria criteria);
    Task<LocationDto?> GetByIdAsync(Guid id);
    Task<LocationDto> CreateAsync(CreateLocationDto dto);
    Task<LocationDto> UpdateAsync(Guid id, UpdateLocationDto dto);
    Task DeleteAsync(Guid id);
}
