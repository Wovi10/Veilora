using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class TreePermissionConfiguration : IEntityTypeConfiguration<TreePermission>
{
    public void Configure(EntityTypeBuilder<TreePermission> builder)
    {
        builder.HasKey(tp => tp.Id);

        builder.Property(tp => tp.PermissionLevel)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(tp => tp.GrantedAt)
            .IsRequired();

        builder.Property(tp => tp.CreatedAt)
            .IsRequired();

        builder.Property(tp => tp.UpdatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(tp => tp.Tree)
            .WithMany()
            .HasForeignKey(tp => tp.TreeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(tp => tp.User)
            .WithMany(u => u.TreePermissions)
            .HasForeignKey(tp => tp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(tp => tp.TreeId);
        builder.HasIndex(tp => tp.UserId);
        builder.HasIndex(tp => new { tp.TreeId, tp.UserId })
            .IsUnique();
    }
}