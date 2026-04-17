using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class EntityLanguageConfiguration : IEntityTypeConfiguration<EntityLanguage>
{
    public void Configure(EntityTypeBuilder<EntityLanguage> builder)
    {
        builder.HasKey(el => new { el.CharacterId, el.LanguageId });

        builder.HasOne(el => el.Character)
            .WithMany(c => c.Languages)
            .HasForeignKey(el => el.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(el => el.Language)
            .WithMany(l => l.EntityLanguages)
            .HasForeignKey(el => el.LanguageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(el => el.CharacterId);
        builder.HasIndex(el => el.LanguageId);
    }
}
