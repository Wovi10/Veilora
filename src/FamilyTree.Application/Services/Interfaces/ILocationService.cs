using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTree.Application.DTOs.Location;

namespace FamilyTree.Application.Services.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetByWorldIdAsync(Guid worldId);
    Task<PagedResult<LocationDto>> GetPagedAsync(LocationCriteria criteria);
    Task<LocationDto?> GetByIdAsync(Guid id);
    Task<LocationDto> CreateAsync(CreateLocationDto dto);
    Task<LocationDto> UpdateAsync(Guid id, UpdateLocationDto dto);
    Task DeleteAsync(Guid id);
}
