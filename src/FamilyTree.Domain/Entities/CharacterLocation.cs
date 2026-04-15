namespace FamilyTree.Domain.Entities;

public class CharacterLocation
{
    public Guid CharacterId { get; set; }
    public Guid LocationId { get; set; }

    public Character Character { get; set; } = null!;
    public Location Location { get; set; } = null!;
}
