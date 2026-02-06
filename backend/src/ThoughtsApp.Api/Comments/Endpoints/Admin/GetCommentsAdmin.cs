using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Comments.Endpoints.Admin;

public class GetCommentsAdmin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("{thoughtId}/comments", Handle)
            .WithSummary("Gets all comments related to the given thought.")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid ThoughtId);

    public record User(Guid Id, string Username);

    public record Comment(
        Guid Id,
        string Content,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc,
        User User
    );

    public record Response(Guid ThoughtId, List<Comment> Comments);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.ThoughtId).NotEmpty().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        ClaimsPrincipal claimsPrincipal,
        CancellationToken cancellationToken
    )
    {
        var comments = await db
            .Comments.Where(x => x.ThoughtId == request.ThoughtId)
            .Select(x => new Comment(
                x.Id,
                x.Content,
                x.CreatedAtUtc,
                x.UpdatedAtUtc,
                new User(x.UserId, x.User.Username)
            ))
            .ToListAsync(cancellationToken);

        var response = new Response(request.ThoughtId, comments);
        return TypedResults.Ok(response);
    }
}
