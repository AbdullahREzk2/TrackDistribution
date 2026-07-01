namespace TrackDistribution.Application.DTOs;

public record DspResponse(Guid Id, string Name);

// POST /api/tracks/{id}/distribute — submit to one or more DSPs at once
public record DistributeTrackRequest(List<Guid> DspIds);

public record TrackDistributionResponse(
    Guid Id,
    Guid DspId,
    string DspName,
    DateTime SubmittedAt,
    string Status);