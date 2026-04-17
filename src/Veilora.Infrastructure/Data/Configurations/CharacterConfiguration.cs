using Veilora.Domain.Entities;
using Veilora.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("Characters");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Name).IsRequired().HasMaxLength(200);
        builder.Property(c => c.Description).HasColumnType("text");
        builder.Property(c => c.FirstName).HasMaxLength(100);
        builder.Property(c => c.LastName).HasMaxLength(100);
        builder.Property(c => c.MiddleName).HasMaxLength(100);
        builder.Property(c => c.MaidenName).HasMaxLength(100);
        builder.Property(c => c.Species).HasMaxLength(100);
        builder.Property(c => c.Gender).HasConversion<string>().HasMaxLength(20);
        builder.Property(c => c.BirthDate).HasColumnType("date");
        builder.Property(c => c.DeathDate).HasColumnType("date");
        builder.Property(c => c.Residence).HasMaxLength(200);
        builder.Property(c => c.Biography).HasColumnType("text");
        builder.Property(c => c.ProfilePhotoUrl).HasMaxLength(500);
        builder.Property(c => c.OtherNames).HasMaxLength(500);
        builder.Property(c => c.Position).HasMaxLength(200);
        builder.Property(c => c.Height).HasMaxLength(100);
        builder.Property(c => c.HairColour).HasMaxLength(100);

        builder.HasOne(c => c.World)
            .WithMany()
            .HasForeignKey(c => c.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Parent1)
            .WithMany(c => c.ChildrenAsParent1)
            .HasForeignKey(c => c.Parent1Id)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.Parent2)
            .WithMany(c => c.ChildrenAsParent2)
            .HasForeignKey(c => c.Parent2Id)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.BirthDateSuffix)
            .WithMany()
            .HasForeignKey(c => c.BirthDateSuffixId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.DeathDateSuffix)
            .WithMany()
            .HasForeignKey(c => c.DeathDateSuffixId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.BirthPlaceLocation)
            .WithMany()
            .HasForeignKey(c => c.BirthPlaceLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(c => c.DeathPlaceLocation)
            .WithMany()
            .HasForeignKey(c => c.DeathPlaceLocationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(c => c.WorldId);
        builder.HasIndex(c => c.Name);
    }
}
