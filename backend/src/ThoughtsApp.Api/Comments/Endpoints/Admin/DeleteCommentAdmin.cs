using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Comments.Endpoints.Admin;

public class DeleteCommentAdmin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapDelete("/{thoughtId}/comments/{commentId}", Handle)
            .WithSummary("Deletes comment by id on thought by id")
            .WithRequestValidation<Request>();
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

    private static async Task<NoContent> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        await db
            .Comments.Where(x => x.Id == request.CommentId && x.ThoughtId == request.ThoughtId)
            .ExecuteDeleteAsync(cancellationToken);

        return TypedResults.NoContent();
    }
}
