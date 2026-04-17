using Veilora.Domain.Common;
using Veilora.Domain.Enums;

namespace Veilora.Domain.Entities;

public class Relationship : BaseEntity
{
    public Guid Character1Id { get; set; }
    public Guid Character2Id { get; set; }
    public RelationshipType RelationshipType { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string? Notes { get; set; }

    public Character Character1 { get; set; } = null!;
    public Character Character2 { get; set; } = null!;
}
