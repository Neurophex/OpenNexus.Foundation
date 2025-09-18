using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.CQRS.Core;

/// <summary>
/// Interface for a command handler in the CQRS pattern.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IQueryHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
    where TResponse : notnull
{
    /// <summary>
    /// Handles the specified command request and returns a response.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TResponse>> HandleAsync(TQuery request, CancellationToken cancellationToken);
}