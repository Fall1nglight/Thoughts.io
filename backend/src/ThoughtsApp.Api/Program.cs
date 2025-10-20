using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using ThoughtsApp.Api;
using ThoughtsApp.Api.Data.Shared;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting application...");

    var builder = WebApplication.CreateBuilder(args);
    builder.AddServices();

    var app = builder.Build();
    await app.Configure();

    app.MapGet(
        "/",
        async (AppDbContext db) =>
        {
            return TypedResults.Ok(await db.Thoughts.Where(t => t.IsPublic).ToListAsync());
        }
    );

    app.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
