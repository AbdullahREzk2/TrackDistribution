using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;
using FluentValidation;
using TrackDistribution.Application.Common.Exceptions;
using ValidationException = FluentValidation.ValidationException;

namespace TrackDistribution.Api.Middleware;

/// <summary>
/// Central place that turns exceptions into consistent JSON error responses,
/// so controllers stay free of try/catch noise. This is what makes our
/// error responses "meaningful" per the task requirement.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, title, errors) = exception switch
        {
            ValidationException vex => (
                HttpStatusCode.BadRequest,
                "One or more validation errors occurred.",
                (object?)vex.Errors.Select(e => new { field = e.PropertyName, message = e.ErrorMessage })),

            NotFoundException nfEx => (HttpStatusCode.NotFound, nfEx.Message, null),

            BusinessRuleException brEx => (HttpStatusCode.Conflict, brEx.Message, null),

            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.", null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(exception, "Unhandled exception");

        context.Response.StatusCode = (int)statusCode;

        var payload = JsonSerializer.Serialize(new
        {
            status = (int)statusCode,
            title,
            errors
        }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(payload);
    }
}