using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class DateSuffix : BaseEntity
{
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
    public int Order { get; set; }
    public bool IsDefault { get; set; }
    public Guid WorldId { get; set; }

    public World World { get; set; } = null!;
}
