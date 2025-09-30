using Microsoft.Extensions.DependencyInjection;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Dispatcher for handling commands and queries in the CQRS pattern.
/// </summary>
public class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dispatcher"/> class with the specified service provider.
    /// </summary>
    /// <param name="provider"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public Dispatcher(IServiceProvider provider)
    {
        _serviceProvider = provider ?? throw new ArgumentNullException(nameof(provider), "Service provider cannot be null.");
    }

    /// <summary>
    /// Dispatches a command and returns a response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<Result<TResponse>> DispatchCommand<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default) where TResponse : notnull
    {
        // Get the method info for the InvokeCommandPipeline method
        var method = typeof(Dispatcher).GetMethod(nameof(InvokeCommandPipeline), global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance);

        // Make the method generic with the specific command type and response type
        var closed = method!.MakeGenericMethod(command.GetType(), typeof(TResponse));

        // Invoke the command pipeline with the provided command and cancellation token
        return (Task<Result<TResponse>>)closed.Invoke(this, [command, cancellationToken])!;
    }

    /// <summary>
    /// Dispatches a query and returns a response.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task<Result<TResponse>> DispatchQuery<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default) where TResponse : notnull
    {
        // Get the method info for the InvokeQueryPipeline method
        var method = typeof(Dispatcher).GetMethod(nameof(InvokeQueryPipeline), global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance);

        // Make the method generic with the specific query type and response type
        var closed = method!.MakeGenericMethod(query.GetType(), typeof(TResponse));

        // Invoke the query pipeline with the provided query and cancellation token
        return (Task<Result<TResponse>>)closed.Invoke(this, [query, cancellationToken])!;
    }

    /// <summary>
    /// Invokes the command pipeline for a given command and returns the response.
    /// </summary>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="command"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task<Result<TResponse>> InvokeCommandPipeline<TCommand, TResponse>(TCommand command, CancellationToken ct)
        where TCommand : ICommand<TResponse>
        where TResponse : notnull
    {
        // Get the command handler from the service provider
        // Throw an exception if the handler is not registered
        var handler = _serviceProvider.GetService<ICommandHandler<TCommand, TResponse>>() ?? throw new HandlerNotRegisteredException(
            typeof(TCommand),
            typeof(ICommandHandler<,>).MakeGenericType(typeof(TCommand), typeof(TResponse))
        );

        // Get the pipeline behaviors from the service provider
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TCommand, TResponse>>().ToList();

        // Define the terminal delegate that will invoke the command handler
        PipelineDelegate<TResponse> terminal = (ct) => handler.HandleAsync(command, ct);

        // Create the pipeline by chaining the behaviors in reverse order
        var pipeline = behaviors
            .Reverse<IPipelineBehavior<TCommand, TResponse>>()
            .Aggregate(terminal, (next, behavior) => (ct) => behavior.HandleAsync(command, next, ct));

        // Call the pipeline and return the result
        return await pipeline(ct);
    }

    /// <summary>
    /// Invokes the query pipeline for a given query and returns the response.
    /// </summary>
    /// <typeparam name="TQuery"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="query"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    private async Task<Result<TResponse>> InvokeQueryPipeline<TQuery, TResponse>(TQuery query, CancellationToken ct)
        where TQuery : IQuery<TResponse>
        where TResponse : notnull
    {
        // Get the command handler from the service provider
        // Throw an exception if the handler is not registered
        var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TResponse>>() ?? throw new HandlerNotRegisteredException(
            typeof(TQuery),
            typeof(IQueryHandler<,>).MakeGenericType(typeof(TQuery), typeof(TResponse))
        );

        // Get the pipeline behaviors from the service provider
        var behaviors = _serviceProvider.GetServices<IPipelineBehavior<TQuery, TResponse>>().ToList();

        // Define the terminal delegate that will invoke the query handler
        PipelineDelegate<TResponse> terminal = (ct) => handler.HandleAsync(query, ct);

        // Create the pipeline by chaining the behaviors in reverse order
        var pipeline = behaviors
            .Reverse<IPipelineBehavior<TQuery, TResponse>>()
            .Aggregate(terminal, (next, behavior) => (ct) => behavior.HandleAsync(query, next, ct));

        // Call the pipeline and return the result
        return await pipeline(ct);
    }
}
