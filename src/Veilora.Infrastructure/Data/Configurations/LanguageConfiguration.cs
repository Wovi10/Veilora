using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(100);

        builder.HasOne(l => l.World)
            .WithMany()
            .HasForeignKey(l => l.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.WorldId);
        builder.HasIndex(l => new { l.WorldId, l.Name }).IsUnique();
    }
}
