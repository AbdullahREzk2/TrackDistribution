using Microsoft.EntityFrameworkCore;
using TrackDistribution.Domain.Entities;

namespace TrackDistribution.Application.Interfaces;

/// <summary>
/// Abstraction over the persistence layer. Application depends on this interface only;
/// TrackDistribution.Infrastructure provides the concrete EF Core implementation.
/// This keeps Application testable (mockable) and decoupled from EF Core specifics.
/// </summary>
public interface IApplicationDbContext
{
    DbSet<Artist> Artists { get; }
    DbSet<Track> Tracks { get; }
    DbSet<Dsp> Dsps { get; }
    DbSet<TrackDistributionRecord> TrackDistributions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}