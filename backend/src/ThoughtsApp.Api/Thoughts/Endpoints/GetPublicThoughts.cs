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
        builder.MapGet("/", Handle).WithSummary("Gets public thoughts");
    }

    public record Response(
        Guid Id,
        Guid UserId,
        string Title,
        string Content,
        bool IsPublic,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc,
        User User
    );

    public record User(Guid Id, string Username);

    private static async Task<Ok<List<Response>>> Handle(AppDbContext context)
    {
        var publicThoughts = await context
            .Thoughts.Where(t => t.IsPublic)
            .Select(t => new Response(
                t.Id,
                t.UserId,
                t.Title,
                t.Content,
                t.IsPublic,
                t.CreatedAtUtc,
                t.UpdatedAtUtc,
                new User(t.User.Id, t.User.Username)
            ))
            .ToListAsync();
        return TypedResults.Ok(publicThoughts);
    }
}
