using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Reactions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Reactions.Endpoints;

public class UpsertReaction : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPut("/{thoughtId}/reactions", Handle)
            .WithSummary("Creates/updates reaction of the given thought by id")
            .WithRequestValidation<Request>()
            .WithEnsureEntityExistsFilter<Thought, Request>(x => x.ThoughtId);
    }

    public record Body(int ReactionId);

    public record Request(Guid ThoughtId, [FromBody] Body Body);

    // todo | make this dynamic
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("ThoughtId is required.");
            RuleFor(x => x.Body.ReactionId).GreaterThanOrEqualTo(1).LessThanOrEqualTo(3);
        }
    }

    private static async Task<Results<Ok, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var userId = claimsPrincipal.GetUserId();

        // todo | move this to filter
        var isThoughtPublic = await db
            .Thoughts.Where(x => x.Id == request.ThoughtId)
            .Select(x => x.IsPublic)
            .SingleAsync(cancellationToken);

        if (!isThoughtPublic)
            return TypedResults.NotFound();

        var reaction = await db.ThoughtReactions.SingleOrDefaultAsync(
            tr => tr.ThoughtId == request.ThoughtId && tr.UserId == userId,
            cancellationToken
        );

        // update the existing reaction
        if (reaction != null)
        {
            reaction.ReactionId = request.Body.ReactionId;
        }
        else
        {
            var newReaction = new ThoughtReaction
            {
                ThoughtId = request.ThoughtId,
                ReactionId = request.Body.ReactionId,
                UserId = userId,
            };

            await db.ThoughtReactions.AddAsync(newReaction, cancellationToken);
        }

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
