using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class DateSuffixConfiguration : IEntityTypeConfiguration<DateSuffix>
{
    public void Configure(EntityTypeBuilder<DateSuffix> builder)
    {
        builder.ToTable("DateSuffixes");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Name).IsRequired().HasMaxLength(100);
        builder.Property(d => d.Abbreviation).IsRequired().HasMaxLength(20);
        builder.Property(d => d.Order).IsRequired();
        builder.Property(d => d.IsDefault).IsRequired();

        builder.HasOne(d => d.World)
            .WithMany()
            .HasForeignKey(d => d.WorldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => d.WorldId);
        builder.HasIndex(d => new { d.WorldId, d.Abbreviation }).IsUnique();
    }
}
