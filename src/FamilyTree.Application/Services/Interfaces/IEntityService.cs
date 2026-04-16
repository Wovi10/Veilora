using FamilyTree.Application.Common;
using FamilyTree.Application.Criteria;
using FamilyTree.Application.DTOs.Entity;

namespace FamilyTree.Application.Services.Interfaces;

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
