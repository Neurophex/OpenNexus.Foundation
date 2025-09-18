using OpenNexus.Foundation.DDD.Events;

namespace OpenNexus.Foundation.DDD.Core;

/// <summary>
/// Base class for all entities in the domain.
/// Entities are mutable and defined by their identity.
/// </summary>
public abstract class Entity<TIdentity> : IDomainEventSource
{
    /// <summary>
    /// A collection of domain events that have occurred on this entity.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public TIdentity Id { get; private init; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Entity()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        // Parameterless constructor for EF Core
        // This is required for EF Core to create instances of this class
        // when materializing entities from the database.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{TIdentity}"/> class with the specified identity.
    /// </summary>
    /// <param name="identity"></param>
    public Entity(TIdentity identity)
    {
        Id = identity;
    }

    /// <summary>
    /// Returns a string representation of the entity, including its type and identity.
    /// </summary>
    /// <returns>A string with the template {typeName} [Id: {Id}]</returns>
    public override string ToString()
    {
        var myType = GetType();
        var typeName = myType.Name;
        return $"{typeName} [Id: {Id}]";
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current entity.
    /// This works by fist checking if the object is null, then checking if it is the same instance,
    /// and finally checking if the Ids are equal.
    /// </summary>
    /// <param name="obj">The object to compare to</param>
    /// <returns>True if the object type is the same and the id's match.</returns>
    public override bool Equals(object? obj)
    {
        // Check if the object is null
        if (obj is null)
        {
            return false;
        }

        // Check if the object is the same instance
        if (obj is not Entity<TIdentity> other)
        {
            return false;
        }

        // Check if the Ids are equal
        return Id?.Equals(other.Id) ?? false;
    }

    /// <summary>
    /// Returns a hash code for the entity.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        // Use the Id's hash code to generate a hash code for the entity
        return Id?.GetHashCode() ?? 0;
    }

    /// <summary>
    /// Adds a domain event to the entity's domain events collection.
    /// </summary>
    /// <param name="domainEvent"></param>
    public void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes all domain events registered to this entity.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    /// <summary>
    /// Checks if there are any domain events registered with this entity.
    /// </summary>
    /// <returns>True if the amount of events on this entity is > 0.</returns>
    public bool HasEvents() => _domainEvents.Count > 0;

    /// <summary>
    /// Gets all the domain events that have occurred on this entity.
    /// </summary>
    /// <returns>A read-only collection of domain events.</returns>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();
}
