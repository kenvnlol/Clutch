using Immediate.Validations.Shared;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Clutch.Infrastructure.Exceptions;

public static class ExceptionStartupExtensions
{
    public static void ConfigureProblemDetails(ProblemDetailsOptions options) =>
        options.CustomizeProblemDetails = c =>
        {
            if (c.Exception is null)
            {
                return;
            }

            c.ProblemDetails = c.Exception switch
            {
                ValidationException ex => new ValidationProblemDetails(
                    ex
                        .Errors
                        .GroupBy(x => x.PropertyName, StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(
                            x => x.Key,
                            x => x.Select(x => x.ErrorMessage).ToArray(),
                            StringComparer.OrdinalIgnoreCase
                        )
                )
                {
                    Status = StatusCodes.Status400BadRequest,
                },

                ExceptionBase ex => new()
                {
                    Type = ex.ErrorCode,
                    Detail = ex.Message,
                    Status = ex.StatusCode
                },

                UnauthorizedAccessException ex => new()
                {
                    Detail = "Access denied.",
                    Status = StatusCodes.Status403Forbidden,
                },

                var ex => new ProblemDetails
                {
                    Detail = "An error has occurred.",
                    Status = StatusCodes.Status500InternalServerError
                },
            };

            c.ProblemDetails.Instance = $"{c.HttpContext.Request.Method} {c.HttpContext.Request.Path}";
            c.ProblemDetails.Extensions.TryAdd("requestId", c.HttpContext.TraceIdentifier);
            var activity = c.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
            c.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
            c.HttpContext.Response.StatusCode =
                c.ProblemDetails.Status
                ?? StatusCodes.Status500InternalServerError;
        };
}
