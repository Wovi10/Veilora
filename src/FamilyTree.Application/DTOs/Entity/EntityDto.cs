namespace FamilyTree.Application.DTOs.Entity;

public record EntityDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; init; }
    public required Guid WorldId { get; init; }
    public string? Description { get; init; }

    // Character-specific
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? MaidenName { get; init; }
    public string? Species { get; init; }
    public DateOnly? BirthDate { get; init; }
    public DateOnly? DeathDate { get; init; }
    public string? BirthPlace { get; init; }
    public string? Residence { get; init; }
    public string? Gender { get; init; }
    public string? Biography { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public Guid? Parent1Id { get; init; }
    public Guid? Parent2Id { get; init; }

    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
