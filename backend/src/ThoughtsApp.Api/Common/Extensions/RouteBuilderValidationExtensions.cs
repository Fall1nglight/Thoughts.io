using Microsoft.AspNetCore.Http.HttpResults;
using ThoughtsApp.Api.Common.Filters;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Shared.Types;

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

    public static RouteHandlerBuilder WithEnsureEntityExistsFilter<TEntity, TRequest>(
        this RouteHandlerBuilder builder,
        Func<TRequest, Guid> idSelector
    )
        where TEntity : class, IEntity
    {
        return builder
            .AddEndpointFilterFactory(
                (context, next) =>
                    async invocationContext =>
                    {
                        var db =
                            invocationContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                        var filter = new EnsureEntityExists<TEntity, TRequest>(db, idSelector);

                        return await filter.InvokeAsync(invocationContext, next);
                    }
            )
            .Produces<NotFound>();
    }

    public static RouteHandlerBuilder WithEnsureUserOwnsEntity<TEntity, TRequest>(
        this RouteHandlerBuilder builder,
        Func<TRequest, Guid> idSelector
    )
        where TEntity : class, IEntity, IOwnedEntity
    {
        return builder
            .AddEndpointFilterFactory(
                (context, next) =>
                    async invocationContext =>
                    {
                        var db =
                            invocationContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

                        var filter = new EnsureUserOwnsEntity<TEntity, TRequest>(db, idSelector);

                        return await filter.InvokeAsync(invocationContext, next);
                    }
            )
            .Produces<NotFound>();
    }
}
