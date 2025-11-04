using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using ThoughtsApp.Api.Authentication;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Comments;
using ThoughtsApp.Api.Data.Shared;
using ThoughtsApp.Api.Data.Thoughts;

namespace ThoughtsApp.Api.Comments.Endpoints;

public class CreateComment : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPost("/{thoughtId}/comments", Handle)
            .WithSummary("Create a new comment")
            .WithRequestValidation<Request>()
            .WithEnsureEntityExistsFilter<Thought, Request>(x => x.ThoughtId);
    }

    public record Body(string Content);

    public record Request(Guid ThoughtId, [FromBody] Body Body);

    public record Response(Guid CommentId);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");

            RuleFor(x => x.Body.Content)
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MinimumLength(5)
                .WithMessage("{PropertyName} must be at least {MinLength} characters long.")
                .MaximumLength(150)
                .WithMessage("{PropertyName} must not exceed {MaxLength} characters.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        // check if thought is private before proceeding

        var comment = new Comment
        {
            UserId = claimsPrincipal.GetUserId(),
            ThoughtId = request.ThoughtId,
            Content = request.Body.Content.Trim(),
        };

        await db.Comments.AddAsync(comment, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.Ok(new Response(comment.Id));
    }
}
