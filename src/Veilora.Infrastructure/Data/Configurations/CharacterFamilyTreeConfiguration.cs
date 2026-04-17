using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class CharacterFamilyTreeConfiguration : IEntityTypeConfiguration<CharacterFamilyTree>
{
    public void Configure(EntityTypeBuilder<CharacterFamilyTree> builder)
    {
        builder.ToTable("CharacterFamilyTrees");
        builder.HasKey(cft => new { cft.CharacterId, cft.FamilyTreeId });

        builder.HasOne(cft => cft.Character)
            .WithMany(c => c.CharacterFamilyTrees)
            .HasForeignKey(cft => cft.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(cft => cft.FamilyTree)
            .WithMany(ft => ft.CharacterFamilyTrees)
            .HasForeignKey(cft => cft.FamilyTreeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cft => cft.CharacterId);
        builder.HasIndex(cft => cft.FamilyTreeId);
    }
}
