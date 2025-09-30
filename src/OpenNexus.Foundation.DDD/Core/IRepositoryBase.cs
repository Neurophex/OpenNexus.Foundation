using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD.Core;

/// <summary>
/// Interface for a generic repository base.
/// </summary>
/// <typeparam name="TRoot"></typeparam>
/// <typeparam name="TIdentity"></typeparam>
public interface IRepositoryBase<TRoot, TIdentity> where TRoot : AggregateRoot<TIdentity>
{
    /// <summary>
    /// Asynchronously retrieves an entity by its identifier.
    /// </summary>
    /// <param name="id">The id to retrieve the root with</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    public Task<Result<TRoot>> GetByIdAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<IReadOnlyList<TRoot>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously adds a new entity to the repository.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<TRoot>> AddAsync(TRoot root, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously updates an existing entity in the repository.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<TRoot>> UpdateAsync(TRoot root, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously deletes an entity by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result> DeleteAsync(TIdentity id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously checks if an entity exists by its identifier.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Result<bool>> ExistsAsync(TIdentity id, CancellationToken cancellationToken = default);
}
