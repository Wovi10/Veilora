using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.MiddleName)
            .HasMaxLength(100);

        builder.Property(p => p.MaidenName)
            .HasMaxLength(100);

        builder.Property(p => p.BirthDate)
            .HasColumnType("date");

        builder.Property(p => p.DeathDate)
            .HasColumnType("date");

        builder.Property(p => p.BirthPlace)
            .HasMaxLength(200);

        builder.Property(p => p.Residence)
            .HasMaxLength(200);

        builder.Property(p => p.Gender)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(p => p.Biography)
            .HasColumnType("text");

        builder.Property(p => p.ProfilePhotoUrl)
            .HasMaxLength(500);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired();

        // Self-referencing parent FKs
        builder.HasOne(p => p.Parent1)
            .WithMany(p => p.ChildrenAsParent1)
            .HasForeignKey(p => p.Parent1Id)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Parent2)
            .WithMany(p => p.ChildrenAsParent2)
            .HasForeignKey(p => p.Parent2Id)
            .OnDelete(DeleteBehavior.SetNull);

        // Indexes for better query performance
        builder.HasIndex(p => p.LastName);
        builder.HasIndex(p => new { p.FirstName, p.LastName });
        builder.HasIndex(p => p.BirthDate);
    }
}