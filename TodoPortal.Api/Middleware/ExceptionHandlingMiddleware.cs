using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TodoPortal.Application.Common;

namespace TodoPortal.Api.Middleware;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        ProblemDetails problem;
        var statusCode = StatusCodes.Status500InternalServerError;

        switch (exception)
        {
            case ValidationException validationException:
                statusCode = StatusCodes.Status422UnprocessableEntity;
                problem = CreateValidationProblemDetails(validationException, context.Request.Path, statusCode);
                break;

            case NotFoundException notFoundException:
                statusCode = StatusCodes.Status404NotFound;
                problem = new ProblemDetails
                {
                    Title = "Resource not found.",
                    Detail = notFoundException.Message,
                    Status = statusCode,
                    Instance = context.Request.Path
                };
                break;

            default:
                _logger.LogError(exception, "Unhandled exception while processing {Path}", context.Request.Path);
                problem = new ProblemDetails
                {
                    Title = "An unexpected error occurred.",
                    Detail = "Refer to the trace identifier for additional details.",
                    Status = statusCode,
                    Instance = context.Request.Path
                };
                break;
        }

        problem.Extensions["traceId"] = Activity.Current?.Id ?? context.TraceIdentifier;

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(problem, cancellationToken: context.RequestAborted);
    }

    private static ValidationProblemDetails CreateValidationProblemDetails(
        ValidationException exception,
        string instance,
        int statusCode)
    {
        var modelState = new ModelStateDictionary();

        foreach (var (key, errors) in exception.Errors)
        {
            foreach (var error in errors)
            {
                modelState.AddModelError(key, error);
            }
        }

        return new ValidationProblemDetails(modelState)
        {
            Title = "One or more validation errors occurred.",
            Detail = exception.Message,
            Status = statusCode,
            Instance = instance
        };
    }
}
