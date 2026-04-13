using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class World : BaseEntity
{
    public required string Name { get; set; }
    public string? Author { get; set; }
    public string? Description { get; set; }
    public Guid? CreatedById { get; set; }

    public User? CreatedBy { get; set; }
    public ICollection<Entity> Entities { get; set; } = [];
    public ICollection<FamilyTree> FamilyTrees { get; set; } = [];
    public ICollection<Note> Notes { get; set; } = [];
    public ICollection<WorldPermission> Permissions { get; set; } = [];
}
