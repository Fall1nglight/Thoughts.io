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
            .MapDelete("/{thoughtId}/reactions/user", Handle)
            .WithSummary("Deletes authorized user's reaction of a thought by id")
            .WithRequestValidation<Request>()
            .WithEnsureEntityIsAccessible<Thought, Request>(x => x.ThoughtId);
    }

    public record Request(Guid ThoughtId);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Results<NoContent, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var rowsDeleted = await db
            .ThoughtReactions.Where(x =>
                x.ThoughtId == request.ThoughtId && x.UserId == claimsPrincipal.GetUserId()
            )
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted == 0)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }
}
