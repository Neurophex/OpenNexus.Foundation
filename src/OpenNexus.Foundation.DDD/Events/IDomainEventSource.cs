namespace OpenNexus.Foundation.DDD.Events;

/// <summary>
/// Interface for a domain event source.
/// </summary>
public interface IDomainEventSource
{
    /// <summary>
    /// Gets the collection of domain events.
    /// </summary>
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }

    /// <summary>
    /// Clears the domain events.
    /// </summary>
    void ClearDomainEvents();

    /// <summary>
    /// Adds a domain event to the source.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    void RaiseDomainEvent(IDomainEvent domainEvent);

    /// <summary>
    /// Checks if there are any domain events registered in the source.
    /// </summary>
    /// <returns>True if there are events registered with this entity</returns>
    bool HasEvents();
}