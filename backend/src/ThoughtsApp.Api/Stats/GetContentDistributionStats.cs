using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Stats;

public class GetContentDistributionStats : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/content/breakdown", Handle).WithSummary("Gets content distribution");
    }

    public record Response(int ThoughtCount, int CommentCount);

    private static async Task<Ok<Response>> Handle(
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var thoughtCount = await db.Thoughts.CountAsync(cancellationToken);
        var commentCount = await db.Comments.CountAsync(cancellationToken);
        var response = new Response(thoughtCount, commentCount);
        return TypedResults.Ok(response);
    }
}
