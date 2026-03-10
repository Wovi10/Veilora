using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.RelationshipType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(r => r.Notes)
            .HasColumnType("text");

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.UpdatedAt)
            .IsRequired();

        // Foreign key relationships
        builder.HasOne(r => r.Person1)
            .WithMany(p => p.RelationshipsAsPerson1)
            .HasForeignKey(r => r.Person1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Person2)
            .WithMany(p => p.RelationshipsAsPerson2)
            .HasForeignKey(r => r.Person2Id)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(r => r.Person1Id);
        builder.HasIndex(r => r.Person2Id);
        builder.HasIndex(r => r.RelationshipType);
    }
}