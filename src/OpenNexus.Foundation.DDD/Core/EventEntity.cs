namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Base class for entities that produce domain events.
/// Inherits from Entity and implements IDomainEventSource.
/// </summary>
public class EventEntity<TIdentity> : Entity<TIdentity>, IDomainEventSource
{
    // The list of domain events produced by this entity
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Constructs a new EventProducingEntity with the given identity.
    /// </summary>
    /// <param name="identity"></param>
    public EventEntity(TIdentity identity) : base(identity) { }

    /// <summary>
    /// Checks if there are any domain events that have been raised on the entity.
    /// </summary>
    /// <returns></returns>
    protected bool HasEvents() => _domainEvents.Count > 0;

    /// <summary>
    /// Raises a new domain event and adds it to the list of events.
    /// </summary>
    /// <param name="domainEvent"></param>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    /// <summary>
    /// Gets the domain events that have been raised on the entity.
    /// This is a read-only collection.
    /// </summary>
    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Clears all domain events that have been raised on the entity.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
