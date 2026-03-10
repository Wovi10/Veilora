using FamilyTree.Domain.Entities;

namespace FamilyTree.Infrastructure.Repositories.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<IEnumerable<Person>> SearchAsync(string searchTerm);
    Task<IEnumerable<Person>> GetPersonsByTreeIdAsync(Guid treeId);
    Task<IEnumerable<Person>> GetAncestorsAsync(Guid personId);
    Task<IEnumerable<Person>> GetDescendantsAsync(Guid personId);
}

