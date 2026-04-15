using FamilyTree.Domain.Common;
using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FamilyTreeEntity = FamilyTree.Domain.Entities.FamilyTree;

namespace FamilyTree.Infrastructure.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<World> Worlds => Set<World>();
    public DbSet<Entity> Entities => Set<Entity>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Character> Characters => Set<Character>();
    public DbSet<FamilyTreeEntity> FamilyTrees => Set<FamilyTreeEntity>();
    public DbSet<CharacterFamilyTree> CharacterFamilyTrees => Set<CharacterFamilyTree>();
    public DbSet<Relationship> Relationships => Set<Relationship>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<User> Users => Set<User>();
    public DbSet<TreePermission> TreePermissions => Set<TreePermission>();
    public DbSet<WorldPermission> WorldPermissions => Set<WorldPermission>();
    public DbSet<Language> Languages => Set<Language>();
    public DbSet<CharacterLocation> CharacterLocations => Set<CharacterLocation>();
    public DbSet<EntityAffiliation> EntityAffiliations => Set<EntityAffiliation>();
    public DbSet<EntityLanguage> EntityLanguages => Set<EntityLanguage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;
            if (entry.State is EntityState.Added or EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
        return await base.SaveChangesAsync(cancellationToken);
    }
}
