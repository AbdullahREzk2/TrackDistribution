using System.Text.RegularExpressions;
using FluentValidation;
using TrackDistribution.Application.DTOs;

namespace TrackDistribution.Application.Validators;

public class CreateTrackRequestValidator : AbstractValidator<CreateTrackRequest>
{
    // Standard ISRC format: CC-REG-YY-NNNNN (12 alphanumeric chars, hyphens optional)
    // e.g. USRC17607839
    private static readonly Regex IsrcPattern = new(
        @"^[A-Z]{2}[A-Z0-9]{3}\d{2}\d{5}$", RegexOptions.Compiled);

    public CreateTrackRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Track title is required.")
            .MaximumLength(300);

        RuleFor(x => x.ArtistId)
            .NotEmpty().WithMessage("ArtistId is required.");

        RuleFor(x => x.Isrc)
            .NotEmpty().WithMessage("ISRC is required.")
            .Must(isrc => IsrcPattern.IsMatch(isrc.Replace("-", "").ToUpperInvariant()))
            .WithMessage("ISRC must follow the standard format, e.g. USRC17607839 (CC + 3 alphanumeric + YY + 5 digits).");

        RuleFor(x => x.Genre)
            .NotEmpty().WithMessage("Genre is required.")
            .MaximumLength(100);

        RuleFor(x => x.ReleaseDate)
            .NotEqual(default(DateTime)).WithMessage("ReleaseDate is required.");
    }
}