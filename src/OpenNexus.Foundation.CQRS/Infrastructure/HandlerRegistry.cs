using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Registry for command and query handlers in the CQRS pattern.
/// </summary>
internal sealed class HandlerRegistry : IHandlerRegistry
{
    private readonly object _queryLock = new();
    private readonly object _commandLock = new();

    /// <summary>
    /// Dictionary mapping command types to their handler types.
    /// </summary>
    private readonly Dictionary<Type, Type> _commandHandlers;

    /// <summary>
    /// Dictionary mapping query types to their handler types.
    /// </summary>
    private readonly Dictionary<Type, Type> _queryHandlers;

    /// <summary>
    /// Initializes a new instance of the <see cref="HandlerRegistry"/> class.
    /// </summary>
    public HandlerRegistry()
    {
        _commandHandlers = [];
        _queryHandlers = [];
    }

    /// <summary>
    /// Gets the handler type for the specified command type.
    /// </summary>
    /// <param name="commandType"></param>
    /// <param name="handlerType"></param>
    internal void RegisterCommandHandler(Type commandType, Type handlerType)
    {
        lock (_commandLock) _commandHandlers[commandType] = handlerType;
    }

    /// <summary>
    /// Gets the handler type for the specified query type.
    /// </summary>
    /// <param name="queryType"></param>
    /// <param name="handlerType"></param>
    internal void RegisterQueryHandler(Type queryType, Type handlerType)
    {
        lock(_queryLock) _queryHandlers[queryType] = handlerType;
    }

    /// <summary>
    /// Registers all command and query handlers found in the specified assembly.
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="logger"></param>
    internal void RegisterHandlersFromAssembly(Assembly assembly, ILogger? logger = null)
    {
        // Keep track of the number of registered handlers
        int count = 0;

        // Scan the assembly for command handlers
        assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(ti => ti.iface.IsGenericType && ti.iface.GetGenericTypeDefinition() == typeof(ICommandHandler<,>))
            .ToList()
            .ForEach(ti =>
            {
                // Register the command handler
                var commandType = ti.iface.GetGenericArguments()[0];
                var handlerType = ti.type;
                RegisterCommandHandler(commandType, handlerType);
                logger?.LogInformation("Registered command handler: {HandlerType} for command: {CommandType}", handlerType.FullName, commandType.FullName);
                count++;
            });

        logger?.LogInformation("Registered {HandlerCount} command handlers from assembly: {AssemblyName}", count, assembly.FullName);

        // Reset count for query handlers
        count = 0;

        // Scan the assembly for query handlers
        assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (type, iface) => new { type, iface })
            .Where(ti => ti.iface.IsGenericType && ti.iface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            .ToList()
            .ForEach(ti =>
            {
                // Register the query handler
                var queryType = ti.iface.GetGenericArguments()[0];
                var handlerType = ti.type;
                RegisterQueryHandler(queryType, handlerType);
                logger?.LogInformation("Registered query handler: {HandlerType} for query: {QueryType}", handlerType.FullName, queryType.FullName);
                count++;
            });

        logger?.LogInformation("Registered {HandlerCount} query handlers from assembly: {AssemblyName}", count, assembly.FullName);

        logger?.LogInformation("Completed registering CQRS handlers from assembly: {AssemblyName}", assembly.FullName);
    }

    /// <summary>
    /// Creates an instance of the command handler for the given command type.
    /// </summary>
    /// <param name="commandType">The command type to resolve a handler for.</param>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    /// <returns>An instantiated handler for the specified command.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the command type.</exception>
    public object CreateCommandHandler(Type commandType, IServiceProvider provider)
    {
        if (!_commandHandlers.TryGetValue(commandType, out var handlerType))
            throw new InvalidOperationException($"No command handler registered for command type {commandType.FullName}");

        return ActivatorUtilities.CreateInstance(provider, handlerType);
    }

    /// <summary>
    /// Creates an instance of the query handler for the given query type.
    /// </summary>
    /// <param name="queryType">The query type to resolve a handler for.</param>
    /// <param name="provider">The service provider used to resolve dependencies.</param>
    /// <returns>An instantiated handler for the specified query.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no handler is registered for the query type.</exception>
    public object CreateQueryHandler(Type queryType, IServiceProvider provider)
    {
        if (!_queryHandlers.TryGetValue(queryType, out var handlerType))
            throw new InvalidOperationException($"No query handler registered for query type {queryType.FullName}");

        return ActivatorUtilities.CreateInstance(provider, handlerType);
    }
}
