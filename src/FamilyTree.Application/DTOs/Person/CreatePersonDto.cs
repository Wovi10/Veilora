namespace FamilyTree.Application.DTOs.Person;

public record CreatePersonDto
{
    public string FirstName { get; init; } = string.Empty;
    public string? MiddleName { get; init; }
    public string LastName { get; init; } = string.Empty;
    public string? MaidenName { get; init; }
    public DateOnly? BirthDate { get; init; }
    public DateOnly? DeathDate { get; init; }
    public string? BirthPlace { get; init; }
    public string? Residence { get; init; }
    public string Gender { get; init; } = string.Empty;
    public string? Biography { get; init; }
}