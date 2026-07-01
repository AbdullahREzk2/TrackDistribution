namespace TrackDistribution.Application.DTOs;

public record CreateArtistRequest(string Name, string Email, string Country);

public record ArtistResponse(Guid Id, string Name, string Email, string Country);