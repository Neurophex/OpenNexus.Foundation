namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for an event dispatcher that handles domain events.
/// </summary>
public interface IEventDispatcher
{
    /// <summary>
    /// Dispatches a domain event.
    /// </summary>
    /// <param name="event">The domain event to dispatch.</param>
    Task Dispatch(IDomainEvent @event, CancellationToken ct = default);

    /// <summary>
    /// Dispatches a collection of domain events.
    /// </summary>
    /// <param name="events">The collection of domain events to dispatch.</param>
    Task Dispatch(IEnumerable<IDomainEvent> events, CancellationToken ct = default);
}
