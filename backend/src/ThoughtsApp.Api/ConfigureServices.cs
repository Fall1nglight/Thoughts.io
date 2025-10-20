using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ThoughtsApp.Api.Common.ExceptionHandlers;
using ThoughtsApp.Api.Data.Shared;

namespace ThoughtsApp.Api;

internal static class ConfigureServices
{
    // todo | order functions by calling order
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSerilog();
        builder.AddSwagger();
        builder.AddProblemDetails();
        builder.AddExceptionHandlers();
        builder.AddJsonHandling();
        builder.AddCors();
        builder.AddDatabase();
        // passwordhasher
        // fluentvalidation
        // jwtauthentication
        // refreshtoken provider
        // fusion cache
    }

    /// <summary>
    /// Configures Serilog then adds it to the DI container
    /// </summary>
    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog(
            (services, loggerConfiguration) =>
            {
                // load configuration from appsettings.json
                loggerConfiguration
                    .ReadFrom.Configuration(builder.Configuration)
                    // integrates Serilog with DI
                    // this is required for custom sinks or enrichers that
                    //  depennd on application services
                    .ReadFrom.Services(services);
            }
        );
    }

    /// <summary>
    /// Configures SwaggerAPI, then adds it to the DI container
    /// </summary>
    private static void AddSwagger(this WebApplicationBuilder builder)
    {
        // add endpoints to OpenAPI doc
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(options =>
        {
            // fix nested class names
            options.CustomSchemaIds(type => type.FullName?.Replace('+', '.'));

            // generate security schemes for OpenAPI doc
            options.InferSecuritySchemes();
        });
    }

    /// <summary>
    /// Configures JSON handling (disallows unmapped members, then adds it to the DI container
    /// </summary>
    private static void AddJsonHandling(this WebApplicationBuilder builder)
    {
        builder.Services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
        });
    }

    /// <summary>
    /// Adds ProblemDetails service to the DI container for consistent error responses
    /// </summary>
    private static void AddProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails();
    }

    /// <summary>
    /// Adds CORS to the DI container
    /// </summary>
    private static void AddCors(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors();
    }

    /// <summary>
    /// Configures AppDbContext, then adds it to the DI container
    /// </summary>
    private static void AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<AppDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(
                builder.Configuration.GetConnectionString("ThoughtsAppDatabase")
            );
        });
    }

    private static void AddExceptionHandlers(this WebApplicationBuilder builder)
    {
        // builder.Services.AddExceptionHandler<JsonExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    }
}
