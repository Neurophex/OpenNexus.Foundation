using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Indicates that an entity can be created from a data model.
/// </summary>
/// <typeparam name="TSelf"></typeparam>
/// <typeparam name="TDataModel"></typeparam>
public interface ICreateableEntity<TSelf, TDataModel>
    where TDataModel : class
    where TSelf : ICreateableEntity<TSelf, TDataModel>
{
    /// <summary>
    /// Creates an instance of the entity from the given data model.
    /// </summary>
    /// <param name="dataModel"></param>
    /// <returns></returns>
    static abstract Result<TSelf> Create(TDataModel dataModel);
}