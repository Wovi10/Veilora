using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class WorldConfiguration : IEntityTypeConfiguration<World>
{
    public void Configure(EntityTypeBuilder<World> builder)
    {
        builder.HasKey(w => w.Id);
        builder.Property(w => w.Name).IsRequired().HasMaxLength(200);
        builder.Property(w => w.Author).HasMaxLength(200);
        builder.Property(w => w.Description).HasColumnType("text");
        builder.HasIndex(w => w.Name);

        builder.HasOne(w => w.CreatedBy)
            .WithMany(u => u.CreatedWorlds)
            .HasForeignKey(w => w.CreatedById)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
