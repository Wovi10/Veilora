using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string? MaidenName { get; set; }
    public DateOnly? BirthDate { get; set; }
    public DateOnly? DeathDate { get; set; }
    public string? BirthPlace { get; set; }
    public string? Residence { get; set; }
    public Gender Gender { get; set; }
    public string? Biography { get; set; }
    public string? ProfilePhotoUrl { get; set; }

    public Guid? Parent1Id { get; set; }
    public Guid? Parent2Id { get; set; }

    // Navigation properties
    public Person? Parent1 { get; set; }
    public Person? Parent2 { get; set; }
    public ICollection<Person> ChildrenAsParent1 { get; set; } = [];
    public ICollection<Person> ChildrenAsParent2 { get; set; } = [];
    public ICollection<PersonTree> PersonTrees { get; set; } = [];
    public ICollection<Relationship> RelationshipsAsPerson1 { get; set; } = [];
    public ICollection<Relationship> RelationshipsAsPerson2 { get; set; } = [];
}