using FamilyTree.Domain.Common;
using FamilyTree.Domain.Enums;

namespace FamilyTree.Domain.Entities;

public class TreePermission : BaseEntity
{
    public Guid TreeId { get; set; }
    public Guid UserId { get; set; }
    public PermissionLevel PermissionLevel { get; set; }
    public DateTime GrantedAt { get; set; }

    // Navigation properties
    public Tree Tree { get; set; } = null!;
    public User User { get; set; } = null!;
}