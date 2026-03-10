using FamilyTree.Domain.Entities;

namespace FamilyTree.Application.Repositories.Interfaces;

public interface IPersonRepository : IRepository<Person>
{
    Task<IEnumerable<Person>> GetDescendantsAsync(Guid personId);
    Task<IEnumerable<Person>> GetAncestorsAsync(Guid personId);
    Task<IEnumerable<Person>> GetPersonsByTreeIdAsync(Guid treeId);
    Task<IEnumerable<Person>> SearchAsync(string searchTerm);
}