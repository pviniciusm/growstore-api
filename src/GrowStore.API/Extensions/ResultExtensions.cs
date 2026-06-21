using GrowStore.Application.Common.Errors;
using GrowStore.Application.Common.Results;
using Microsoft.AspNetCore.Mvc;

namespace GrowStore.API.Extensions;

public static class ResultExtensions
{
    public static ActionResult ToActionResult(this Result result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.NoContent();
        }

        return controller.ProblemFromError(result.Error);
    }

    public static ActionResult<T> ToActionResult<T>(this Result<T> result, ControllerBase controller)
    {
        if (result.IsSuccess)
        {
            return controller.Ok(result.Value);
        }

        return controller.ProblemFromError(result.Error);
    }

    private static ObjectResult ProblemFromError(this ControllerBase controller, Error? error)
    {
        var fallbackError = Error.Failure("General.Failure", "An unexpected error occurred.");
        error ??= fallbackError;

        var statusCode = GetStatusCode(error.Type);

        return controller.StatusCode(statusCode, new ProblemDetails
        {
            Status = statusCode,
            Title = GetTitle(error.Type),
            Detail = error.Message,
            Type = $"https://httpstatuses.com/{statusCode}",
            Extensions =
            {
                ["code"] = error.Code
            }
        });
    }

    private static int GetStatusCode(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };
    }

    private static string GetTitle(ErrorType errorType)
    {
        return errorType switch
        {
            ErrorType.Validation => "Validation error",
            ErrorType.NotFound => "Resource not found",
            ErrorType.Conflict => "Conflict",
            ErrorType.Unauthorized => "Unauthorized",
            ErrorType.Forbidden => "Forbidden",
            _ => "Unexpected error"
        };
    }
}
