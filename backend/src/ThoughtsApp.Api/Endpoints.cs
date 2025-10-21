using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using ThoughtsApp.Api.Common;
using ThoughtsApp.Api.Thoughts.Endpoints;

namespace ThoughtsApp.Api;

public static class Endpoints
{
    /// <summary>
    /// Configures OpenApi security scheme for JWT
    /// </summary>
    private static readonly OpenApiSecurityScheme SecurityScheme = new OpenApiSecurityScheme
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
    /// Extension method to map every endpoint
    /// </summary>
    /// <param name="app">WebApplication</param>
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoints = app.MapGroup("/api/v1").WithOpenApi();

        endpoints.MapThoughtEndpoints();
    }

    /// <summary>
    /// Maps authentication endpoints
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    private static void MapAuthenticationEndpoints(this RouteGroupBuilder builder)
    {
        var endpoints = builder.MapPublicGroup("/auth").WithTags("Auth");
    }

    /// <summary>
    /// Maps thought endpoints
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    private static void MapThoughtEndpoints(this RouteGroupBuilder builder)
    {
        var endpoints = builder.MapPublicGroup("/thoughts").WithTags("Thoughts");

        endpoints.MapEndpoint<GetPublicThoughts>();
    }

    /// <summary>
    /// Extension method to create public route groups
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    /// <param name="prefix">Prefix for the group</param>
    /// <returns></returns>
    private static RouteGroupBuilder MapPublicGroup(this RouteGroupBuilder builder, string? prefix)
    {
        return builder.MapGroup(prefix ?? string.Empty).AllowAnonymous();
    }

    /// <summary>
    /// Extension method to create authorized route groups
    /// </summary>
    /// <param name="builder">RouteGroupBuilder</param>
    /// <param name="prefix">Prefix for the group</param>
    private static RouteGroupBuilder MapAuthorizedGroup(
        this RouteGroupBuilder builder,
        string? prefix
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
    /// Extension method to register the endpoints defined in the specified endpoint class
    /// </summary>
    /// <param name="routeBuilder">The route builder on which the endpoints will be mapped</param>
    /// <typeparam name="TEndpoint">The type (class) that implements the IEndpoint interface and contains the route mapping logic</typeparam>
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(
        this IEndpointRouteBuilder routeBuilder
    )
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(routeBuilder);
        return routeBuilder;
    }
}
