using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class ReadingSessionConfiguration : IEntityTypeConfiguration<ReadingSession>
{
    public void Configure(EntityTypeBuilder<ReadingSession> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasOne(s => s.World)
            .WithMany()
            .HasForeignKey(s => s.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Notes)
            .WithOne(n => n.Session)
            .HasForeignKey(n => n.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => new { s.UserId, s.EndedAt });
    }
}
