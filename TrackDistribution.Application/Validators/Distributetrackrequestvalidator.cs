using FluentValidation;
using TrackDistribution.Application.DTOs;

namespace TrackDistribution.Application.Validators;

public class DistributeTrackRequestValidator : AbstractValidator<DistributeTrackRequest>
{
    public DistributeTrackRequestValidator()
    {
        RuleFor(x => x.DspIds)
            .NotEmpty().WithMessage("At least one DspId must be provided.");

        RuleForEach(x => x.DspIds)
            .NotEmpty().WithMessage("DspId values cannot be empty GUIDs.");
    }
}