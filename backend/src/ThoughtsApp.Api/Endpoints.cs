using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ThoughtsApp.Api.Authentication.Endpoints;
using ThoughtsApp.Api.Comments.Endpoints;
using ThoughtsApp.Api.Comments.Endpoints.Admin;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Common.Filters;
using ThoughtsApp.Api.Reactions.Endpoints;
using ThoughtsApp.Api.Stats;
using ThoughtsApp.Api.Thoughts.Endpoints;
using ThoughtsApp.Api.Thoughts.Endpoints.Admin;
using ThoughtsApp.Api.Users;
using ThoughtsApp.Api.Users.Admin;

namespace ThoughtsApp.Api;

public static class Endpoints
{
    /// <summary>
    ///     Configures OpenApi security scheme for JWT
    /// </summary>
    private static readonly OpenApiSecurityScheme SecurityScheme = new()
    {
        Type = SecuritySchemeType.Http,
        Name = JwtBearerDefaults.AuthenticationScheme,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = JwtBearerDefaults.AuthenticationScheme,
        },
    };

    /// <summary>
    ///     Extension method to map every endpoint
    /// </summary>
    /// <param name="app">WebApplication</param>
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("/api/v1")
            .AddEndpointFilter<RequestLoggingFilter>()
            .WithOpenApi();

        endpoints.MapAuthenticationEndpoints();
        endpoints.MapThoughtEndpoints();
        endpoints.MapUserEndpoints();
        endpoints.MapAdminEndpoints();
        endpoints.MapStatEndpoints();
    }

    /// <summary>
    ///     Maps authentication endpoints
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    private static void MapAuthenticationEndpoints(this RouteGroupBuilder builder)
    {
        // todo | add cleanup service to remove unused refresh tokens
        // to prevent "database bloating"

        var endpoints = builder.MapGroup("/auth").WithTags("Auth");

        endpoints
            .MapPublicGroup()
            .MapEndpoint<Signup>()
            .MapEndpoint<Login>()
            .MapEndpoint<RenewToken>();
    }

    /// <summary>
    ///     Maps thought endpoints
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    private static void MapThoughtEndpoints(this RouteGroupBuilder builder)
    {
        var endpoints = builder.MapGroup("/thoughts").WithTags("Thoughts");

        // thought endpoints
        endpoints
            .MapAuthorizedGroup()
            .MapEndpoint<CreateThought>()
            .MapEndpoint<UpdateThought>()
            .MapEndpoint<DeleteThought>();

        endpoints
            .MapPublicGroup()
            .MapEndpoint<GetPublicThoughts>()
            .MapEndpoint<GetThoughtsByUserId>()
            .MapEndpoint<GetThoughtById>();

        // reaction endpoints
        endpoints
            .MapAuthorizedGroup()
            .MapEndpoint<GetReactions>()
            .MapEndpoint<GetReactionsById>()
            .MapEndpoint<UpsertReaction>()
            .MapEndpoint<DeleteReaction>();

        // comment endpoints
        endpoints
            .MapAuthorizedGroup()
            .MapEndpoint<GetComments>()
            .MapEndpoint<GetComment>()
            .MapEndpoint<CreateComment>()
            .MapEndpoint<UpdateComment>()
            .MapEndpoint<DeleteComment>();
    }

    private static void MapUserEndpoints(this RouteGroupBuilder builder)
    {
        var endpoints = builder.MapGroup("/users").WithTags("Users");

        // public user endpoints
        endpoints.MapPublicGroup().MapEndpoint<GetUser>();

        // authorized user endpoints
        endpoints.MapAuthorizedGroup().MapEndpoint<UpdateUser>().MapEndpoint<DeleteUser>();
    }

    private static void MapAdminEndpoints(this RouteGroupBuilder builder)
    {
        var endpoints = builder.MapAdminOnlyGroup("/admin").WithTags("Admin");
        var thoughts = endpoints.MapGroup("/thoughts");

        // thoughts
        thoughts
            .MapEndpoint<GetThoughtsAdmin>()
            .MapEndpoint<GetThoughtByIdAdmin>()
            .MapEndpoint<GetThoughtsByUserIdAdmin>()
            .MapEndpoint<GetThoughtsByTitleAdmin>()
            .MapEndpoint<GetThoughtsByUsername>()
            .MapEndpoint<UpdateThoughtAdmin>()
            .MapEndpoint<DeleteThoughtAdmin>();

        // comments
        thoughts.MapEndpoint<DeleteCommentAdmin>().MapEndpoint<GetCommentsAdmin>();

        // users
        endpoints
            .MapGroup("/users")
            .MapEndpoint<GetUsersAdmin>()
            .MapEndpoint<GetUserByUsernameAdmin>()
            .MapEndpoint<DeleteUserAdmin>();
    }

    private static void MapStatEndpoints(this RouteGroupBuilder builder)
    {
        var endpoints = builder.MapAdminOnlyGroup("/stats").WithTags("Stats");
        endpoints.MapEndpoint<GetContentDistributionStats>();

        // users
        var users = endpoints.MapGroup("/users");
        users.MapEndpoint<GetUserGrowthStats>().MapEndpoint<GetMostActiveUsers>();

        // thoughts
        var thoughts = endpoints.MapGroup("/thoughts");
        thoughts.MapEndpoint<GetThoughtActivityStats>();
        thoughts.MapEndpoint<GetMostPopularThoughts>();

        // reactions
        var reactions = endpoints.MapGroup("/reactions");
        reactions.MapEndpoint<GetReactionDistributionStats>();
    }

    private static RouteGroupBuilder MapAdminOnlyGroup(
        this RouteGroupBuilder builder,
        string? prefix = null
    )
    {
        return builder
            .MapGroup(prefix ?? string.Empty)
            .RequireAuthorization("AdminOnly")
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Security = [new OpenApiSecurityRequirement { [SecurityScheme] = [] }],
            });
    }

    /// <summary>
    ///     Extension method to create public route groups
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    /// <param name="prefix">Prefix for the group</param>
    /// <returns></returns>
    private static RouteGroupBuilder MapPublicGroup(
        this RouteGroupBuilder builder,
        string? prefix = null
    )
    {
        return builder.MapGroup(prefix ?? string.Empty).AllowAnonymous();
    }

    /// <summary>
    ///     Extension method to create authorized route groups
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    /// <param name="prefix">Prefix for the group</param>
    private static RouteGroupBuilder MapAuthorizedGroup(
        this RouteGroupBuilder builder,
        string? prefix = null
    )
    {
        return builder
            .MapGroup(prefix ?? string.Empty)
            .RequireAuthorization()
            .WithOpenApi(operation => new OpenApiOperation(operation)
            {
                Security = [new OpenApiSecurityRequirement { [SecurityScheme] = [] }],
            });
    }

    /// <summary>
    ///     Extension method to register the endpoints defined in the specified endpoint class
    /// </summary>
    /// <param name="routeBuilder">The route builder on which the endpoints will be mapped</param>
    /// <typeparam name="TEndpoint">
    ///     The type (class) that implements the IEndpoint interface and contains the route mapping
    ///     logic
    /// </typeparam>
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(
        this IEndpointRouteBuilder routeBuilder
    )
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(routeBuilder);
        return routeBuilder;
    }
}
