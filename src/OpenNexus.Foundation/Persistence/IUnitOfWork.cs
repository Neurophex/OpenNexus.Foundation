namespace OpenNexus.Foundation.Persistence;

/// <summary>
/// Interface for a Unit of Work pattern implementation.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Enlists a participant in the Unit of Work.
    /// </summary>
    /// <param name="participant"></param>
    void Enlist(IUnitOfWorkParticipant participant);

    /// <summary>
    /// Prepares all participants for saving changes asynchronously.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken ct = default);
}
