using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class Relationship : BaseEntity
{
    public Guid Entity1Id { get; set; }
    public Guid Entity2Id { get; set; }
    public RelationshipType RelationshipType { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }

    public Entity Entity1 { get; set; } = null!;
    public Entity Entity2 { get; set; } = null!;
}
