using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class PersonTreeConfiguration : IEntityTypeConfiguration<PersonTree>
{
    public void Configure(EntityTypeBuilder<PersonTree> builder)
    {
        // Composite primary key
        builder.HasKey(pt => new { pt.PersonId, pt.TreeId });

        // Foreign key relationships
        builder.HasOne(pt => pt.Person)
            .WithMany(p => p.PersonTrees)
            .HasForeignKey(pt => pt.PersonId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pt => pt.Tree)
            .WithMany(t => t.PersonTrees)
            .HasForeignKey(pt => pt.TreeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Property(pt => pt.PositionX).IsRequired(false);
        builder.Property(pt => pt.PositionY).IsRequired(false);

        // Indexes
        builder.HasIndex(pt => pt.PersonId);
        builder.HasIndex(pt => pt.TreeId);
    }
}