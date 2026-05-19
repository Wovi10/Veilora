using Veilora.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Veilora.Infrastructure.Data.Configurations;

public class PhotoConfiguration : IEntityTypeConfiguration<Photo>
{
    public void Configure(EntityTypeBuilder<Photo> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.EntityId).IsRequired();
        builder.Property(p => p.EntityType).HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(p => p.Url).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.DisplayOrder).IsRequired();
        builder.HasIndex(p => new { p.EntityType, p.EntityId, p.DisplayOrder });
    }
}
