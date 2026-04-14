namespace FamilyTree.Domain.Entities;

public class EntityLocation
{
    public Guid CharacterId { get; set; }
    public Guid PlaceId { get; set; }

    public Entity Character { get; set; } = null!;
    public Entity Place { get; set; } = null!;
}
