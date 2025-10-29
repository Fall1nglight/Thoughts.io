using FluentValidation;

namespace ThoughtsApp.Api.Common.Filters;

public class RequestValidationFilter<TRequest> : IEndpointFilter
{
    // fields
    private readonly ILogger<RequestValidationFilter<TRequest>> _logger;
    private readonly IValidator<TRequest>? _validator;

    // constructors
    public RequestValidationFilter(
        ILogger<RequestValidationFilter<TRequest>> logger,
        IValidator<TRequest>? validator
    )
    {
        _logger = logger;
        _validator = validator;
    }

    // methods

    /// <summary>
    ///     Intercepts the incoming request to perform validation on the <typeparamref name="TRequest" /> object.
    ///     If a validator is registered, it validates the request.
    ///     If validation fails, it returns a 400 Bad Request (ValidationProblem).
    ///     If validation succeeds or no validator is found, it proceeds to the next filter in the pipeline.
    /// </summary>
    /// <param name="context">EndpointFilterInvocationContext</param>
    /// <param name="next">EndpointFilterDelegate</param>
    /// <returns></returns>
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        // replace fixes nested class name
        var requestName = typeof(TRequest).FullName?.Replace("+", ".");

        if (_validator == null)
        {
            _logger.LogInformation("{Request}: No validator configured.", requestName);
            return await next(context);
        }

        var request = context.Arguments.OfType<TRequest>().First();
        var validationResult = await _validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("{Request}: Validation failed.", requestName);
            return TypedResults.ValidationProblem(validationResult.ToDictionary());
        }

        _logger.LogInformation("{Request}: Validation succedded.", requestName);
        return await next(context);
    }
}
