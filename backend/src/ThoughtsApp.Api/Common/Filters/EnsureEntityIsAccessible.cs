using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Shared.Types;

namespace ThoughtsApp.Api.Common.Filters;

public class EnsureEntityIsAccessible<TEntity, TRequest> : IEndpointFilter
    where TEntity : class, IEntity, IOwnedEntity, IAccessibleEntity
{
    // implementáció
    // szükségünk van egy idSelector() függvényre, amely megadja a TEntity id-ját => ez alapján keressük majd az adatbázisban
    // szükségünk van egy

    private AppDbContext _db;
    private Func<TRequest, Guid> _idSelector;

    private record Entity(Guid Id, Guid UserId, bool IsPublic);

    public EnsureEntityIsAccessible(AppDbContext db, Func<TRequest, Guid> idSelector)
    {
        _db = db;
        _idSelector = idSelector;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var request = context.Arguments.OfType<TRequest>().First();
        var cancellationToken = context.HttpContext.RequestAborted;
        var userId = context.HttpContext.User.GetUserId();
        var id = _idSelector(request);

        // cél feltérképezése
        // 1. meg kell állapítanunk, hogy az adott Thought létezik-e
        //  ha NEM => HTTP 404
        // 2. meg kell állapítanunk, hogyha privát az adott Thought a bejelentkezett User-hez tartozik-e
        //  ha NEM => HTTP 404

        var entity = await _db.Set<TEntity>()
            .Where(x => x.Id == id)
            .Select(x => new Entity(x.Id, x.UserId, x.IsPublic))
            .SingleOrDefaultAsync(cancellationToken);

        if (entity == null)
            return TypedResults.NotFound();

        if (!entity.IsPublic && entity.UserId != userId)
            return TypedResults.NotFound();

        return await next(context);
    }
}
