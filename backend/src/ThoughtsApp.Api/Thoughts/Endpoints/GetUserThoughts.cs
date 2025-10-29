using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class GetUserThoughts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("", Handle).WithSummary("Gets user's thoughts");
    }

    public record Response(
        Guid Id,
        string Username,
        string Title,
        string Content,
        bool IsPublic,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    private static async Task<Ok<List<Response>>> Handle(
        AppDbContext context,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var thoughts = await context
            .Thoughts.Where(x => x.UserId == claimsPrincipal.GetUserId())
            .Select(x => new Response(
                x.Id,
                x.User.Username,
                x.Title,
                x.Content,
                x.IsPublic,
                x.CreatedAtUtc,
                x.UpdatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(thoughts);
    }
}
