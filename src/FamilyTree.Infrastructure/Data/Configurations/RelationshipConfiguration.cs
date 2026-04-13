using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RelationshipType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(r => r.Notes).HasColumnType("text");

        builder.HasOne(r => r.Entity1)
            .WithMany(e => e.RelationshipsAsEntity1)
            .HasForeignKey(r => r.Entity1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Entity2)
            .WithMany(e => e.RelationshipsAsEntity2)
            .HasForeignKey(r => r.Entity2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.Entity1Id);
        builder.HasIndex(r => r.Entity2Id);
        builder.HasIndex(r => r.RelationshipType);
    }
}
