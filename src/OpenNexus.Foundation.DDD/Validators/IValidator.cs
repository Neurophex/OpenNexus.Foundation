using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Interface for a validator
/// </summary>
public interface IValidator<T>
{
    /// <summary>
    /// Validates the given value
    /// </summary>
    /// <param name="value">The value to validate</param>
    /// <returns>A result indicating whether the validation was successful or not</returns>
    Result Validate(T value);
}