namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for tracking changes in an entity.
/// </summary>
public interface IChangeTracker
{
    /// <summary>
    /// Indicates whether the entity has uncommitted changes.
    /// </summary>
    public bool IsDirty { get; }

    /// <summary>
    /// Marks the entity as clean, indicating that all changes have been committed.
    /// </summary>
    public void AcceptChanges();
}
