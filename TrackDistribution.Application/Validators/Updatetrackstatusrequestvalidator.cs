using FluentValidation;
using TrackDistribution.Application.DTOs;
using TrackDistribution.Domain.Enums;

namespace TrackDistribution.Application.Validators;

public class UpdateTrackStatusRequestValidator : AbstractValidator<UpdateTrackStatusRequest>
{
    public UpdateTrackStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty().WithMessage("Status is required.")
            .Must(s => Enum.TryParse<TrackStatus>(s, ignoreCase: true, out _))
            .WithMessage($"Status must be one of: {string.Join(", ", Enum.GetNames<TrackStatus>())}.");
    }
}