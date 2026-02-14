using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Stats;

public class GetThoughtActivityStats : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapGet("/activity", Handle)
            .WithSummary("Gets thought activity stats for a given peroid of time")
            .WithRequestValidation<Request>();
    }

    public record Request(int Days = 30);

    public record DataPoint(DateTime DateUtc, int Count);

    public record Response(List<DataPoint> Creations);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Days)
                .InclusiveBetween(1, 365)
                .WithMessage("{PropertyName} must be between {From} and {To}.");
        }
    }

    private static async Task<Ok<Response>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var cutOffDate = DateTime.UtcNow.Date.AddDays(-request.Days);

        var growthStats = await db
            .Thoughts.Where(x => x.CreatedAtUtc >= cutOffDate)
            .GroupBy(x => x.CreatedAtUtc.Date)
            .Select(g => new DataPoint(g.Key, g.Count()))
            .ToListAsync(cancellationToken);

        var response = new Response(growthStats);
        return TypedResults.Ok(response);
    }
}
