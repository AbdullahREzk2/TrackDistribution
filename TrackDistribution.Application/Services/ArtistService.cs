using Microsoft.EntityFrameworkCore;
using TrackDistribution.Application.DTOs;
using TrackDistribution.Application.Interfaces;
using TrackDistribution.Domain.Entities;

namespace TrackDistribution.Application.Services;

public class ArtistService : IArtistService
{
    private readonly IApplicationDbContext _db;

    public ArtistService(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ArtistResponse> CreateAsync(CreateArtistRequest request, CancellationToken ct = default)
    {
        var artist = new Artist
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Country = request.Country.Trim()
        };

        _db.Artists.Add(artist);
        await _db.SaveChangesAsync(ct);

        return new ArtistResponse(artist.Id, artist.Name, artist.Email, artist.Country);
    }

    public async Task<List<ArtistResponse>> GetAllAsync(CancellationToken ct = default)
    {
        return await _db.Artists
            .OrderBy(a => a.Name)
            .Select(a => new ArtistResponse(a.Id, a.Name, a.Email, a.Country))
            .ToListAsync(ct);
    }

}