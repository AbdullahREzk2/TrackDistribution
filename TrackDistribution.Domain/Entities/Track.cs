using TrackDistribution.Domain.Enums;

namespace TrackDistribution.Domain.Entities;
public class Track
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public Guid ArtistId { get; set; }
    public Artist? Artist { get; set; }

    //  unique per track
    public string Isrc { get; set; } = string.Empty;

    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; } = string.Empty;
    public TrackStatus Status { get; set; } = TrackStatus.Draft;

    public ICollection<TrackDistributionRecord> Distributions { get; set; } = new List<TrackDistributionRecord>();
}
