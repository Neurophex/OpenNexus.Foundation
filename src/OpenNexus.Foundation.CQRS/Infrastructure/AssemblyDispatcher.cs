using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Dispatcher for handling commands and queries in the CQRS pattern.
/// </summary>
public class AssemblyDispatcher : IDispatcher
{
    public static string HandlerAsyncMethodName = "HandleAsync";

    private readonly IServiceProvider _serviceProvider;
    private readonly IHandlerRegistry _registry;
    private readonly ILogger<AssemblyDispatcher> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dispatcher"/> class with the specified service provider.
    /// </summary>
    /// <param name="provider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public AssemblyDispatcher(IServiceProvider provider)
    {
        _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider), "Service provider cannot be null.");
        _registry = provider.GetRequiredService<IHandlerRegistry>();
        _logger = provider.GetService<ILogger<AssemblyDispatcher>>() ?? NullLogger<AssemblyDispatcher>.Instance;
    }

    /// <summary>
    /// Dispatches a command and returns a response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<TResponse>> Command<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default) where TResponse : notnull
    {
        // Log the command dispatching
        _logger.LogDebug("Dispatching command of type {CommandType}", command.GetType().FullName);

        try
        {
            // Create the command handler instance using the handler registry
            var handlerInstance = _registry.CreateCommandHandler(command.GetType(), _serviceProvider);

            var handleMethod = handlerInstance.GetType().GetMethod(HandlerAsyncMethodName, [command.GetType(), typeof(CancellationToken)]) ??
                throw new InvalidOperationException($"HandleAsync method not found on handler for command type {command.GetType().FullName}");

            var task = handleMethod.Invoke(handlerInstance, [command, cancellationToken]) as Task<Result<TResponse>> ??
                throw new InvalidOperationException($"HandleAsync method did not return expected Task<Result<{typeof(TResponse).FullName}>>");

            // Log that the command has been dispatched
            _logger.LogDebug("Command of type {CommandType} dispatched successfully", command.GetType().FullName);

            // Await the task and return the result
            var result = await task.ConfigureAwait(false);

            // Log the result of the command handling
            _logger.LogDebug("Command of type {CommandType} handled with result: {Result}", command.GetType().FullName, result.IsSuccess ? "Success" : "Failure");

            // Return the result
            return Result<TResponse>.Success(result.Value);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An error occurred while dispatching command of type {CommandType}", command.GetType().FullName);

            // Return an error result
            return Result<TResponse>.Error(ex.Message);
        }
    }

    /// <summary>
    /// Dispatches a query and returns a response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default) where TResponse : notnull
    {
        // Log the query dispatching
        _logger.LogDebug("Dispatching query of type {QueryType}", query.GetType().FullName);

        try
        {
            // Create the query handler instance using the handler registry
            var handlerInstance = _registry.CreateQueryHandler(query.GetType(), _serviceProvider);

            var handleMethod = handlerInstance.GetType().GetMethod(HandlerAsyncMethodName, [query.GetType(), typeof(CancellationToken)]) ??
                throw new InvalidOperationException($"HandleAsync method not found on handler for query type {query.GetType().FullName}");

            var task = handleMethod.Invoke(handlerInstance, [query, cancellationToken]) as Task<Result<TResponse>> ??
                throw new InvalidOperationException($"HandleAsync method did not return expected Task<Result<{typeof(TResponse).FullName}>>");

            // Log that the query has been dispatched
            _logger.LogDebug("Query of type {QueryType} dispatched successfully", query.GetType().FullName);

            // Await the task and return the result
            var result = await task.ConfigureAwait(false);

            // Log the result of the command handling
            _logger.LogDebug("Query of type {QueryType} handled with result: {Result}", query.GetType().FullName, result.IsSuccess ? "Success" : "Failure");

            return Result<TResponse>.Success(result.Value);
        }
        catch (Exception ex)
        {
            // Log the exception
            _logger.LogError(ex, "An error occurred while dispatching query of type {QueryType}", query.GetType().FullName);

            // Return an error result
            return Result<TResponse>.Error(ex.Message);
        }
    }

    /// <summary>
    /// Invokes the command pipeline for a given command and returns the response.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    // private async Task<Result<TResponse>> InvokeCommandPipeline<TCommand, TResponse>(TCommand command, CancellationToken ct)
    //     where TCommand : ICommand<TResponse>
    //     where TResponse : notnull
    // {
    //     // Get the command handler from the service provider
    //     // Throw an exception if the handler is not registered
    //     var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>() ?? throw new HandlerNotRegisteredException(
    //         typeof(TCommand),
    //         typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResponse))
    //     );

    //     // Get the pipeline behaviors from the service provider
    //     var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TCommand, TResponse>>().ToList();

    //     // Define the terminal delegate that will invoke the command handler
    //     PipelineDelegate<TResponse> terminal = (ct) => handler.HandleAsync(command, ct);

    //     // Create the pipeline by chaining the behaviors in reverse order
    //     var pipeline = behaviors
    //         .Reverse<IPipelineBehavior<TCommand, TResponse>>()
    //         .Aggregate(terminal, (next, behavior) => (ct) => behavior.HandleAsync(command, next, ct));

    //     // Call the pipeline and return the result
    //     return await pipeline(ct);
    // }

    /// <summary>
    /// Invokes the query pipeline for a given query and returns the response.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    // private async Task<Result<TResponse>> InvokeQueryPipeline<TQuery, TResponse>(TQuery query, CancellationToken ct)
    //     where TQuery : IQuery<TResponse>
    //     where TResponse : notnull
    // {
    //     // Get the command handler from the service provider
    //     // Throw an exception if the handler is not registered
    //     var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResponse>>() ?? throw new HandlerNotRegisteredException(
    //         typeof(TQuery),
    //         typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResponse))
    //     );

    //     // Get the pipeline behaviors from the service provider
    //     var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TQuery, TResponse>>().ToList();

    //     // Define the terminal delegate that will invoke the query handler
    //     PipelineDelegate<TResponse> terminal = (ct) => handler.HandleAsync(query, ct);

    //     // Create the pipeline by chaining the behaviors in reverse order
    //     var pipeline = behaviors
    //         .Reverse<IPipelineBehavior<TQuery, TResponse>>()
    //         .Aggregate(terminal, (next, behavior) => (ct) => behavior.HandleAsync(query, next, ct));

    //     // Call the pipeline and return the result
    //     return await pipeline(ct);
    // }
}
