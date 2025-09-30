using OpenNexus.Foundation.DDD;

namespace OpenNexus.Foundation.DDD.Tests.Types;

/// <summary>
/// Tests for the DomainProperty class
/// </summary>
public class DomainPropertyTests
{
    /// <summary>
    /// Tests creating a DomainProperty with a valid initial value
    /// </summary>
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

    /// <summary>
    /// Tests creating a DomainProperty with an invalid initial value
    /// </summary>
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

    /// <summary>
    /// Tests creating a DomainProperty or throwing if invalid
    /// </summary>
    [Fact]
    public void CreateOrThrow_ReturnsInstance_WhenValueIsValid()
    {
        var validators = new IValidator<int>[] { new AlwaysValidValidator<int>() };

        var property = DomainProperty<int>.CreateOrThrow(42, validators);

        Assert.Equal(42, property.Value);
        int primitive = property;           // implicit operator
        Assert.Equal(42, primitive);
    }

    /// <summary>
    /// Tests that CreateOrThrow throws ArgumentException when the value is invalid
    /// </summary>
    [Fact]
    public void CreateOrThrow_ThrowsArgumentException_WhenValueIsInvalid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };

        var ex = Assert.Throws<ArgumentException>(
            () => DomainProperty<string>.CreateOrThrow("", validators));

        Assert.Contains("Value cannot be empty", ex.Message);
    }

    /// <summary>
    /// Tests setting a new valid value
    /// </summary>
    [Fact]
    public void SetValue_UpdatesValue_WhenNewValueIsValid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };
        var prop = DomainProperty<string>.CreateOrThrow("Initial", validators);

        var result = prop.SetValue("Updated");

        Assert.True(result.IsSuccess);
        Assert.Equal("Updated", prop.Value);
    }

    /// <summary>
    /// Tests that setting an invalid new value returns an error
    /// </summary>
    [Fact]
    public void SetValue_ReturnsError_WhenNewValueIsInvalid()
    {
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };
        var prop = DomainProperty<string>.CreateOrThrow("Valid", validators);

        var result = prop.SetValue("");

        Assert.False(result.IsSuccess);
        Assert.Equal("Valid", prop.Value); // value should not change
    }

    /// <summary>
    /// Tests TrySetValue returns true and null error message when setting a valid value
    /// </summary>
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

    /// <summary>
    /// Tests TrySetValue returns false and error message when setting an invalid value
    /// </summary>
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

    /// <summary>
    /// Tests that ToString returns the underlying value's string representation
    /// </summary>
    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        var validators = new IValidator<string>[] { new AlwaysValidValidator<string>() };
        var prop = DomainProperty<string>.CreateOrThrow("Kepler", validators);

        Assert.Equal("Kepler", prop.ToString());
    }

    /// <summary>
    /// Tests that the implicit operator returns the underlying value
    /// </summary>
    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        var validators = new IValidator<string>[] { new AlwaysValidValidator<string>() };
        var prop = DomainProperty<string>.CreateOrThrow("Galaxy", validators);

        string value = prop; // implicit conversion
        Assert.Equal("Galaxy", value);
    }
}