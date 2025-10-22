using ThoughtsApp.Api.Common.Filters;

namespace ThoughtsApp.Api.Common.Extensions;

public static class RouteBuilderValidationExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<TRequest>(
        this RouteHandlerBuilder builder
    )
    {
        return builder
            .AddEndpointFilter<RequestValidationFilter<TRequest>>()
            .ProducesValidationProblem();
    }
}
