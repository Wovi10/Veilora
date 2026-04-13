using FamilyTree.Application.DTOs.World;

namespace FamilyTree.Application.Services.Interfaces;

public interface IWorldService
{
    Task<IEnumerable<WorldDto>> GetAllByUserAsync(Guid userId);
    Task<WorldDto?> GetByIdAsync(Guid id);
    Task<WorldDto> CreateAsync(CreateWorldDto dto, Guid? createdById = null);
    Task<WorldDto> UpdateAsync(Guid id, UpdateWorldDto dto);
    Task DeleteAsync(Guid id);
    Task<bool> TransferOwnershipAsync(Guid id, string newOwnerEmail);
}
