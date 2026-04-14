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

        builder.HasOne(e => e.World)
            .WithMany(w => w.Entities)
            .HasForeignKey(e => e.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.WorldId);
        builder.HasIndex(e => e.Type);
        builder.HasIndex(e => e.Name);
    }
}
