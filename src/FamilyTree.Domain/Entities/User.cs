using FamilyTree.Domain.Common;

namespace FamilyTree.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string? DisplayName { get; set; }

    public ICollection<FamilyTree> CreatedFamilyTrees { get; set; } = [];
    public ICollection<World> CreatedWorlds { get; set; } = [];
    public ICollection<TreePermission> TreePermissions { get; set; } = [];
    public ICollection<WorldPermission> WorldPermissions { get; set; } = [];
}
