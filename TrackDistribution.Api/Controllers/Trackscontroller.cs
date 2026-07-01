using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TrackDistribution.Application.DTOs;
using TrackDistribution.Application.Interfaces;

namespace TrackDistribution.Api.Controllers;

[ApiController]
[Route("api/tracks")]
public class TracksController : ControllerBase
{
    private readonly ITrackService _trackService;
    private readonly IValidator<CreateTrackRequest> _createValidator;
    private readonly IValidator<UpdateTrackStatusRequest> _statusValidator;
    private readonly IValidator<DistributeTrackRequest> _distributeValidator;

    public TracksController(
        ITrackService trackService,
        IValidator<CreateTrackRequest> createValidator,
        IValidator<UpdateTrackStatusRequest> statusValidator,
        IValidator<DistributeTrackRequest> distributeValidator)
    {
        _trackService = trackService;
        _createValidator = createValidator;
        _statusValidator = statusValidator;
        _distributeValidator = distributeValidator;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<TrackDetailResponse>> Create([FromBody] CreateTrackRequest request, CancellationToken ct)
    {
        await _createValidator.ValidateAndThrowAsync(request, ct);
        var result = await _trackService.CreateAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet]
    public async Task<ActionResult<List<TrackListItemResponse>>> GetAll(
        [FromQuery] Guid? artistId, [FromQuery] string? genre, [FromQuery] string? status, CancellationToken ct)
    {
        var result = await _trackService.GetAllAsync(new TrackFilterQuery(artistId, genre, status), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TrackDetailResponse>> GetById(Guid id, CancellationToken ct)
    {
        var result = await _trackService.GetByIdAsync(id, ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/distribute")]
    [Authorize]
    public async Task<ActionResult<TrackDetailResponse>> Distribute(Guid id, [FromBody] DistributeTrackRequest request, CancellationToken ct)
    {
        await _distributeValidator.ValidateAndThrowAsync(request, ct);
        var result = await _trackService.DistributeAsync(id, request, ct);
        return Ok(result);
    }

    [HttpPatch("{id:guid}/status")]
    [Authorize]
    public async Task<ActionResult<TrackDetailResponse>> UpdateStatus(Guid id, [FromBody] UpdateTrackStatusRequest request, CancellationToken ct)
    {
        await _statusValidator.ValidateAndThrowAsync(request, ct);
        var result = await _trackService.UpdateStatusAsync(id, request, ct);
        return Ok(result);
    }
}