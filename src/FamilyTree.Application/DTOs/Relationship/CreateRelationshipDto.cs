namespace FamilyTree.Application.DTOs.Relationship;

public record CreateRelationshipDto
{
    public Guid Person1Id { get; init; }
    public Guid Person2Id { get; init; }
    public string RelationshipType { get; init; } = string.Empty;
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public string? Notes { get; init; }
}