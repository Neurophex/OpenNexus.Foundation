using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.DDD.Validators;

namespace OpenNexus.Foundation.DDD.Tests.Types;

public class DomainPropertyTests
{
    [Fact]
    public void Create_ReturnsSuccess_WhenInitialValueIsValid()
    {
        var validators = new IValidator<string>[] {
                new NotEmptyStringValidator(),
                new MaxLengthValidator(10)
            };

        var result = DomainProperty<string>.Create("Hello", validators);

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello", result.Value.Value);
    }

    [Fact]
    public void Create_ReturnsError_WhenInitialValueIsInvalid()
    {
        var validators = new IValidator<string>[] {
                new NotEmptyStringValidator(),
                new MaxLengthValidator(5)
            };

        var result = DomainProperty<string>.Create("Too long for five", validators);

        Assert.False(result.IsSuccess);
        Assert.Contains("exceeds", result.GetErrorMessage());
    }

    [Fact]
    public void CreateOrThrow_ReturnsInstance_WhenValueIsValid()
    {
        var validators = new IValidator<int>[] { new AlwaysValidValidator<int>() };

        var property = DomainProperty<int>.CreateOrThrow(42, validators);

        Assert.Equal(42, property.Value);
        int primitive = property;           // implicit operator
        Assert.Equal(42, primitive);
    }

    [Fact]
    public void CreateOrThrow_ThrowsArgumentException_WhenValueIsInvalid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };

        var ex = Assert.Throws<ArgumentException>(
            () => DomainProperty<string>.CreateOrThrow("", validators));

        Assert.Contains("Value cannot be empty", ex.Message);
    }

    [Fact]
    public void SetValue_UpdatesValue_WhenNewValueIsValid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };
        var prop = DomainProperty<string>.CreateOrThrow("Initial", validators);

        var result = prop.SetValue("Updated");

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", prop.Value);
    }

    [Fact]
    public void SetValue_ReturnsError_WhenNewValueIsInvalid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };
        var prop = DomainProperty<string>.CreateOrThrow("Valid", validators);

        var result = prop.SetValue("");

        Assert.False(result.IsSuccess);
        Assert.Equal("Valid", prop.Value); // value should not change
    }

    [Fact]
    public void TrySetValue_ReturnsTrueAndNullError_WhenValid()
    {
        var validators = new IValidator<int>[] { new AlwaysValidValidator<int>() };
        var prop = DomainProperty<int>.CreateOrThrow(1, validators);

        var success = prop.TrySetValue(2, out var error);

        Assert.True(success);
        Assert.Null(error);
        Assert.Equal(2, prop.Value);
    }

    [Fact]
    public void TrySetValue_ReturnsFalseAndError_WhenInvalid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };
        var prop = DomainProperty<string>.CreateOrThrow("Valid", validators);

        var success = prop.TrySetValue("", out var error);

        Assert.False(success);
        Assert.NotNull(error);
        Assert.Equal("Valid", prop.Value); // unchanged
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var validators = new IValidator<string>[] { new AlwaysValidValidator<string>() };
        var prop = DomainProperty<string>.CreateOrThrow("Kepler", validators);

        Assert.Equal("Kepler", prop.ToString());
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var validators = new IValidator<string>[] { new AlwaysValidValidator<string>() };
        var prop = DomainProperty<string>.CreateOrThrow("Galaxy", validators);

        string value = prop; // implicit conversion
        Assert.Equal("Galaxy", value);
    }
}