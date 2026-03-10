using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? DisplayName { get; set; }

    // Navigation properties
    public ICollection<Tree> CreatedTrees { get; set; } = [];
    public ICollection<TreePermission> TreePermissions { get; set; } = [];
}