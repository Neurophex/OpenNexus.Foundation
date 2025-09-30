namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for domain events.
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the domain where the event occurred.
    /// </summary>
    DateTime OccuredOn { get; }

    /// <summary>
    /// Gets the correlation ID for the event, if any.
    /// </summary>
    string? CorrelationId { get; }

    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    Guid EventId { get; }
}