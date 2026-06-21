using GrowStore.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Middlewares;

public sealed class ExceptionHandlingMiddleware
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
        catch (DomainException exception)
        {
            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Business rule violation",
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Unhandled exception");

            await WriteProblemDetailsAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteProblemDetailsAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Type = $"https://httpstatuses.com/{statusCode}",
            Instance = context.Request.Path
        };

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
