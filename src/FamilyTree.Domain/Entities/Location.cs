using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class Location : BaseEntity
{
    public required string Name { get; set; }
    public Guid WorldId { get; set; }
    public string? Description { get; set; }

    public World World { get; set; } = null!;
}
