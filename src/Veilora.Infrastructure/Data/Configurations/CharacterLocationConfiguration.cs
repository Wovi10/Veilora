using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class CharacterLocationConfiguration : IEntityTypeConfiguration<CharacterLocation>
{
    public void Configure(EntityTypeBuilder<CharacterLocation> builder)
    {
        builder.HasKey(cl => new { cl.CharacterId, cl.LocationId });

        builder.HasOne(cl => cl.Character)
            .WithMany(c => c.Locations)
            .HasForeignKey(cl => cl.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cl => cl.Location)
            .WithMany()
            .HasForeignKey(cl => cl.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cl => cl.CharacterId);
        builder.HasIndex(cl => cl.LocationId);
    }
}
