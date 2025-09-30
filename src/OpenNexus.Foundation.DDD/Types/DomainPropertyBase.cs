using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD.Types;

/// <summary>
/// Base class for domain properties that encapsulate a value along with its validation logic
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class DomainPropertyBase<T>
    where T : notnull
{
    // The validators for the domain property
    protected readonly IReadOnlyList<IValidator<T>> _validators;

    /// <summary>
    /// Creates a new instance of the <see cref="DomainPropertyBase{T}"/> class.
    /// </summary>
    /// <param name="validators">The validators to use for validating the value</param>
    protected DomainPropertyBase(IEnumerable<IValidator<T>> validators)
    {
        // Store the validators
        _validators = [.. validators];
    }

    /// <summary>
    /// Gets the value of the domain property
    /// </summary>
    public abstract T Value { get; }

    /// <summary>
    /// Validates a value using all configured validators.
    /// Throws if invalid when used from constructor.
    /// </summary>
    protected static void ThrowIfInvalid(T value, IEnumerable<IValidator<T>> validators)
    {
        // Validate the value using all validators
        foreach (var validator in validators)
        {
            // If any validator fails, return the error
            var result = validator.Validate(value);
            if (!result.IsSuccess) throw new ArgumentException($"Value is not valid: {result.GetErrorMessage()}");
        }
    }

    /// <summary>
    /// Validates a value using all configured validators.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    protected static Result Validate(T value, IEnumerable<IValidator<T>> validators)
    {
        // Validate the value using all validators
        foreach (var validator in validators)
        {
            // If any validator fails, return the error
            var result = validator.Validate(value);
            if (!result.IsSuccess) return Result.Error(result.GetErrorMessage());
        }

        return Result.Success();
    }

    /// <summary>
    /// Implicit conversion to the underlying type
    /// </summary>
    /// <param name="p"></param>
    public static implicit operator T(DomainPropertyBase<T> p) => p.Value;

    /// <summary>
    /// Returns a string that represents the current object.
    /// </summary>
    /// <returns></returns>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}