namespace FamilyTree.Application.DTOs.World;

public record WorldDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public string? Author { get; init; }
    public string? Description { get; init; }
    public Guid? CreatedById { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
