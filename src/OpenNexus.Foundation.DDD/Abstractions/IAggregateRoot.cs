namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for aggregate roots in the domain.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public interface IAggregateRoot<TIdentity> : IEntity<TIdentity>, IEntityNode, IDomainEventSource, IChangeTracker, IDomainEventCollector
{
    
}