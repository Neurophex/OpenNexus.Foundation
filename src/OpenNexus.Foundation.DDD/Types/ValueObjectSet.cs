using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD;

/// <summary>
/// A set of value objects
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ValueObjectSet<T> : DomainSetBase<T>
    where T : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObjectSet{T}"/> class.
    /// </summary>
    public ValueObjectSet() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueObjectSet{T}"/> class.
    /// </summary>
    /// <param name="items">Populates the set with the given items</param>
    public ValueObjectSet(params ISetValidator<T>[] items) : base(items) { }

    /// <summary>
    /// Creates a new instance of the <see cref="ValueObjectSet{T}"/> class with validation.
    /// Validates each item using the provided set validators before adding them to the set.
    /// If any item fails validation, an error result is returned.
    /// </summary>
    /// <param name="items"></param>
    /// <param name="setValidators"></param>
    /// <returns></returns>
    public static Result<ValueObjectSet<T>> CreateValidated(IEnumerable<T> items, params ISetValidator<T>[] setValidators)
    {
        return CreateValidated<ValueObjectSet<T>>(items, setValidators);
    }
}
