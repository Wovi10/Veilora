using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class EntityConfiguration : IEntityTypeConfiguration<Entity>
{
    public void Configure(EntityTypeBuilder<Entity> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Type).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Description).HasColumnType("text");
        builder.Property(e => e.FirstName).HasMaxLength(100);
        builder.Property(e => e.LastName).HasMaxLength(100);
        builder.Property(e => e.MiddleName).HasMaxLength(100);
        builder.Property(e => e.MaidenName).HasMaxLength(100);
        builder.Property(e => e.Species).HasMaxLength(100);
        builder.Property(e => e.BirthDate).HasColumnType("date");
        builder.Property(e => e.DeathDate).HasColumnType("date");
        builder.Property(e => e.BirthPlace).HasMaxLength(200);
        builder.Property(e => e.Residence).HasMaxLength(200);
        builder.Property(e => e.Gender).HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.Biography).HasColumnType("text");
        builder.Property(e => e.ProfilePhotoUrl).HasMaxLength(500);

        builder.HasOne(e => e.World)
            .WithMany(w => w.Entities)
            .HasForeignKey(e => e.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Parent1)
            .WithMany(e => e.ChildrenAsParent1)
            .HasForeignKey(e => e.Parent1Id)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Parent2)
            .WithMany(e => e.ChildrenAsParent2)
            .HasForeignKey(e => e.Parent2Id)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.WorldId);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Name);
    }
}
