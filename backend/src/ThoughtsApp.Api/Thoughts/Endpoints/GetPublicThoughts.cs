using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Thoughts.Endpoints;

public class GetPublicThoughts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder.MapGet("/public", Handle).WithSummary("Gets public thoughts");
    }

    // todo | maybe don't expose UserId and User?
    public record Response(
        Guid Id,
        string Username,
        string Title,
        string Content,
        bool IsPublic,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    public record User(Guid Id, string Username);

    private static async Task<Ok<List<Response>>> Handle(AppDbContext context)
    {
        var publicThoughts = await context
            .Thoughts.Where(t => t.IsPublic)
            .Select(t => new Response(
                t.Id,
                t.User.Username,
                t.Title,
                t.Content,
                t.IsPublic,
                t.CreatedAtUtc,
                t.UpdatedAtUtc
            ))
            .ToListAsync();

        return TypedResults.Ok(publicThoughts);
    }
}
