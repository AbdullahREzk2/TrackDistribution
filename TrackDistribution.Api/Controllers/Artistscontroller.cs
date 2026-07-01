using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackDistribution.Application.DTOs;
using TrackDistribution.Application.Interfaces;

namespace TrackDistribution.Api.Controllers;

[ApiController]
[Route("api/artists")]
public class ArtistsController : ControllerBase
{
    private readonly IArtistService _artistService;
    private readonly IValidator<CreateArtistRequest> _createValidator;

    public ArtistsController(IArtistService artistService, IValidator<CreateArtistRequest> createValidator)
    {
        _artistService = artistService;
        _createValidator = createValidator;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<ArtistResponse>> Create([FromBody] CreateArtistRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _artistService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<ArtistResponse>>> GetAll(CancellationToken ct)
    {
        var result = await _artistService.GetAllAsync(ct);
        return Ok(result);
    }
}