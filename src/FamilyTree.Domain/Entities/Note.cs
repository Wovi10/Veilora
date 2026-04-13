using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class Note : BaseEntity
{
    public required string Content { get; set; }
    public Guid? WorldId { get; set; }
    public Guid? EntityId { get; set; }

    public World? World { get; set; }
    public Entity? Entity { get; set; }
}
