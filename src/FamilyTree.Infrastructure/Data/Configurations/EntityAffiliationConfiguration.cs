using FamilyTree.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FamilyTree.Infrastructure.Data.Configurations;

public class EntityAffiliationConfiguration : IEntityTypeConfiguration<EntityAffiliation>
{
    public void Configure(EntityTypeBuilder<EntityAffiliation> builder)
    {
        builder.HasKey(ea => new { ea.CharacterId, ea.GroupId });

        builder.HasOne(ea => ea.Character)
            .WithMany(c => c.Affiliations)
            .HasForeignKey(ea => ea.CharacterId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ea => ea.Group)
            .WithMany()
            .HasForeignKey(ea => ea.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ea => ea.CharacterId);
        builder.HasIndex(ea => ea.GroupId);
    }
}
