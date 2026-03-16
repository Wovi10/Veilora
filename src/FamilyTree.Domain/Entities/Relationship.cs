using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class Relationship : BaseEntity
{
    public Guid Person1Id { get; set; }
    public Guid Person2Id { get; set; }
    public RelationshipType RelationshipType { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public Person Person1 { get; set; } = null!;
    public Person Person2 { get; set; } = null!;
}