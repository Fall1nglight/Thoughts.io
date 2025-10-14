using Serilog;

namespace ThoughtsApp.Api;

internal static class ConfigureServices
{
    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.AddSerilog();
    }
    
    /// <summary>
    /// Configures Serilog then adds it to the DI container
    /// </summary>
    private static void AddSerilog(this WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog((services, loggerConfiguration) =>
        {
            // load configuration from appsettings.json
            loggerConfiguration.ReadFrom.Configuration(builder.Configuration)
                // integrates Serilog with DI
                // this is required for custom sinks or enrichers that
                //  depennd on application services
                .ReadFrom.Services(services);
        });
    }
}