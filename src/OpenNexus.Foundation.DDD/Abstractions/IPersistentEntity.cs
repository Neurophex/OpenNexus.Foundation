using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Indicates that an entity can be persisted to a data model.
/// </summary>
/// <typeparam name="TDataModel"></typeparam>
public interface IPersistentEntity<TSelf, TDataModel>
    where TDataModel : class
    where TSelf : IPersistentEntity<TSelf, TDataModel>
{
    /// <summary>
    /// Converts the entity to its corresponding data model representation.
    /// </summary>
    /// <returns>The data model object</returns>
    public TDataModel ToDataModel();

    /// <summary>
    /// Rehydrates an entity from the given data model.
    /// </summary>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    static abstract Result<TSelf> Rehydrate(TDataModel dataModel);
}
