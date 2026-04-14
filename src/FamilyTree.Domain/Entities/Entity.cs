using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class Entity : BaseEntity
{
    public required string Name { get; set; }
    public EntityType Type { get; set; }
    public Guid WorldId { get; set; }
    public string? Description { get; set; }

    // Character-specific (nullable for other types)
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? MaidenName { get; set; }
    public string? Species { get; set; }
    public DateOnly? BirthDate { get; set; }
    public string? BirthDateSuffix { get; set; }
    public DateOnly? DeathDate { get; set; }
    public string? DeathDateSuffix { get; set; }
    public Guid? BirthPlaceEntityId { get; set; }
    public Guid? DeathPlaceEntityId { get; set; }
    public string? Residence { get; set; }
    public Gender? Gender { get; set; }
    public string? Biography { get; set; }
    public string? ProfilePhotoUrl { get; set; }
    public string? OtherNames { get; set; }
    public string? Position { get; set; }
    public string? Height { get; set; }
    public string? HairColour { get; set; }

    // Parent references (for family trees)
    public Guid? Parent1Id { get; set; }
    public Guid? Parent2Id { get; set; }

    // Navigation
    public World World { get; set; } = null!;
    public Entity? Parent1 { get; set; }
    public Entity? Parent2 { get; set; }
    public Entity? BirthPlaceEntity { get; set; }
    public Entity? DeathPlaceEntity { get; set; }
    public ICollection<Entity> ChildrenAsParent1 { get; set; } = [];
    public ICollection<Entity> ChildrenAsParent2 { get; set; } = [];
    public ICollection<EntityFamilyTree> EntityFamilyTrees { get; set; } = [];
    public ICollection<Relationship> RelationshipsAsEntity1 { get; set; } = [];
    public ICollection<Relationship> RelationshipsAsEntity2 { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<EntityLocation> Locations { get; set; } = [];
    public ICollection<EntityLocation> CharactersLocatedHere { get; set; } = [];
    public ICollection<EntityAffiliation> Affiliations { get; set; } = [];
    public ICollection<EntityAffiliation> CharactersAffiliated { get; set; } = [];
    public ICollection<EntityLanguage> Languages { get; set; } = [];
}
