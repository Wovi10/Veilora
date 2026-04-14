namespace FamilyTree.Domain.Entities;

public class EntityAffiliation
{
    public Guid CharacterId { get; set; }
    public Guid GroupId { get; set; }

    public Entity Character { get; set; } = null!;
    public Entity Group { get; set; } = null!;
}
