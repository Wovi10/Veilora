namespace FamilyTree.Domain.Entities;

public class CharacterFamilyTree
{
    public Guid CharacterId { get; set; }
    public Guid FamilyTreeId { get; set; }
    public double? PositionX { get; set; }
    public double? PositionY { get; set; }

    public Character Character { get; set; } = null!;
    public FamilyTree FamilyTree { get; set; } = null!;
}
