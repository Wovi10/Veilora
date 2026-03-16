using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class Tree : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? CreatedBy { get; set; }

    // Navigation properties
    public User? Creator { get; set; }
    public ICollection<PersonTree> PersonTrees { get; set; } = [];
}