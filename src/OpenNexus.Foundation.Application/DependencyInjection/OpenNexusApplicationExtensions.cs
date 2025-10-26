using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenNexus.Foundation.Application;

/// <summary>
/// Extension methods for initializing application services.
/// </summary>
public static class OpenNexusApplicationExtensions
{
    /// <summary>
    /// Initializes application services by invoking all registered IAppInitializer implementations.
    /// </summary>
    /// <param name="provider"></param>
    public static async Task InitializeApplicationServices(this IServiceProvider provider)
    {
        // Create a logger for logging initialization progress
        var logger = provider.GetService<ILoggerFactory>()?.CreateLogger("AppInitialization");

        // Retrieve all registered IAppInitializer services
        var initializers = provider.GetServices<IServiceInitializer>().ToList();

        logger?.LogInformation("Found {InitializerCount} application initializers.", initializers.Count);

        // Invoke the Initialize method on each initializer
        var tasks = initializers.Select(async initializer =>
        {
            var initializerType = initializer.GetType().FullName;
            logger?.LogInformation("Starting initializer: {InitializerType}", initializerType);
            try
            {
                await initializer.Initialize(provider);
                logger?.LogInformation("Completed initializer: {InitializerType}", initializerType);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Initializer {InitializerType} failed with exception.", initializerType);
                throw;
            }
        });

        // Wait for all initializers to complete
        await Task.WhenAll([.. tasks]);

        // Log completion of all initializers
        logger?.LogInformation("All application initializers have completed.");
    }
}
