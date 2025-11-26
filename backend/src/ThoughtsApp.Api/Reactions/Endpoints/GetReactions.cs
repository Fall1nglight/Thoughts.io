using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Reactions.Endpoints;

public class GetReactions : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/{thoughtId}/reactions", Handle)
            .WithSummary("Gets all reactions by thoughtId")
            .WithRequestValidation<Request>()
            .WithEnsureEntityIsAccessible<Thought, Request>(x => x.ThoughtId);
    }

    public record Request(Guid ThoughtId);

    public record User(Guid Id, string Username);

    public record Reaction(int Id, List<User> Users);

    public record Response(Guid ThoughtId, List<Reaction> Reactions);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var reactions = await db
            .ThoughtReactions.Where(x => x.ThoughtId == request.ThoughtId)
            .GroupBy(tr => tr.ReactionId)
            .Select(group => new Reaction(
                group.Key,
                group.Select(tr => new User(tr.UserId, tr.User.Username)).ToList()
            ))
            .ToListAsync(cancellationToken);

        var response = new Response(request.ThoughtId, reactions);
        return TypedResults.Ok(response);
    }
}
