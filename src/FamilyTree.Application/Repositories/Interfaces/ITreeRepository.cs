using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface ITreeRepository : IRepository<Tree>
{
    Task<Tree?> GetTreeWithPersonsAsync(Guid treeId);
    Task AddPersonToTreeAsync(Guid treeId, Guid personId);
    Task RemovePersonFromTreeAsync(Guid treeId, Guid personId);
    Task<bool> IsPersonInTreeAsync(Guid treeId, Guid personId);
    Task UpdatePersonPositionAsync(Guid treeId, Guid personId, double x, double y);
}

