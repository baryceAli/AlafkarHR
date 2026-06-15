using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Shared.Exceptions.Handler;

public class CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
    : IExceptionHandler
{
    private const string UnexpectedErrorMessage = "An unexpected error occurred. Please try again or contact support.";

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Unhandled API exception at {Path}. Time of occurrence: {Time}", context.Request.Path, DateTime.UtcNow);

        (string Detail, string Title, int StatusCode) details = exception switch
        {
            InternalServerException =>
            (
                UnexpectedErrorMessage,
                "Internal Server Error",
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            ),
            ValidationException validationFailure =>
            (
                GetValidationDetail(validationFailure),
                "Validation Error",
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            BadRequestException =>
            (
                exception.Message,
                "Bad Request",
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            ArgumentException =>
            (
                exception.Message,
                "Bad Request",
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            InvalidOperationException =>
            (
                exception.Message,
                "Bad Request",
                context.Response.StatusCode = StatusCodes.Status400BadRequest
            ),
            UnauthorizedAccessException =>
            (
                exception.Message,
                "Unauthorized",
                context.Response.StatusCode = StatusCodes.Status401Unauthorized
            ),
            NotFoundException =>
            (
                exception.Message,
                "Not Found",
                context.Response.StatusCode = StatusCodes.Status404NotFound
            ),
            _ =>
            (
                UnexpectedErrorMessage,
                "Internal Server Error",
                context.Response.StatusCode = StatusCodes.Status500InternalServerError
            )
        };

        context.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Detail = details.Detail,
            Status = details.StatusCode,
            Instance = context.Request.Path
        };

        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => error.ErrorMessage).Distinct().ToArray());

            problemDetails.Extensions.Add("errors", errors);
        }

        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken: cancellationToken);
        return true;
    }

    private static string GetValidationDetail(ValidationException validationException)
    {
        var firstError = validationException.Errors.FirstOrDefault()?.ErrorMessage;

        return string.IsNullOrWhiteSpace(firstError)
            ? "One or more validation errors occurred."
            : firstError;
    }
}
