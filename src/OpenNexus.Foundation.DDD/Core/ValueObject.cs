using System.Globalization;

namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Base class for all value objects in the domain.
/// Value objects are immutable and defined by their properties.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Returns a string representation of the value object, including its type and properties.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object? obj)
    {
        // Check if the object is null or not of the same type
        if (obj is null || obj.GetType() != GetType())
            return false;

        // Cast the object to ValueObject
        var other = obj as ValueObject;

        // Compare the equality components of the two value objects
        return Equals(other);
    }

    /// <summary>
    /// Returns a hash code for the value object based on its equality components.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        // Use a hash code combining the hash codes of the equality components
        return GetEqualityComponents()
            .Aggregate(0, (current, component) => HashCode.Combine(current, component?.GetHashCode() ?? 0));
    }

    /// <summary>
    /// Returns a string representation of the value object, including its type and equality components.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // Get the type of the current instance and its name
        var myType = GetType();
        var typeName = myType.Name;
        
        // Return a string with the type name and the equality components
        return $"{typeName} [{string.Join(", ", GetEqualityComponents().Select(x => Convert.ToString(x, CultureInfo.InvariantCulture)))}]";
    }

    /// <summary>
    /// Gets all the components that make up the value object.
    /// </summary>
    /// <returns>An enumerable of the different equality components</returns>
    public abstract IEnumerable<object?> GetEqualityComponents();

    /// <summary>
    /// Checks if the current value object is equal to another value object.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(ValueObject? other)
    {
        // Check if the other object is null or not of the same type
        if (other is null)
            return false;

        // If the other object is null, return false
        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <summary>
    /// Overloaded equality operator to compare two value objects for equality.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        // If both are null, they are equal
        if (left is null && right is null)
            return true;

        // If one is null and the other is not, they are not equal
        if (left is null || right is null)
            return false;

        // Use the Equals method to compare the two value objects
        return left.Equals(right);
    }

    /// <summary>
    /// Overloaded inequality operator to compare two value objects for inequality.
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        // Use the equality operator to determine if they are not equal
        return !(left == right);
    }
}

