using Microsoft.AspNetCore.Mvc;
using TrackDistribution.Application.Interfaces;

namespace TrackDistribution.Api.Controllers;

public record LoginRequest(string Username, string Password);
public record LoginResponse(string Token, DateTime ExpiresAtUtc);

/// <summary>
/// Minimal demo authentication so the task's "JWT-protected endpoint" requirement is testable.
/// A full user/identity system (registration, password hashing, refresh tokens) is out of scope
/// for this take-home — see DECISIONS.md for the reasoning. Credentials are read from config,
/// not hardcoded, so this endpoint doesn't ship real secrets in source.
/// </summary>
[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtTokenService _tokenService;
    private readonly IConfiguration _configuration;

    public AuthController(IJwtTokenService tokenService, IConfiguration configuration)
    {
        _tokenService = tokenService;
        _configuration = configuration;
    }

    [HttpPost("token")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var demoUser = _configuration["DemoUser:Username"];
        var demoPassword = _configuration["DemoUser:Password"];

        if (request.Username != demoUser || request.Password != demoPassword)
            return Unauthorized(new { status = 401, title = "Invalid username or password." });

        var token = _tokenService.GenerateToken(request.Username, new[] { "Admin" });
        return Ok(new LoginResponse(token, DateTime.UtcNow.AddMinutes(60)));
    }
}