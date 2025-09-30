using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD.Tests.Types;

/// <summary>
/// A validator that always succeeds
/// </summary>
/// <typeparam name="T"></typeparam>
internal sealed class AlwaysValidValidator<T> : IValidator<T>
{
    public Result Validate(T value) => Result.Success();
}

/// <summary>
/// A validator that ensures a string is not empty
/// </summary>
internal sealed class NotEmptyStringValidator : IValidator<string>
{
    public Result Validate(string value)
        => string.IsNullOrWhiteSpace(value)
            ? Result.Error("Value cannot be empty.")
            : Result.Success();
}

/// <summary>
/// A validator that ensures a string does not exceed a maximum length
/// </summary>
internal sealed class MaxLengthValidator : IValidator<string>
{
    private readonly int _max;
    public MaxLengthValidator(int max) => _max = max;
    public Result Validate(string value)
        => value.Length <= _max
            ? Result.Success()
            : Result.Error($"Value exceeds {_max} characters.");
}