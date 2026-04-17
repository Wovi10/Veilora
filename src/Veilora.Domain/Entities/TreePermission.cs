using Veilora.Domain.Common;
using Veilora.Domain.Enums;

namespace Veilora.Domain.Entities;

public class TreePermission : BaseEntity
{
    public Guid TreeId { get; set; }
    public Guid UserId { get; set; }
    public PermissionLevel PermissionLevel { get; set; }
    public DateTime GrantedAt { get; set; }

    // Navigation properties
    public FamilyTree Tree { get; set; } = null!;
    public User User { get; set; } = null!;
}
