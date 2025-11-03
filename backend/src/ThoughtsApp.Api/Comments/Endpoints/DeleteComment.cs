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
            .WithEnsureEntityExistsFilter<Thought, Request>(x => x.ThoughtId)
            .WithEnsureUserOwnsEntity<Comment, Request>(x => x.CommentId);
    }

    public record Request(Guid ThoughtId, Guid CommentId);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("ThoughtId is required.");
            RuleFor(x => x.CommentId).NotEmpty().WithMessage("CommentId is required.");
        }
    }

    private static async Task<Results<Ok, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        await db
            .Comments.Where(x => x.ThoughtId == request.ThoughtId)
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.Ok();
    }
}
