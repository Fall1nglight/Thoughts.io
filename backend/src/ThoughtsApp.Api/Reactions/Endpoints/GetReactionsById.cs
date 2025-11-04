using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Reactions.Endpoints;

public class GetReactionsById : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/{thoughtId}/reactions/{reactionId}", Handle)
            .WithSummary("Gets all reactions by thoughtId")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid ThoughtId, int ReactionId);

    public record User(string Username);

    public record Response(int ReactionId, List<User> Users);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.ReactionId)
                .InclusiveBetween(1, 3)
                .WithMessage("{PropertyName} must be between {From} and {To}.");
        }
    }

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var thought = await db
            .Thoughts.Where(x => x.Id == request.ThoughtId)
            .Select(x => new { x.IsPublic, x.UserId })
            .SingleOrDefaultAsync(cancellationToken);

        if (thought == null)
            return TypedResults.NotFound();

        var userId = claimsPrincipal.GetUserId();

        if (!thought.IsPublic && thought.UserId != userId)
            return TypedResults.NotFound();

        var usernames = await db
            .ThoughtReactions.Where(x =>
                x.ThoughtId == request.ThoughtId && x.ReactionId == request.ReactionId
            )
            .Select(x => new User(x.User.Username))
            .ToListAsync(cancellationToken);

        var response = new Response(request.ReactionId, usernames);
        return TypedResults.Ok(response);
    }
}
