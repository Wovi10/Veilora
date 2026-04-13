namespace FamilyTree.Domain.Entities;

public class EntityFamilyTree
{
    public Guid EntityId { get; set; }
    public Guid FamilyTreeId { get; set; }
    public double? PositionX { get; set; }
    public double? PositionY { get; set; }

    public Entity Entity { get; set; } = null!;
    public FamilyTree FamilyTree { get; set; } = null!;
}
