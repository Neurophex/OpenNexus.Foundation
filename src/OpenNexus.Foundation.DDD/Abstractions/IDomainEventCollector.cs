namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for collecting domain events from entities and aggregate roots.
/// </summary>
public interface IDomainEventCollector
{
    /// <summary>
    /// Collect all domain events raised by the entity and its children.
    /// This also clears the events from the entities.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<IDomainEvent> PullDomainEvents();

    /// <summary>
    /// Collect all domain events raised by the entity and its children without clearing them.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IDomainEvent> PollDomainEvents();
}