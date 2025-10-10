namespace OpenNexus.Foundation.DDD;

public interface IValueObject<TSelf> : IEquatable<TSelf>
    where TSelf : IValueObject<TSelf>
{
    /// <summary>
    /// Gets the components that define the equality of the value object.
    /// Each component contributes to equality and hashing.
    /// </summary>
    IEnumerable<object?> GetEqualityComponents();
}