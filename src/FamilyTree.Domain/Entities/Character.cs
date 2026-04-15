using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class Character : BaseEntity
{
    public required string Name { get; set; }
    public Guid WorldId { get; set; }
    public string? Description { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? MaidenName { get; set; }
    public string? Species { get; set; }
    public Gender? Gender { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? BirthDateSuffix { get; set; }
    public DateOnly? DeathDate { get; set; }
    public string? DeathDateSuffix { get; set; }
    public Guid? BirthPlaceLocationId { get; set; }
    public Guid? DeathPlaceLocationId { get; set; }
    public string? Residence { get; set; }
    public string? Biography { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? OtherNames { get; set; }
    public string? Position { get; set; }
    public string? Height { get; set; }
    public string? HairColour { get; set; }

    public Guid? Parent1Id { get; set; }
    public Guid? Parent2Id { get; set; }

    // Navigation
    public World World { get; set; } = null!;
    public Location? BirthPlaceLocation { get; set; }
    public Location? DeathPlaceLocation { get; set; }
    public Character? Parent1 { get; set; }
    public Character? Parent2 { get; set; }
    public ICollection<Character> ChildrenAsParent1 { get; set; } = [];
    public ICollection<Character> ChildrenAsParent2 { get; set; } = [];
    public ICollection<CharacterFamilyTree> CharacterFamilyTrees { get; set; } = [];
    public ICollection<Relationship> RelationshipsAsCharacter1 { get; set; } = [];
    public ICollection<Relationship> RelationshipsAsCharacter2 { get; set; } = [];
    public ICollection<CharacterLocation> Locations { get; set; } = [];
    public ICollection<EntityAffiliation> Affiliations { get; set; } = [];
    public ICollection<EntityLanguage> Languages { get; set; } = [];
}
