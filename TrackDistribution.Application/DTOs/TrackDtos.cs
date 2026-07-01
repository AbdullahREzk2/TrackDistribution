namespace TrackDistribution.Application.DTOs;

public record CreateTrackRequest(
    string Title,
    Guid ArtistId,
    string Isrc,
    DateTime ReleaseDate,
    string Genre);

public record UpdateTrackStatusRequest(string Status);

// Lightweight shape for list views — includes artist name so the front-end
// doesn't need a second round trip per track.
public record TrackListItemResponse(
    Guid Id,
    string Title,
    string ArtistName,
    string Genre,
    string Status,
    DateTime ReleaseDate);

public record TrackDetailResponse(
    Guid Id,
    string Title,
    Guid ArtistId,
    string ArtistName,
    string Isrc,
    DateTime ReleaseDate,
    string Genre,
    string Status,
    List<TrackDistributionResponse> Distributions);

// Query parameters for GET /api/tracks
public record TrackFilterQuery(Guid? ArtistId, string? Genre, string? Status);