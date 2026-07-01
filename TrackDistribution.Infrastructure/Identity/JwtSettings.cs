namespace TrackDistribution.Infrastructure.Identity;

public class JwtSettings
{
    public string SigningKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "TrackDistributionApi";
    public string Audience { get; set; } = "TrackDistributionClient";
    public int ExpiryMinutes { get; set; } = 60;
}