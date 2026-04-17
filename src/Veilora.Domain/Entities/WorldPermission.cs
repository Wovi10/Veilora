using Veilora.Domain.Common;

namespace Veilora.Domain.Entities;

public class WorldPermission : BaseEntity
{
    public Guid WorldId { get; set; }
    public Guid UserId { get; set; }
    public bool CanEdit { get; set; }

    public World World { get; set; } = null!;
    public User User { get; set; } = null!;
}
