namespace OpenNexus.Foundation.Persistence;

/// <summary>
/// Interface for a participant in a Unit of Work pattern.
/// </summary>
public interface IUnitOfWorkParticipant
{
    /// <summary>
    /// Prepares the participant for saving changes asynchronously.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task PrepareAsync(CancellationToken ct = default);

    /// <summary>
    /// Commits the changes made by this participant asynchronously.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken ct = default);

    /// <summary>
    /// Rolls back the changes made by this participant asynchronously.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task RollbackAsync(CancellationToken ct = default);
}