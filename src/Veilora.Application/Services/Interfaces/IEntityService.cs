using Veilora.Application.Common;
using Veilora.Application.Criteria;
using Veilora.Application.DTOs.Entity;

namespace Veilora.Application.Services.Interfaces;

public interface IEntityService
{
    Task<IEnumerable<EntityDto>> GetAllAsync();
    Task<EntityDto?> GetByIdAsync(Guid id);
    Task<IEnumerable<EntityDto>> GetByWorldIdAsync(Guid worldId);
    Task<IEnumerable<EntityDto>> GetByWorldIdAndTypeAsync(Guid worldId, string type);
    Task<PagedResult<EntityDto>> GetPagedAsync(EntityCriteria criteria);
    Task<EntityDto> CreateAsync(CreateEntityDto dto);
    Task<EntityDto> UpdateAsync(Guid id, UpdateEntityDto dto);
    Task DeleteAsync(Guid id);
    Task<IEnumerable<EntityDto>> SearchAsync(string searchTerm);
}
