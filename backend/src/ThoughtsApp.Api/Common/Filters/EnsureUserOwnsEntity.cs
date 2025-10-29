using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Shared.Types;

namespace ThoughtsApp.Api.Common.Filters;

public class EnsureUserOwnsEntity<TEntity, TRequest> : IEndpointFilter
    where TEntity : class, IEntity, IOwnedEntity
{
    // fields
    private readonly AppDbContext _context;
    private readonly Func<TRequest, Guid> _idSelector;

    private record Entity(Guid Id, Guid UserId);

    // constructors
    public EnsureUserOwnsEntity(AppDbContext context, Func<TRequest, Guid> idSelector)
    {
        _context = context;
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

        var entity = await _context
            .Set<TEntity>()
            .Where(x => x.Id == id)
            .Select(x => new Entity(x.Id, x.UserId))
            .SingleOrDefaultAsync(cancellationToken);

        // return HTTP 404 Notfound to prevent leaking sensitive information about user entities
        if (entity == null || entity.UserId != userId)
            return TypedResults.NotFound();

        return await next(context);
    }
}
