namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Base class for aggregate roots in the domain.
/// </summary>
/// <typeparam name="TIdentity">The type of Id for the given AggregateRoot</typeparam>
public abstract class AggregateRootBase<TIdentity> : EntityBase<TIdentity>, IAggregateRoot<TIdentity>
{
    // Flag that indicates if the aggregate root has uncommitted changes
    private bool _isDirty = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="AggregateRootBase{TIdentity}"/> class with a specified identity.
    /// </summary>
    /// <param name="id"></param>
    public AggregateRootBase(TIdentity id) : base(id) { }

    /// <summary>
    /// Collect all domain events raised by the aggregate root and its entities.
    /// This also clears the events from the entities.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyCollection<IDomainEvent> PullDomainEvents()
    {
        // Get all the event sources in the aggregate root's hierarchy
        var sources = CollectEventSourcesRecursively(this);

        // Collect events from all sources
        var events = sources.SelectMany(s => s.DomainEvents).ToList();

        // Clear events from all sources
        foreach (var source in sources) source.ClearDomainEvents();

        // Return the events
        return events;
    }

    /// <summary>
    /// Marks the aggregate root as dirty, indicating it has uncommitted changes.
    /// </summary>
    protected void MarkDirty()
    {
        _isDirty = true;
    }

    /// <summary>
    /// Indicates whether the aggregate root has uncommitted changes.
    /// </summary>
    public bool IsDirty => _isDirty;

    /// <summary>
    /// Collect all domain events raised by the aggregate root and its entities without clearing them.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IDomainEvent> PollDomainEvents()
    {
        return [.. CollectEventSourcesRecursively(this).SelectMany(s => s.DomainEvents)];
    }

    /// <summary>
    /// Accepts all changes made to the aggregate root and its entities, clearing any raised domain events.
    /// Also marks the aggregate root as not dirty.
    /// </summary>
    public void AcceptChanges()
    {
        // Loop through all event sources and clear their events
        foreach (var source in CollectEventSourcesRecursively(this)) source.ClearDomainEvents();

        // Mark the aggregate root as not dirty
        _isDirty = false;
    }

    /// <summary>
    /// Recursively collects all event sources in the aggregate root's hierarchy.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private static IEnumerable<IDomainEventSource> CollectEventSourcesRecursively(IEntityNode node)
    {
        // If the node is an event source, yield it
        if (node is IDomainEventSource source)
        {
            yield return source;
        }

        // Recursively process child nodes
        foreach (var child in node.GetChildNodes())
        {
            // Recursively collect event sources from child nodes
            foreach (var e in CollectEventSourcesRecursively(child)) yield return e;
        }
    }
}
