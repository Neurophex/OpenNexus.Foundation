namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Base class for all entities in the domain.
/// Entities are mutable and defined by their identity.
/// </summary>
public abstract class Entity<TIdentity> : IEntityNode
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public TIdentity Id { get; private init; }

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
    /// Gets the child nodes of the entity.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerable<IEntityNode> GetChildNodes() => [];
}
