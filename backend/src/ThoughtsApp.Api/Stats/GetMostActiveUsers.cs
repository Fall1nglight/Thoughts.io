using System.Security.Cryptography.Xml;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Stats;

public class GetMostActiveUsers : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .Map("/leaderboard", Handle)
            .WithSummary("Get most active users")
            .WithRequestValidation<Request>();
    }

    public record Request(int Limit = 10);

    public record Reaction(int Id, int Count);

    public record Stats(int ThoughtCount, int CommentCount, List<Reaction> Reactions);

    public record User(Guid Id, string Username, Stats Stats);

    public record Response(List<User> Users);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 30)
                .WithMessage("{PropertyName} must be between {From} and {To}.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var usrs = await db
            .Users.OrderByDescending(x => x.Thoughts.Count + x.Comments.Count + x.Reactions.Count)
            .Take(request.Limit)
            .Select(x => new User(
                x.Id,
                x.Username,
                new Stats(
                    x.Thoughts.Count,
                    x.Comments.Count,
                    x.Reactions.GroupBy(r => r.ReactionId)
                        .Select(g => new Reaction(g.Key, g.Count()))
                        .ToList()
                )
            ))
            .ToListAsync(cancellationToken);

        var response = new Response(usrs);
        return TypedResults.Ok(response);
    }
}
