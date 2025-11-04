using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class GetPublicThoughts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("", Handle).WithSummary("Gets public thoughts");
    }

    public record Reaction(int Id, int Count);

    public record Response(
        Guid Id,
        string Username,
        string Title,
        string Content,
        bool IsPublic,
        List<Reaction> Reactions,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    public record User(Guid Id, string Username);

    private static async Task<Ok<List<Response>>> Handle(
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var publicThoughts = await db
            .Thoughts.Where(t => t.IsPublic)
            .Select(t => new Response(
                t.Id,
                t.User.Username,
                t.Title,
                t.Content,
                t.IsPublic,
                t.Reactions.GroupBy(tr => tr.Reaction.Id)
                    .Select(group => new Reaction(group.Key, group.Count()))
                    .ToList(),
                t.CreatedAtUtc,
                t.UpdatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(publicThoughts);
    }
}
