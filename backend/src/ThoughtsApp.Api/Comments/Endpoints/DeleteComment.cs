using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Comments;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Comments.Endpoints;

public class DeleteComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete("/{thoughtId}/comments/{commentId}", Handle)
            .WithSummary("Deletes comment by id on thought by id")
            .WithRequestValidation<Request>()
            .WithEnsureEntityIsAccessible<Thought, Request>(x => x.ThoughtId);
    }

    public record Request(Guid ThoughtId, Guid CommentId);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.CommentId).NotEmpty().WithMessage("{PropertyName} is required.");
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
            .Comments.Where(x =>
                x.Id == request.CommentId
                && x.ThoughtId == request.ThoughtId
                && x.UserId == claimsPrincipal.GetUserId()
            )
            .ExecuteDeleteAsync(cancellationToken);

        if (rowsDeleted == 0)
            return TypedResults.NotFound();

        return TypedResults.NoContent();
    }
}
