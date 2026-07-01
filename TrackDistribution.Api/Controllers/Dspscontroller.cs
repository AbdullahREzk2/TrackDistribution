using Microsoft.AspNetCore.Mvc;
using TrackDistribution.Application.DTOs;
using TrackDistribution.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TrackDistribution.Api.Controllers;

[ApiController]
[Route("api/dsps")]
public class DspsController : ControllerBase
{
    private readonly IApplicationDbContext _db;

    public DspsController(IApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<ActionResult<List<DspResponse>>> GetAll(CancellationToken ct)
    {
        var dsps = await _db.Dsps
            .OrderBy(d => d.Name)
            .Select(d => new DspResponse(d.Id, d.Name))
            .ToListAsync(ct);

        return Ok(dsps);
    }
}