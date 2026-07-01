using TrackDistribution.Domain.Enums;

namespace TrackDistribution.Domain.Entities;

/// <summary>
/// Represents a single distribution of a Track to a DSP.
/// Named "TrackDistributionRecord" rather than "TrackDistribution" to avoid
/// colliding with the root solution namespace (TrackDistribution.*).
/// Maps to the "TrackDistribution" table via EF configuration.
/// </summary>
public class TrackDistributionRecord
{
    public Guid Id { get; set; }

    public Guid TrackId { get; set; }
    public Track? Track { get; set; }

    public Guid DspId { get; set; }
    public Dsp? Dsp { get; set; }

    public DateTime SubmittedAt { get; set; }
    public DistributionStatus Status { get; set; } = DistributionStatus.Pending;
}
