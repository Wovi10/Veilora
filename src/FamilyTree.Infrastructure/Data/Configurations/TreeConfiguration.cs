using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class TreeConfiguration : IEntityTypeConfiguration<Tree>
{
    public void Configure(EntityTypeBuilder<Tree> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Description)
            .HasColumnType("text");

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired();

        // Index
        builder.HasIndex(t => t.Name);

        // FK to User (nullable - tree can exist without an owner)
        builder.HasOne(t => t.Creator)
            .WithMany(u => u.CreatedTrees)
            .HasForeignKey(t => t.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);
    }
}