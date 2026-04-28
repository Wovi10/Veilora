using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class Event : BaseEntity
{
    public required string Name { get; set; }
    public Guid WorldId { get; set; }
    public string? Description { get; set; }

    public World World { get; set; } = null!;
}
