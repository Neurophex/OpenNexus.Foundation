using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Types;

/// <summary>
/// A domain property that encapsulates a value along with its validation logic
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class DomainProperty<T> : DomainPropertyBase<T>
    where T : notnull
{
    // The value of the domain property
    private T _value;

    /// <summary>
    /// Creates a new instance of the <see cref="DomainProperty{T}"/> class.
    /// </summary>
    /// <param name="initialValue">The initial value to assign to the property</param>
    /// <param name="validators">The validators to use for validating the value</param>
    /// <exception cref="ArgumentException">Thrown if the initial value is not valid</exception>
    private DomainProperty(T initialValue, IEnumerable<IValidator<T>> validators)
        : base(validators)
    {
        // If all validators pass, set the initial value
        _value = initialValue;
    }

    /// <summary>
    /// Gets the value of the domain property
    /// </summary>
    public override T Value => _value;

    /// <summary>
    /// Sets the value of the domain property after validating it
    /// </summary>
    /// <param name="newValue"></param>
    /// <returns></returns>
    public Result SetValue(T newValue)
    {
        // Validate the new value using all validators
        var validationResult = Validate(newValue, _validators);
        if (!validationResult.IsSuccess) return validationResult;

        // If all validators pass, set the new value and return success
        _value = newValue;
        return Result.Success();
    }

    /// <summary>
    /// Tries to set the value of the domain property after validating it
    /// </summary>
    /// <param name="newValue"></param>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public bool TrySetValue(T newValue, out string? errorMessage)
    {
        // Try to set the value and capture the result
        var result = SetValue(newValue);

        // If setting the value failed, set the error message
        errorMessage = result.IsSuccess ? null : result.GetErrorMessage();

        // Return whether setting the value was successful
        return result.IsSuccess;
    }
    
    /// <summary>
    /// Creates a new instance of the <see cref="DomainProperty{T}"/> class.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validators"></param>
    /// <returns></returns>
    public static Result<DomainProperty<T>> Create(T value, params IValidator<T>[] validators)
    {
        // Validate the property
        var validationResult = Validate(value, validators);

        // Check if the property is valid, return error if not
        if (!validationResult.IsSuccess) return Result<DomainProperty<T>>.Error(validationResult.GetErrorMessage());

        // Create and return the property
        var property = new DomainProperty<T>(value, validators);
        return property;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DomainProperty{T}"/> class or throws if invalid.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="validators"></param>
    /// <returns></returns>
    public static DomainProperty<T> CreateOrThrow(T value, params IValidator<T>[] validators)
    {
        // This will throw if invalid
        ThrowIfInvalid(value, validators);

        // Create the property
        var property = new DomainProperty<T>(value, validators);

        // Return the property
        return property;
    }
}
