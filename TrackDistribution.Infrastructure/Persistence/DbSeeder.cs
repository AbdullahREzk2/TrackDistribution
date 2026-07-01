using TrackDistribution.Domain.Entities;
using TrackDistribution.Domain.Enums;

namespace TrackDistribution.Infrastructure.Persistence;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (context.Artists.Any())
            return; // already seeded

        var artists = new List<Artist>
        {
            new() { Id = Guid.NewGuid(), Name = "Nour El Din", Email = "nour.eldin@example.com", Country = "Egypt" },
            new() { Id = Guid.NewGuid(), Name = "Layla Hassan", Email = "layla.hassan@example.com", Country = "UAE" },
            new() { Id = Guid.NewGuid(), Name = "Marcus Reed", Email = "marcus.reed@example.com", Country = "United States" }
        };
        context.Artists.AddRange(artists);

        var dsps = new List<Dsp>
        {
            new() { Id = Guid.NewGuid(), Name = "Spotify" },
            new() { Id = Guid.NewGuid(), Name = "Apple Music" },
            new() { Id = Guid.NewGuid(), Name = "YouTube Music" }
        };
        context.Dsps.AddRange(dsps);

        var tracks = new List<Track>
        {
            new() { Id = Guid.NewGuid(), Title = "Nile Nights", ArtistId = artists[0].Id, Isrc = "EGABC2400001", ReleaseDate = new DateTime(2024, 3, 10), Genre = "Pop", Status = TrackStatus.Distributed },
            new() { Id = Guid.NewGuid(), Title = "Desert Bloom", ArtistId = artists[0].Id, Isrc = "EGABC2400002", ReleaseDate = new DateTime(2024, 6, 1), Genre = "Chillout", Status = TrackStatus.Submitted },
            new() { Id = Guid.NewGuid(), Title = "Golden Skyline", ArtistId = artists[1].Id, Isrc = "AEABC2400003", ReleaseDate = new DateTime(2024, 1, 20), Genre = "Electronic", Status = TrackStatus.Distributed },
            new() { Id = Guid.NewGuid(), Title = "Midnight Oasis", ArtistId = artists[1].Id, Isrc = "AEABC2400004", ReleaseDate = new DateTime(2025, 2, 14), Genre = "R&B", Status = TrackStatus.Draft },
            new() { Id = Guid.NewGuid(), Title = "Sandstorm Heart", ArtistId = artists[1].Id, Isrc = "AEABC2400005", ReleaseDate = new DateTime(2025, 5, 5), Genre = "Pop", Status = TrackStatus.Submitted },
            new() { Id = Guid.NewGuid(), Title = "Backroad Blues", ArtistId = artists[2].Id, Isrc = "USABC2400006", ReleaseDate = new DateTime(2023, 11, 9), Genre = "Blues", Status = TrackStatus.Distributed },
            new() { Id = Guid.NewGuid(), Title = "City Lights Fade", ArtistId = artists[2].Id, Isrc = "USABC2400007", ReleaseDate = new DateTime(2024, 9, 30), Genre = "Hip-Hop", Status = TrackStatus.Draft },
            new() { Id = Guid.NewGuid(), Title = "Runaway Static", ArtistId = artists[2].Id, Isrc = "USABC2400008", ReleaseDate = new DateTime(2025, 6, 18), Genre = "Rock", Status = TrackStatus.Submitted },
            new() { Id = Guid.NewGuid(), Title = "Coastal Drift", ArtistId = artists[2].Id, Isrc = "USABC2400009", ReleaseDate = new DateTime(2026, 1, 12), Genre = "Indie", Status = TrackStatus.Draft }
        };
        context.Tracks.AddRange(tracks);

        var now = DateTime.UtcNow;
        var distributions = new List<TrackDistributionRecord>
        {
            // Nile Nights -> distributed everywhere, all live
            new() { Id = Guid.NewGuid(), TrackId = tracks[0].Id, DspId = dsps[0].Id, SubmittedAt = now.AddDays(-30), Status = DistributionStatus.Live },
            new() { Id = Guid.NewGuid(), TrackId = tracks[0].Id, DspId = dsps[1].Id, SubmittedAt = now.AddDays(-30), Status = DistributionStatus.Live },
            new() { Id = Guid.NewGuid(), TrackId = tracks[0].Id, DspId = dsps[2].Id, SubmittedAt = now.AddDays(-30), Status = DistributionStatus.Live },

            // Golden Skyline -> live on Spotify, rejected on Apple Music
            new() { Id = Guid.NewGuid(), TrackId = tracks[2].Id, DspId = dsps[0].Id, SubmittedAt = now.AddDays(-45), Status = DistributionStatus.Live },
            new() { Id = Guid.NewGuid(), TrackId = tracks[2].Id, DspId = dsps[1].Id, SubmittedAt = now.AddDays(-45), Status = DistributionStatus.Rejected },

            // Backroad Blues -> live on YouTube Music only
            new() { Id = Guid.NewGuid(), TrackId = tracks[5].Id, DspId = dsps[2].Id, SubmittedAt = now.AddDays(-90), Status = DistributionStatus.Live },

            // Desert Bloom & Sandstorm Heart & Runaway Static -> submitted, pending review
            new() { Id = Guid.NewGuid(), TrackId = tracks[1].Id, DspId = dsps[0].Id, SubmittedAt = now.AddDays(-3), Status = DistributionStatus.Pending },
            new() { Id = Guid.NewGuid(), TrackId = tracks[4].Id, DspId = dsps[0].Id, SubmittedAt = now.AddDays(-2), Status = DistributionStatus.Pending },
            new() { Id = Guid.NewGuid(), TrackId = tracks[4].Id, DspId = dsps[1].Id, SubmittedAt = now.AddDays(-2), Status = DistributionStatus.Pending },
            new() { Id = Guid.NewGuid(), TrackId = tracks[7].Id, DspId = dsps[2].Id, SubmittedAt = now.AddDays(-1), Status = DistributionStatus.Pending }
        };
        context.TrackDistributions.AddRange(distributions);

        await context.SaveChangesAsync();
    }
}