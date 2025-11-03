using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Reactions.Endpoints;

public class DeleteReaction : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete("/{thoughtId}/reactions", Handle)
            .WithSummary("Deletes authorized user's reaction of a thought by id")
            .WithRequestValidation<Request>()
            .WithEnsureEntityExistsFilter<Thought, Request>(x => x.ThoughtId);
    }

    public record Request(Guid ThoughtId);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("ThoughtId is required.");
        }
    }

    private static async Task<Results<Ok, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        // todo | move this to filter
        var isThoughtPublic = await db
            .Thoughts.Where(x => x.Id == request.ThoughtId)
            .Select(x => x.IsPublic)
            .SingleAsync(cancellationToken);

        if (!isThoughtPublic)
            return TypedResults.NotFound();

        await db
            .ThoughtReactions.Where(x =>
                x.ThoughtId == request.ThoughtId && x.UserId == claimsPrincipal.GetUserId()
            )
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
