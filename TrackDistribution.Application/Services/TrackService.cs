using Microsoft.EntityFrameworkCore;
using TrackDistribution.Application.Common.Exceptions;
using TrackDistribution.Application.DTOs;
using TrackDistribution.Application.Interfaces;
using TrackDistribution.Domain.Entities;
using TrackDistribution.Domain.Enums;

namespace TrackDistribution.Application.Services;

public class TrackService : ITrackService
{
    private readonly IApplicationDbContext _db;

    public TrackService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<TrackDetailResponse> CreateAsync(CreateTrackRequest request, CancellationToken ct = default)
    {
        var artist = await _db.Artists.FirstOrDefaultAsync(a => a.Id == request.ArtistId, ct)
            ?? throw new NotFoundException(nameof(Artist), request.ArtistId);

        var normalizedIsrc = request.Isrc.Replace("-", "").ToUpperInvariant();

        var isrcTaken = await _db.Tracks.AnyAsync(t => t.Isrc == normalizedIsrc, ct);
        if (isrcTaken)
            throw new BusinessRuleException($"A track with ISRC '{normalizedIsrc}' already exists.");

        var track = new Track
        {
            Id = Guid.NewGuid(),
            Title = request.Title.Trim(),
            ArtistId = request.ArtistId,
            Isrc = normalizedIsrc,
            ReleaseDate = request.ReleaseDate,
            Genre = request.Genre.Trim(),
            Status = TrackStatus.Draft
        };

        _db.Tracks.Add(track);
        await _db.SaveChangesAsync(ct);

        return ToDetailResponse(track, artist.Name, new List<TrackDistributionRecord>());
    }

    public async Task<List<TrackListItemResponse>> GetAllAsync(TrackFilterQuery filter, CancellationToken ct = default)
    {
        var query = _db.Tracks.Include(t => t.Artist).AsQueryable();

        if (filter.ArtistId.HasValue)
            query = query.Where(t => t.ArtistId == filter.ArtistId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Genre))
            query = query.Where(t => t.Genre.ToLower() == filter.Genre.ToLower());

        if (!string.IsNullOrWhiteSpace(filter.Status))
        {
            if (!Enum.TryParse<TrackStatus>(filter.Status, ignoreCase: true, out var statusFilter))
                throw new BusinessRuleException(
                    $"Invalid status filter '{filter.Status}'. Must be one of: {string.Join(", ", Enum.GetNames<TrackStatus>())}.");

            query = query.Where(t => t.Status == statusFilter);
        }

        return await query
            .OrderByDescending(t => t.ReleaseDate)
            .Select(t => new TrackListItemResponse(
                t.Id, t.Title, t.Artist!.Name, t.Genre, t.Status.ToString(), t.ReleaseDate))
            .ToListAsync(ct);
    }

    public async Task<TrackDetailResponse> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var track = await _db.Tracks
            .Include(t => t.Artist)
            .Include(t => t.Distributions).ThenInclude(d => d.Dsp)
            .FirstOrDefaultAsync(t => t.Id == id, ct)
            ?? throw new NotFoundException(nameof(Track), id);

        return ToDetailResponse(track, track.Artist!.Name, track.Distributions.ToList());
    }

    public async Task<TrackDetailResponse> DistributeAsync(Guid trackId, DistributeTrackRequest request, CancellationToken ct = default)
    {
        var track = await _db.Tracks
            .Include(t => t.Artist)
            .Include(t => t.Distributions)
            .FirstOrDefaultAsync(t => t.Id == trackId, ct)
            ?? throw new NotFoundException(nameof(Track), trackId);

        var requestedDsps = await _db.Dsps
            .Where(d => request.DspIds.Contains(d.Id))
            .ToListAsync(ct);

        var missingDspIds = request.DspIds.Except(requestedDsps.Select(d => d.Id)).ToList();
        if (missingDspIds.Any())
            throw new NotFoundException(nameof(Dsp), string.Join(", ", missingDspIds));

        var now = DateTime.UtcNow;
        foreach (var dsp in requestedDsps)
        {
            // Avoid duplicate pending/live submissions to the same DSP —
            // if one already exists and isn't rejected, skip re-creating it.
            var existing = track.Distributions.FirstOrDefault(d => d.DspId == dsp.Id);
            if (existing is not null && existing.Status != DistributionStatus.Rejected)
                continue;

            _db.TrackDistributions.Add(new TrackDistributionRecord
            {
                Id = Guid.NewGuid(),
                TrackId = track.Id,
                DspId = dsp.Id,
                SubmittedAt = now,
                Status = DistributionStatus.Pending
            });
        }

        // Submitting to at least one DSP moves the track out of Draft.
        if (track.Status == TrackStatus.Draft)
            track.Status = TrackStatus.Submitted;

        await _db.SaveChangesAsync(ct);

        return await GetByIdAsync(trackId, ct);
    }

    public async Task<TrackDetailResponse> UpdateStatusAsync(Guid trackId, UpdateTrackStatusRequest request, CancellationToken ct = default)
    {
        var track = await _db.Tracks
            .Include(t => t.Artist)
            .Include(t => t.Distributions).ThenInclude(d => d.Dsp)
            .FirstOrDefaultAsync(t => t.Id == trackId, ct)
            ?? throw new NotFoundException(nameof(Track), trackId);

        // Validator already confirmed this parses; re-parsing here keeps the service safe standalone.
        Enum.TryParse<TrackStatus>(request.Status, ignoreCase: true, out var newStatus);
        track.Status = newStatus;

        await _db.SaveChangesAsync(ct);

        return ToDetailResponse(track, track.Artist!.Name, track.Distributions.ToList());
    }

    private static TrackDetailResponse ToDetailResponse(Track track, string artistName, List<TrackDistributionRecord> distributions)
    {
        return new TrackDetailResponse(
            track.Id,
            track.Title,
            track.ArtistId,
            artistName,
            track.Isrc,
            track.ReleaseDate,
            track.Genre,
            track.Status.ToString(),
            distributions.Select(d => new TrackDistributionResponse(
                d.Id, d.DspId, d.Dsp?.Name ?? string.Empty, d.SubmittedAt, d.Status.ToString())).ToList());
    }
}