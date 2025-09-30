using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS.Core;

/// <summary>
/// Delegate for a pipeline behavior in the CQRS pattern.
/// </summary>
/// <typeparam name="TResponse"></typeparam>
/// <param name="cancellationToken"></param>
/// <returns></returns>
public delegate Task<Result<TResponse>> PipelineDelegate<TResponse>(CancellationToken cancellationToken) where TResponse : notnull;

/// <summary>
/// Interface for a pipeline behavior in the CQRS pattern.
/// </summary>
public interface IPipelineBehavior<TRequest, TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the request and allows for additional processing before and after the request is handled.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TResponse>> HandleAsync(TRequest request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken);
}
