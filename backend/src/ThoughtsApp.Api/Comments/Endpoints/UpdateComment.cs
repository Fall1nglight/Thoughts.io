using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Comments;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Comments.Endpoints;

public class UpdateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPut("/{thoughtId}/comments/{commentId}", Handle)
            .WithSummary("Updates a comment")
            .WithRequestValidation<Request>()
            .WithEnsureEntityExistsFilter<Thought, Request>(x => x.ThoughtId)
            .WithEnsureUserOwnsEntity<Comment, Request>(x => x.CommentId);
    }

    public record Body(string Content);

    public record Request(Guid ThoughtId, Guid CommentId, [FromBody] Body Body);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.CommentId).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Body.Content)
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MinimumLength(5)
                .WithMessage("{PropertyName} must be at least {MinLength} characters long.")
                .MaximumLength(150)
                .WithMessage("{PropertyName} must not exceed {MaxLength} characters.");
        }
    }

    private static async Task<Ok> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var comment = await db.Comments.SingleAsync(
            x => x.Id == request.CommentId,
            cancellationToken
        );

        comment.Content = request.Body.Content.Trim();
        comment.UpdatedAtUtc = DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok();
    }
}
