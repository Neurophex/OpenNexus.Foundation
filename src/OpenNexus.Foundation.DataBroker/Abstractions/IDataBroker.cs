using OpenNexus.Foundation.Policy.Abstractions;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DataBroker.Abstractions;

/// <summary>
/// Defines a data broker for managing data projections.
/// </summary>
/// <typeparam name="TProjection"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public interface IDataBroker<TProjection, TIdentity>
    where TProjection : class, IDataProjection
    where TIdentity : notnull
{
    /// <summary>
    /// Gets a data projection by its identity.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="userContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<TProjection>> GetByIdAsync(TIdentity id, IUserContext userContext, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all data projections with optional query options.
    /// </summary>
    /// <param name="userContext"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Result<IEnumerable<TProjection>>> GetAllAsync(IUserContext userContext, IDataQueryOptions? options, CancellationToken cancellationToken = default);
}
