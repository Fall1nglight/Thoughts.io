using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints.Admin;

public class GetThoughtsByTitleAdmin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/title/{title}", Handle)
            .WithSummary("Gets thoughts by title")
            .WithRequestValidation<Request>();
    }

    public record Request(string Title);

    public record User(Guid Id, string Username);

    public record Comment(int Count);

    public record Reaction(int Id, int Count);

    public record Thought(
        Guid Id,
        User User,
        string Title,
        string Content,
        bool IsPublic,
        Comment Comments,
        List<Reaction> Reactions,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    public record Response(List<Thought> Thoughts);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("{PropertyName} is required.")
                .MinimumLength(3)
                .WithMessage("{PropertyName} must be at least {MinLength} characters long.")
                .MaximumLength(50)
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
        var userThoughts = await db
            .Thoughts.Where(t => t.Title.ToLower().Contains(request.Title.ToLower()))
            .AsNoTracking()
            .Select(t => new Thought(
                t.Id,
                new User(t.UserId, t.User.Username),
                t.Title,
                t.Content,
                t.IsPublic,
                new Comment(t.Comments.Count),
                t.Reactions.GroupBy(tr => tr.Reaction.Id)
                    .Select(group => new Reaction(group.Key, group.Count()))
                    .ToList(),
                t.CreatedAtUtc,
                t.UpdatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        var response = new Response(userThoughts);

        return TypedResults.Ok(response);
    }
}
