using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class EntityLocationConfiguration : IEntityTypeConfiguration<EntityLocation>
{
    public void Configure(EntityTypeBuilder<EntityLocation> builder)
    {
        builder.HasKey(el => new { el.CharacterId, el.PlaceId });

        builder.HasOne(el => el.Character)
            .WithMany(e => e.Locations)
            .HasForeignKey(el => el.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(el => el.Place)
            .WithMany(e => e.CharactersLocatedHere)
            .HasForeignKey(el => el.PlaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(el => el.CharacterId);
        builder.HasIndex(el => el.PlaceId);
    }
}
