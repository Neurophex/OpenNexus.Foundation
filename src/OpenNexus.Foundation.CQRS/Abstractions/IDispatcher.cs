using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS;

/// <summary>
/// Dispatches a CQRS request to its appropriate handler and pipeline.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Dispatches a query to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TResponse>> Query<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default) where TResponse : notnull;

    /// <summary>
    /// Dispatches a command to its handler and returns the result.
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TResponse>> Command<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default) where TResponse : notnull;
}
