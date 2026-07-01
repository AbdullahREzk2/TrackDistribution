using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackDistribution.Domain.Entities;

namespace TrackDistribution.Infrastructure.Persistence.Configurations;

public class ArtistConfiguration : IEntityTypeConfiguration<Artist>
{
    public void Configure(EntityTypeBuilder<Artist> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Name).IsRequired().HasMaxLength(200);
        builder.Property(a => a.Email).IsRequired().HasMaxLength(256);
        builder.Property(a => a.Country).IsRequired().HasMaxLength(100);
        builder.HasIndex(a => a.Email).IsUnique();
    }
}
