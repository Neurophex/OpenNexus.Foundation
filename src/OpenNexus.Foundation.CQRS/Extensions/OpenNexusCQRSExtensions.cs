using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenNexus.Foundation.Application;

namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Extension methods for registering CQRS services.
/// </summary>
public static class OpenNexusCQRSExtensions
{
    /// <summary>
    /// Adds CQRS services to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceCollection AddOpenNexusCQRS(this IServiceCollection services)
    {
        // Validate input parameters
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services), "Service collection cannot be null.");
        }

        // Register the AssemblyDispatcher as the implementation for IDispatcher
        services.AddScoped<IDispatcher, AssemblyDispatcher>();

        // Register the HandlerRegistry as the implementation for IHandlerRegistry
        services.AddSingleton<IHandlerRegistry, HandlerRegistry>();

        // Return the modified service collection
        return services;
    }

    /// <summary>
    /// Registers CQRS handlers from the specified assembly.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IServiceProvider UseCQRSHandlersFromAssembly(this IServiceProvider services, Assembly assembly)
    {
        // Validate input parameters
        if (services == null) throw new ArgumentNullException(nameof(services), "Service collection cannot be null.");
        if (assembly == null) throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");

        // Get the handler registry from the service provider
        var handlerRegistry = services.GetRequiredService<IHandlerRegistry>() as HandlerRegistry ??
            throw new InvalidOperationException("Handler registry is not properly registered.");
        
        // Create a logger for logging registration progress
        var logger = services.GetService<ILoggerFactory>()?.CreateLogger("CQRS.Extensions.Registration");

        // Register handlers from the specified assembly
        logger?.LogInformation("Registering CQRS handlers from assembly: {AssemblyName}", assembly.FullName);

        handlerRegistry.RegisterHandlersFromAssembly(assembly, logger);

        // Register the assembly containing CQRS handlers
        return services;
    }

    // public static IServiceCollection RegisterHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    // {
    //     // Validate input parameters
    //     if (services == null) throw new ArgumentNullException(nameof(services), "Service collection cannot be null.");
    //     if (assembly == null) throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");

    //     services.AddSingleton(_ => new HandlerAssemblySource(assembly));
    // }

    // public static IServiceCollection RegisterHandlersFromAssembly(this IServiceCollection services, Assembly assembly)
    // {
    //     // Validate input parameters
    //     if (services == null) throw new ArgumentNullException(nameof(services), "Service collection cannot be null.");
    //     if (assembly == null) throw new ArgumentNullException(nameof(assembly), "Assembly cannot be null.");

    //     // Get the 

    //     return services;
    // }
}
