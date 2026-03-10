using FamilyTree.Application.DTOs.Tree;

namespace FamilyTree.Application.Services.Interfaces;

public interface ITreeService
{
    Task<IEnumerable<TreeDto>> GetAllAsync();
    Task<TreeDto?> GetByIdAsync(Guid id);
    Task<TreeWithPersonsDto?> GetTreeWithPersonsAsync(Guid id);
    Task<TreeDto> CreateAsync(CreateTreeDto dto);
    Task<TreeDto> UpdateAsync(Guid id, UpdateTreeDto dto);
    Task DeleteAsync(Guid id);
    Task AddPersonToTreeAsync(Guid treeId, Guid personId);
    Task RemovePersonFromTreeAsync(Guid treeId, Guid personId);
}