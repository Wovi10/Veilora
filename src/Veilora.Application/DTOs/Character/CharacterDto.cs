using Veilora.Application.DTOs.Entity;
using Veilora.Application.DTOs.Language;

namespace Veilora.Application.DTOs.Character;

public record CharacterDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required Guid WorldId { get; init; }
    public string? Description { get; init; }

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }
    public string? MaidenName { get; init; }
    public string? Species { get; init; }
    public DateOnly? BirthDate { get; init; }
    public Guid? BirthDateSuffixId { get; init; }
    public string? BirthDateSuffixAbbreviation { get; init; }
    public DateOnly? DeathDate { get; init; }
    public Guid? DeathDateSuffixId { get; init; }
    public string? DeathDateSuffixAbbreviation { get; init; }
    public Guid? BirthPlaceLocationId { get; init; }
    public string? BirthPlaceLocationName { get; init; }
    public Guid? DeathPlaceLocationId { get; init; }
    public string? DeathPlaceLocationName { get; init; }
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

    public IReadOnlyList<EntityRefDto> Locations { get; init; } = [];
    public IReadOnlyList<EntityRefDto> Affiliations { get; init; } = [];
    public IReadOnlyList<LanguageDto> Languages { get; init; } = [];
    public IReadOnlyList<EntityRefDto> Spouses { get; init; } = [];
    public IReadOnlyList<EntityRefDto> Children { get; init; } = [];

    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}
