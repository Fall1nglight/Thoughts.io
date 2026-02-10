using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Stats;

public class GetReactionDistributionStats : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/breakdown", Handle).WithSummary("Gets reaction distribution");
    }

    public record Reaction(int Id, string Name, int Count);

    public record Response(List<Reaction> Reactions);

    private static async Task<Ok<Response>> Handle(
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var reactionDistribution = await db
            .ThoughtReactions.GroupBy(x => new { x.ReactionId, x.Reaction.Name })
            .Select(g => new Reaction(g.Key.ReactionId, g.Key.Name, g.Count()))
            .ToListAsync(cancellationToken);

        var response = new Response(reactionDistribution);
        return TypedResults.Ok(response);
    }
}
