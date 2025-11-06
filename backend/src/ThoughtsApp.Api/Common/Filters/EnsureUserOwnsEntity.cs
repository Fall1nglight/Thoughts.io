using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Shared.Types;

namespace ThoughtsApp.Api.Common.Filters;

public class EnsureUserOwnsEntity<TEntity, TRequest> : IEndpointFilter
    where TEntity : class, IEntity, IOwnedEntity
{
    // fields
    private readonly AppDbContext _db;
    private readonly Func<TRequest, Guid> _idSelector;

    private record Entity(Guid Id, Guid UserId);

    // constructors
    public EnsureUserOwnsEntity(AppDbContext db, Func<TRequest, Guid> idSelector)
    {
        _db = db;
        _idSelector = idSelector;
    }

    // methods
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var request = context.Arguments.OfType<TRequest>().Single();
        var cancellationToken = context.HttpContext.RequestAborted;
        var id = _idSelector(request);
        var userId = context.HttpContext.User.GetUserId();

        var entity = await _db.Set<TEntity>()
            .Where(x => x.Id == id)
            .Select(x => new Entity(x.Id, x.UserId))
            .SingleOrDefaultAsync(cancellationToken);

        // return HTTP 404 NotFound to prevent leaking sensitive information about user entities
        if (entity == null || entity.UserId != userId)
            return TypedResults.NotFound();

        return await next(context);
    }
}
