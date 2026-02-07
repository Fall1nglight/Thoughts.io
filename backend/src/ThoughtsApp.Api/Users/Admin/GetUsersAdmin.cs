using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Users.Admin;

public class GetUsersAdmin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("", Handle).WithSummary("Gets every user detail");
    }

    public record User(Guid Id, string Username, DateTime CreatedAtUtc, Stats Stats);

    public record Stats(Thought Thoughts, Comment Comments, List<Reaction> Reactions);

    public record Comment(int Count);

    public record Thought(int Count);

    public record Reaction(int Id, int Count);

    public record Response(List<User> Users);

    private static async Task<Ok<Response>> Handle(
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var user = await db
            .Users.AsNoTracking()
            .Select(x => new User(
                x.Id,
                x.Username,
                x.CreatedAtUtc,
                new Stats(
                    new Thought(x.Thoughts.Count),
                    new Comment(x.Comments.Count),
                    x.Reactions.GroupBy(r => r.ReactionId)
                        .Select(g => new Reaction(g.Key, g.Count()))
                        .ToList()
                )
            ))
            .ToListAsync(cancellationToken);

        var respone = new Response(user);
        return TypedResults.Ok(respone);
    }
}
