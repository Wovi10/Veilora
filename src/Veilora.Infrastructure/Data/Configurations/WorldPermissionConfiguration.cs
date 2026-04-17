using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class WorldPermissionConfiguration : IEntityTypeConfiguration<WorldPermission>
{
    public void Configure(EntityTypeBuilder<WorldPermission> builder)
    {
        builder.HasKey(wp => wp.Id);

        builder.Property(wp => wp.CanEdit).IsRequired();

        builder.HasOne(wp => wp.World)
            .WithMany(w => w.Permissions)
            .HasForeignKey(wp => wp.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wp => wp.User)
            .WithMany(u => u.WorldPermissions)
            .HasForeignKey(wp => wp.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(wp => new { wp.WorldId, wp.UserId }).IsUnique();
    }
}
