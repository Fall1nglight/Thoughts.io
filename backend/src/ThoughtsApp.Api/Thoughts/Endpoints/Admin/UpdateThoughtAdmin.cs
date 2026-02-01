using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Extensions;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api.Thoughts.Endpoints.Admin;

public class UpdateThoughtAdmin : IEndpoint
{
    public static void Map(IEndpointRouteBuilder builder)
    {
        builder
            .MapPut("/{id}", Handle)
            .WithSummary("Updates thought visibility")
            .WithRequestValidation<Request>();
    }

    public record Request(Guid Id, [FromBody] Body Body);

    public record Body(bool IsPublic);

    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("{PropertyName} is required.");
            RuleFor(x => x.Body.IsPublic).NotNull().WithMessage("{PropertyName} is required.");
        }
    }

    private static async Task<Results<NoContent, NotFound>> Handle(
        [AsParameters] Request request,
        AppDbContext db,
        CancellationToken cancellationToken
    )
    {
        var thought = await db
            .Thoughts.Where(x => x.Id == request.Id)
            .SingleOrDefaultAsync(cancellationToken);

        if (thought == null)
            return TypedResults.NotFound();

        thought.IsPublic = request.Body.IsPublic;

        await db.SaveChangesAsync(cancellationToken);
        return TypedResults.NoContent();
    }
}
