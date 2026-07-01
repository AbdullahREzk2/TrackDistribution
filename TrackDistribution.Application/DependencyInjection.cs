using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using TrackDistribution.Application.Interfaces;
using TrackDistribution.Application.Services;

namespace TrackDistribution.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IArtistService, ArtistService>();
        services.AddScoped<ITrackService, TrackService>();

        // Registers every AbstractValidator<T> found in this assembly (all files in Validators/).
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}