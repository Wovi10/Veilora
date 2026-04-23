using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class ReadingNoteConfiguration : IEntityTypeConfiguration<ReadingNote>
{
    public void Configure(EntityTypeBuilder<ReadingNote> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Text).IsRequired().HasColumnType("text");
        builder.Property(n => n.Tags).HasColumnType("text[]");

        builder.HasIndex(n => n.SessionId);
        builder.HasIndex(n => n.UserId);
    }
}
