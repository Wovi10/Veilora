using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class NoteConfiguration : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Content).IsRequired().HasColumnType("text");

        builder.HasOne(n => n.World)
            .WithMany(w => w.Notes)
            .HasForeignKey(n => n.WorldId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Entity)
            .WithMany(e => e.Notes)
            .HasForeignKey(n => n.EntityId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(n => n.WorldId);
        builder.HasIndex(n => n.EntityId);
    }
}
