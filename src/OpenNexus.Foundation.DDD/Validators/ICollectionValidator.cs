using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD.Validators;

/// <summary>
/// Validator interface for validating items against a set of existing items.
/// Used in conjunction with DomainSet or its derivatives when adding new items.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface ISetValidator<T>
{
    /// <summary>
    /// Validates the given item against the current items in the set.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="currentItems"></param>
    /// <returns></returns>
    public Result Validate(T item, IReadOnlySet<T> currentItems);
}
