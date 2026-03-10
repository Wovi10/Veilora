using FamilyTree.Domain.Entities;

namespace FamilyTree.Infrastructure.Repositories.Interfaces;

public interface IRelationshipRepository : IRepository<Relationship>
{
    Task<IEnumerable<Relationship>> GetRelationshipsByTreeIdAsync(Guid treeId);
    Task<IEnumerable<Relationship>> GetPersonRelationshipsAsync(Guid personId);
}

