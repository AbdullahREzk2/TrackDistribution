using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackDistribution.Domain.Entities;

namespace TrackDistribution.Infrastructure.Persistence.Configurations;

public class TrackConfiguration : IEntityTypeConfiguration<Track>
{
    public void Configure(EntityTypeBuilder<Track> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Title).IsRequired().HasMaxLength(300);
        builder.Property(t => t.Isrc).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Genre).IsRequired().HasMaxLength(100);

        // Enum stored as readable string rather than int - keeps the DB self-documenting.
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        // ISRC must be unique per track (per task requirement).
        builder.HasIndex(t => t.Isrc).IsUnique();

        builder.HasOne(t => t.Artist)
            .WithMany(a => a.Tracks)
            .HasForeignKey(t => t.ArtistId)
            .OnDelete(DeleteBehavior.Restrict); // don't silently cascade-delete an artist's catalog
    }
}
