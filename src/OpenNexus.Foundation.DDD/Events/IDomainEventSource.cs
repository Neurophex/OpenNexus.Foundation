namespace OpenNexus.Foundation.DDD.Events;

/// <summary>
/// Indicates that an entity can produce domain events.
/// </summary>
public interface IDomainEventSource
{
    /// <summary>
    /// A collection of domain events that have occurred on this source.
    /// </summary>
    IEnumerable<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears all domain events that have been raised on the source.
    /// </summary>
    void ClearDomainEvents();
}