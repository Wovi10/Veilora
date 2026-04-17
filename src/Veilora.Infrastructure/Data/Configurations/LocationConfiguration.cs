using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(200);
        builder.Property(l => l.Description).HasColumnType("text");

        builder.HasOne(l => l.World)
            .WithMany()
            .HasForeignKey(l => l.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.WorldId);
    }
}
