using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class EntityFamilyTreeConfiguration : IEntityTypeConfiguration<EntityFamilyTree>
{
    public void Configure(EntityTypeBuilder<EntityFamilyTree> builder)
    {
        builder.HasKey(eft => new { eft.EntityId, eft.FamilyTreeId });

        builder.HasOne(eft => eft.Entity)
            .WithMany(e => e.EntityFamilyTrees)
            .HasForeignKey(eft => eft.EntityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(eft => eft.FamilyTree)
            .WithMany(ft => ft.EntityFamilyTrees)
            .HasForeignKey(eft => eft.FamilyTreeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(eft => eft.EntityId);
        builder.HasIndex(eft => eft.FamilyTreeId);
    }
}
