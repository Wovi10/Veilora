using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class DateSuffix : BaseEntity
{
    public required string Name { get; set; }
    public required string Abbreviation { get; set; }
    public long AnchorYear { get; set; }
    public decimal Scale { get; set; } = 1m;
    public bool IsReversed { get; set; }
    public bool IsDefault { get; set; }
    public Guid WorldId { get; set; }

    public World World { get; set; } = null!;
}
