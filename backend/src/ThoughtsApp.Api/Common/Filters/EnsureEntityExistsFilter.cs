using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Shared.Types;

namespace ThoughtsApp.Api.Common.Filters;

public class EnsureEntityExists<TEntity, TRequest> : IEndpointFilter
    where TEntity : class, IEntity
{
    // fields
    private readonly AppDbContext _db;
    private readonly Func<TRequest, Guid> _idSelector;

    // constructors
    public EnsureEntityExists(AppDbContext db, Func<TRequest, Guid> idSelector)
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

        var exists = await _db.Set<TEntity>().AnyAsync(x => x.Id == id, cancellationToken);

        if (!exists)
            return TypedResults.NotFound();

        return await next(context);
    }
}
