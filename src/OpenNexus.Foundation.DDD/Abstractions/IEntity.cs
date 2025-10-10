using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for entities in the domain.
/// </summary>
/// <typeparam name="TIdentity"></typeparam>
public interface IEntity<TIdentity>
{
    /// <summary>
    /// The unique identifier for the entity.
    /// </summary>
    public TIdentity Id { get; }
}