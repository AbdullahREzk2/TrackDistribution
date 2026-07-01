using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TrackDistribution.Domain.Entities;

namespace TrackDistribution.Infrastructure.Persistence.Configurations;

public class TrackDistributionRecordConfiguration : IEntityTypeConfiguration<TrackDistributionRecord>
{
    public void Configure(EntityTypeBuilder<TrackDistributionRecord> builder)
    {
        builder.ToTable("TrackDistributions");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(d => d.Track)
            .WithMany(t => t.Distributions)
            .HasForeignKey(d => d.TrackId)
            .OnDelete(DeleteBehavior.Cascade); // deleting a track deletes its distribution records

        builder.HasOne(d => d.Dsp)
            .WithMany(dsp => dsp.Distributions)
            .HasForeignKey(d => d.DspId)
            .OnDelete(DeleteBehavior.Restrict);

        // A track shouldn't have two active rows for the same DSP.
        builder.HasIndex(d => new { d.TrackId, d.DspId });
    }
}