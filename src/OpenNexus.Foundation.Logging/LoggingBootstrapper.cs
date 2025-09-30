using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Grafana.Loki;

namespace OpenNexus.Foundation.Logging;

/// <summary>
/// Bootstrapper class to configure logging for the application.
/// </summary>
public static class LoggingBootstrapper
{
    /// <summary>
    /// Configures Serilog logging with console and optional Grafana Loki sinks.
    /// </summary>
    /// <param name="hostBuilder">The hostbuilder</param>
    /// <returns></returns>
    public static IHostBuilder UseOpenNexusLogging(this IHostBuilder hostBuilder)
    {
        // Configure Serilog as the logging provider
        hostBuilder.UseSerilog((context, services, cnfg) =>
        {
            // Get environment and Loki configuration settings
            var env = context.HostingEnvironment;
            var lokiUrl = context.Configuration["Logging:Loki:Url"];
            var lokiUsername = context.Configuration["Logging:Loki:Username"];
            var lokiPassword = context.Configuration["Logging:Loki:Password"];

            // Configure Serilog sinks and enrichers
            cnfg.MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithThreadId()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("Application", env.ApplicationName)
                .Enrich.WithProperty("Environment", env.EnvironmentName);

            // Configure to write to the console
            cnfg.WriteTo.Console();

            // If Loki URL is provided, configure Loki sink
            if (!string.IsNullOrWhiteSpace(lokiUrl))
            {
                // Define labels for Loki
                var lokiLabels = new[]
                {
                    new LokiLabel { Key = "Application", Value = env.ApplicationName },
                    new LokiLabel { Key = "Environment", Value = env.EnvironmentName },
                };

                // Configure Loki credentials if username and password are provided
                var lokiCredentials = !string.IsNullOrWhiteSpace(lokiUsername) && !string.IsNullOrWhiteSpace(lokiPassword)
                    ? new LokiCredentials
                    {
                        Login = lokiUsername,
                        Password = lokiPassword
                    }
                    : null;

                // Configure Serilog to write to Grafana Loki
                cnfg.WriteTo.GrafanaLoki(lokiUrl, credentials: lokiCredentials, labels: lokiLabels);
            }

            // Read additional configuration from appsettings.json
            cnfg.ReadFrom.Configuration(context.Configuration);
        });

        // Return the configured host builder
        return hostBuilder;
    }
}