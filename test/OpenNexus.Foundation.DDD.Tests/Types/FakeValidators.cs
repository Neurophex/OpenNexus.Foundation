using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Tests.Types;

internal sealed class AlwaysValidValidator<T> : IValidator<T>
{
    public Result Validate(T value) => Result.Success();
}

internal sealed class NotEmptyStringValidator : IValidator<string>
{
    public Result Validate(string value)
        => string.IsNullOrWhiteSpace(value)
            ? Result.Error("Value cannot be empty.")
            : Result.Success();
}

internal sealed class MaxLengthValidator : IValidator<string>
{
    private readonly int _max;
    public MaxLengthValidator(int max) => _max = max;
    public Result Validate(string value)
        => value.Length <= _max
            ? Result.Success()
            : Result.Error($"Value exceeds {_max} characters.");
}