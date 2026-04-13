using FamilyTree.Application.DTOs.World;

namespace FamilyTree.Application.Services.Interfaces;

public interface IWorldService
{
    Task<IEnumerable<WorldDto>> GetAllAsync();
    Task<WorldDto?> GetByIdAsync(Guid id);
    Task<WorldDto> CreateAsync(CreateWorldDto dto);
    Task<WorldDto> UpdateAsync(Guid id, UpdateWorldDto dto);
    Task DeleteAsync(Guid id);
}
