namespace FamilyTree.Application.DTOs.Person;

public record PersonDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = string.Empty;
    public string? MaidenName { get; init; }
    public DateTime? BirthDate { get; init; }
    public DateTime? DeathDate { get; init; }
    public string? BirthPlace { get; init; }
    public string? Residence { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string? Biography { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}