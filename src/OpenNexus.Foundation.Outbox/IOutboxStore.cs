using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Outbox;

/// <summary>
/// Interface for an outbox store that handles storing and retrieving outbox messages.
/// </summary>
public interface IOutboxStore
{
    /// <summary>
    /// Enqueues a new outbox message for processing.
    /// </summary>
    /// <param name="message">The message to add to te Outbox</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>A result</returns>
    Task<Result> EnqueueAsync(OutboxMessage message, CancellationToken ct = default);

    /// <summary>
    /// Reserves a batch of outbox messages for processing.
    /// </summary>
    /// <param name="batchSize">The maximum size of a batch to reserve</param>
    /// <param name="leaseTime">The amount of time the batch is reserved for</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>A result containing a read only list containing the batched messages if the operation was a success.</returns>
    Task<Result<IReadOnlyList<OutboxMessage>>> ReserveBatchAsync(int batchSize, TimeSpan leaseTime, CancellationToken ct = default);

    /// <summary>
    /// Marks an outbox message as processed.
    /// </summary>
    /// <param name="messageId">The id of the message</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Result> MarkAsProcessedAsync(Guid messageId, CancellationToken ct = default);

    /// <summary>
    /// Marks an outbox message as failed.
    /// </summary>
    /// <param name="messageId">The id of the message</param>
    /// <param name="errorMessage">The error that occurred</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Result> MarkAsFailedAsync(Guid messageId, string errorMessage, CancellationToken ct = default);

    /// <summary>
    /// Releases the lease on a message, making it available for processing again.
    /// </summary>
    /// <param name="messageId">The id of the message to release to lease on</param>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Result> ReleaseLeaseAsync(Guid messageId, CancellationToken ct = default);

    /// <summary>
    /// Releases leases on all messages that have expired leases.
    /// </summary>
    /// <param name="ct"></param>
    /// <returns></returns>
    Task<Result> ReleaseExpiredLeasesAsync(CancellationToken ct = default);
}
