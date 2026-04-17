using Veilora.Domain.Entities;

namespace Veilora.Application.Repositories.Interfaces;

public interface IRelationshipRepository : IRepository<Relationship>
{
    Task<IEnumerable<Relationship>> GetRelationshipsByFamilyTreeIdAsync(Guid familyTreeId);
    Task<IEnumerable<Relationship>> GetEntityRelationshipsAsync(Guid entityId);
}
