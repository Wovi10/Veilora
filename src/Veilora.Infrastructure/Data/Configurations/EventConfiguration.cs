using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class EventConfiguration : IEntityTypeConfiguration<Event>
{
    public void Configure(EntityTypeBuilder<Event> builder)
    {
        builder.ToTable("Events");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasColumnType("text");

        builder.HasOne(e => e.World)
            .WithMany()
            .HasForeignKey(e => e.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.WorldId);
    }
}
