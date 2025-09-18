using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Types;

/// <summary>
/// An immutable domain property that encapsulates a value along with its validation logic
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class ImmutableDomainProperty<T> : DomainPropertyBase<T>
    where T : notnull
{
    // The value of the domain property
    private readonly T _value;

    /// <summary>
    /// Creates a new instance of the <see cref="ImmutableDomainProperty{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value to assign to the property</param>
    /// <param name="validators">The validators to use for validating the value</param>
    /// <exception cref="ArgumentException">Thrown if the initial value is not valid</exception>
    private ImmutableDomainProperty(T initialValue, IEnumerable<IValidator<T>> validators)
        : base(validators)
    {
        // Store the value
        _value = initialValue;
    }

    /// <summary>
    /// Gets the value of the domain property
    /// </summary>
    public override T Value => _value;

    /// <summary>
    /// Creates a new instance of the <see cref="ImmutableDomainProperty{T}"/> class.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validators"></param>
    /// <returns></returns>
    public static Result<ImmutableDomainProperty<T>> Create(T value, params IValidator<T>[] validators)
    {
        // Validate the value using all validators
        var validationResult = Validate(value, validators);

        // Check if the value is valid, return error if not
        if (!validationResult.IsSuccess) return Result<ImmutableDomainProperty<T>>.Error(validationResult.GetErrorMessage());

        // Create and return the property
        var property = new ImmutableDomainProperty<T>(value, validators);
        return property;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ImmutableDomainProperty{T}"/> class or throws if invalid.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validators"></param>
    /// <returns></returns>
    public static ImmutableDomainProperty<T> CreateOrThrow(T value, params IValidator<T>[] validators)
    {
        // This will throw if invalid
        ThrowIfInvalid(value, validators);

        // Create the property
        var property = new ImmutableDomainProperty<T>(value, validators);

        // Return the property
        return property;
    }
}
