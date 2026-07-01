namespace TrackDistribution.Domain.Entities;
public class Dsp
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g. Spotify, Apple Music

    public ICollection<TrackDistributionRecord> Distributions { get; set; } = new List<TrackDistributionRecord>();
}
