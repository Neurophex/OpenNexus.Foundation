using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OpenNexus.Foundation.Outbox;

/// <summary>
/// Bootstrapper class to configure outbox processing for the application.
/// </summary>
public static class OutboxBootstrapper
{
    /// <summary>
    /// Configures the outbox processing services including the outbox store, dispatcher, and worker.
    /// </summary>
    /// <typeparam name="TDispatcher">The dispatcher implementation.</typeparam>
    /// <typeparam name="TStore">The store implementation.</typeparam>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection AddOutbox<TDispatcher, TStore>(this IServiceCollection services, IConfiguration configuration)
        where TDispatcher : class, IOutboxDispatcher
        where TStore : class, IOutboxStore
    {
        // Register the outbox store and dispatcher implementations
        services.Configure<OutboxWorkerOptions>(configuration.GetSection("Outbox"));

        // Register the outbox store and dispatcher implementations
        services.AddSingleton<IOutboxStore, TStore>();
        services.AddSingleton<IOutboxDispatcher, TDispatcher>();

        // Register the outbox worker as a hosted service
        services.AddHostedService<OutboxWorker>();

        // Return the modified service collection
        return services;
    }
}
