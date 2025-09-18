using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.Outbox;

/// <summary>
/// Interface for dispatching outbox messages.
/// </summary>
public interface IOutboxDispatcher
{
    /// <summary>
    /// Publishes a message to the external system (queue, bus, API, etc.).
    /// Should not throw exceptions, but return a Result indicating success or failure.
    /// </summary>
    /// <param name="message">The message to dispatch to the external system</param>
    /// <param name="ct">The cancellation token</param>
    /// <returns>
    /// A result that is success when the message has succesfully been dispatched.
    /// An error result when it hasnt.
    /// </returns>
    Task<Result> DispatchAsync(OutboxMessage message, CancellationToken ct = default);
}