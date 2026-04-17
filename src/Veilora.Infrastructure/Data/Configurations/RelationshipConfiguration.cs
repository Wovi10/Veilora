using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class RelationshipConfiguration : IEntityTypeConfiguration<Relationship>
{
    public void Configure(EntityTypeBuilder<Relationship> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.RelationshipType).HasConversion<string>().HasMaxLength(50).IsRequired();
        builder.Property(r => r.Notes).HasColumnType("text");

        builder.HasOne(r => r.Character1)
            .WithMany(c => c.RelationshipsAsCharacter1)
            .HasForeignKey(r => r.Character1Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Character2)
            .WithMany(c => c.RelationshipsAsCharacter2)
            .HasForeignKey(r => r.Character2Id)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => r.Character1Id);
        builder.HasIndex(r => r.Character2Id);
        builder.HasIndex(r => r.RelationshipType);
    }
}
