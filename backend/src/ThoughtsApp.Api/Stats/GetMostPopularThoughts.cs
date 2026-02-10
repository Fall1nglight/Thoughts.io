using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Stats;

public class GetMostPopularThoughts : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/popular", Handle)
            .WithSummary("Get most popular thoughts")
            .WithRequestValidation<Request>();
    }

    public record Request(string? SortBy, int Limit = 10);

    public record User(Guid Id, string Username);

    public record Reaction(int Id, int Count);

    public record Thought(
        Guid Id,
        User User,
        string Title,
        string Content,
        bool IsPublic,
        int CommentCount,
        List<Reaction> Reactions,
        DateTime CreatedAtUtc,
        DateTime UpdatedAtUtc
    );

    public record Response(List<Thought> Thoughts);

    public enum SortByOptions
    {
        Total = 0,
        Comments = 1,
        Reactions = 2,
    }

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Limit)
                .InclusiveBetween(1, 30)
                .WithMessage("{PropertyName} must be between {From} and {To}.");

            When(
                x => !string.IsNullOrEmpty(x.SortBy),
                () =>
                {
                    RuleFor(x => x.SortBy)
                        .IsEnumName(typeof(SortByOptions), caseSensitive: false)
                        .WithMessage(
                            $"Invalid sort option. Allowed values: {string.Join(", ", Enum.GetNames<SortByOptions>())}"
                        );
                }
            );
        }
    }

    private static async Task<Ok<Response>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var query = db.Thoughts.AsQueryable();
        var sortByOption = SortByOptions.Total;

        if (!string.IsNullOrEmpty(request.SortBy))
            Enum.TryParse(request.SortBy, ignoreCase: true, out sortByOption);

        query = sortByOption switch
        {
            SortByOptions.Comments => query.OrderByDescending(x => x.Comments.Count),
            SortByOptions.Reactions => query.OrderByDescending(x => x.Reactions.Count),
            _ => query.OrderByDescending(x => x.Comments.Count + x.Reactions.Count),
        };

        var thoughts = await query
            .Take(request.Limit)
            .Select(x => new Thought(
                x.Id,
                new User(x.User.Id, x.User.Username),
                x.Title,
                x.Content,
                x.IsPublic,
                x.Comments.Count,
                x.Reactions.GroupBy(r => r.ReactionId)
                    .Select(g => new Reaction(g.Key, g.Count()))
                    .ToList(),
                x.CreatedAtUtc,
                x.CreatedAtUtc
            ))
            .ToListAsync(cancellationToken);

        var response = new Response(thoughts);
        return TypedResults.Ok(response);
    }
}
