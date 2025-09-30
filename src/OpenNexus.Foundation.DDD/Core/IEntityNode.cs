namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Indicates that an entity has child nodes.
/// </summary>
public interface IEntityNode
{
    /// <summary>
    /// Gets the child nodes of this entity.
    /// </summary>
    /// <returns></returns>
    IEnumerable<IEntityNode> GetChildNodes();
}
