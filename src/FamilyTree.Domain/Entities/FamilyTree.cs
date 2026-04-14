using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class FamilyTree : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid WorldId { get; set; }
    public Guid? CreatedBy { get; set; }

    public World World { get; set; } = null!;
    public User? Creator { get; set; }
    public ICollection<CharacterFamilyTree> CharacterFamilyTrees { get; set; } = [];
}
