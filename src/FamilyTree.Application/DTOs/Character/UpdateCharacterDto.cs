namespace FamilyTree.Application.DTOs.Character;

public record UpdateCharacterDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? MaidenName { get; init; }
    public string? Species { get; init; }
    public DateOnly? BirthDate { get; init; }
    public Guid? BirthDateSuffixId { get; init; }
    public DateOnly? DeathDate { get; init; }
    public Guid? DeathDateSuffixId { get; init; }
    public Guid? BirthPlaceLocationId { get; init; }
    public string? BirthPlaceName { get; init; }
    public Guid? DeathPlaceLocationId { get; init; }
    public string? DeathPlaceName { get; init; }
    public string? Residence { get; init; }
    public string? Gender { get; init; }
    public string? Biography { get; init; }
    public string? ProfilePhotoUrl { get; init; }
    public string? OtherNames { get; init; }
    public string? Position { get; init; }
    public string? Height { get; init; }
    public string? HairColour { get; init; }
    public Guid? Parent1Id { get; init; }
    public Guid? Parent2Id { get; init; }

    public IReadOnlyList<Guid> LocationIds { get; init; } = [];
    public IReadOnlyList<string> LocationNames { get; init; } = [];
    public IReadOnlyList<Guid> AffiliationIds { get; init; } = [];
    public IReadOnlyList<string> AffiliationNames { get; init; } = [];
    public IReadOnlyList<Guid> LanguageIds { get; init; } = [];
    public IReadOnlyList<Guid> SpouseIds { get; init; } = [];
    public IReadOnlyList<Guid> ChildIds { get; init; } = [];
}
