namespace ThoughtsApp.Api.Common.Filters;

public class RequestLoggingFilter : IEndpointFilter
{
    // fields
    private readonly ILogger<RequestLoggingFilter> _logger;

    // constructors
    public RequestLoggingFilter(ILogger<RequestLoggingFilter> logger)
    {
        _logger = logger;
    }

    // methods
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        _logger.LogInformation(
            "HTTP {Method} {Path} received",
            context.HttpContext.Request.Method,
            context.HttpContext.Request.Path
        );

        return await next(context);
    }
}
