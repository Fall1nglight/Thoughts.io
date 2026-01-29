using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class GetPublicThoughts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("", Handle).WithSummary("Gets public thoughts");
    }

    public record User(Guid Id, string Username);

    public record Comment(int Count);

    public record Reaction(int Id, int Count);

    public record Thought(
        Guid Id,
        User User,
        int? UserReactionId,
        string Title,
        string Content,
        bool IsPublic,
        Comment Comments,
        List<Reaction> Reactions,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    public record Response(List<Thought> Thoughts);

    private static async Task<Ok<Response>> Handle(
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        Guid? userId = null;

        if (claimsPrincipal.Identity?.IsAuthenticated == true)
            userId = claimsPrincipal.GetUserId();

        var publicThoughts = await db
            .Thoughts.Where(t => t.IsPublic)
            .AsNoTracking()
            .Select(t => new Thought(
                t.Id,
                new User(t.UserId, t.User.Username),
                userId == null
                    ? null
                    : t
                        .Reactions.Where(tr => tr.UserId == userId.Value)
                        .Select(tr => tr.Reaction.Id)
                        .SingleOrDefault(),
                t.Title,
                t.Content,
                t.IsPublic,
                new Comment(t.Comments.Count),
                t.Reactions.GroupBy(tr => tr.Reaction.Id)
                    .Select(group => new Reaction(group.Key, group.Count()))
                    .ToList(),
                t.CreatedAtUtc,
                t.UpdatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        var response = new Response(publicThoughts);

        return TypedResults.Ok(response);
    }
}
