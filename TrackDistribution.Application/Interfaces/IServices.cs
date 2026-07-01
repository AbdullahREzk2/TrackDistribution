using TrackDistribution.Application.DTOs;

namespace TrackDistribution.Application.Interfaces;

public interface IArtistService
{
    Task<ArtistResponse> CreateAsync(CreateArtistRequest request, CancellationToken ct = default);
    Task<List<ArtistResponse>> GetAllAsync(CancellationToken ct = default);
}

public interface ITrackService
{
    Task<TrackDetailResponse> CreateAsync(CreateTrackRequest request, CancellationToken ct = default);
    Task<List<TrackListItemResponse>> GetAllAsync(TrackFilterQuery filter, CancellationToken ct = default);
    Task<TrackDetailResponse> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TrackDetailResponse> DistributeAsync(Guid trackId, DistributeTrackRequest request, CancellationToken ct = default);
    Task<TrackDetailResponse> UpdateStatusAsync(Guid trackId, UpdateTrackStatusRequest request, CancellationToken ct = default);
}

/// <summary>Issues JWTs. Implemented in Infrastructure (needs signing-key config + crypto).</summary>
public interface IJwtTokenService
{
    string GenerateToken(string username, IEnumerable<string> roles);
}