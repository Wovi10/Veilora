using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class Entity : BaseEntity
{
    public required string Name { get; set; }
    public EntityType Type { get; set; }
    public Guid WorldId { get; set; }
    public string? Description { get; set; }

    // Navigation
    public World World { get; set; } = null!;
    public ICollection<Note> Notes { get; set; } = [];
}
