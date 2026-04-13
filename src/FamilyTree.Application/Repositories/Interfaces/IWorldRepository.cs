using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IWorldRepository : IRepository<World>
{
    Task<IEnumerable<World>> GetAllByUserAsync(Guid userId);
}
