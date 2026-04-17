namespace Veilora.Application.DTOs.Relationship;

public record RelationshipDto
{
    public required Guid Id { get; init; }
    public required Guid Character1Id { get; init; }
    public required Guid Character2Id { get; init; }
    public required string RelationshipType { get; init; }
    public DateOnly? StartDate { get; init; }
    public DateOnly? EndDate { get; init; }
    public string? Notes { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
