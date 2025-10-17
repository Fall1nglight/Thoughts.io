using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ThoughtsApp.Api.Common.ExceptionHandlers;

public class GlobalExceptionHandler : IExceptionHandler
{
    // fields
    private readonly IProblemDetailsService _problemDetailsService;

    // constructors
    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService)
    {
        _problemDetailsService = problemDetailsService;
    }

    // methods
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        return await _problemDetailsService.TryWriteAsync(
            new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = new ProblemDetails
                {
                    Type = exception.GetType().Name,
                    Title = "Internal Server Error",
                    Detail = exception.Message,
                    Status = StatusCodes.Status500InternalServerError,
                },
            }
        );
    }
}
