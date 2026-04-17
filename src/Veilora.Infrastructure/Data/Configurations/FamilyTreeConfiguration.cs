using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using FamilyTreeEntity = Veilora.Domain.Entities.FamilyTree;

namespace Veilora.Infrastructure.Data.Configurations;

public class FamilyTreeConfiguration : IEntityTypeConfiguration<FamilyTreeEntity>
{
    public void Configure(EntityTypeBuilder<FamilyTreeEntity> builder)
    {
        builder.HasKey(ft => ft.Id);
        builder.Property(ft => ft.Name).IsRequired().HasMaxLength(200);
        builder.Property(ft => ft.Description).HasColumnType("text");

        builder.HasOne(ft => ft.World)
            .WithMany(w => w.FamilyTrees)
            .HasForeignKey(ft => ft.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ft => ft.Creator)
            .WithMany(u => u.CreatedFamilyTrees)
            .HasForeignKey(ft => ft.CreatedBy)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(ft => ft.WorldId);
        builder.HasIndex(ft => ft.Name);
    }
}
