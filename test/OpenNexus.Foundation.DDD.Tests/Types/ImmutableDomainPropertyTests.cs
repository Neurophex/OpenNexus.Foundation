using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.DDD.Validators;

namespace OpenNexus.Foundation.DDD.Tests.Types;

public class ImmutableDomainPropertyTests
{
    [Fact]
    public void Create_ReturnsSuccess_WhenValueIsValid()
    {
        // Arrange
        var validators = new IValidator<string>[] {
                new NotEmptyStringValidator(),
                new MaxLengthValidator(10)
            };

        // Act
        var result = ImmutableDomainProperty<string>.Create("Hello", validators);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Hello", result.Value.Value);
    }

    [Fact]
    public void Create_ReturnsError_WhenValueIsInvalid()
    {
        // Arrange
        var validators = new IValidator<string>[] {
                new NotEmptyStringValidator(),
                new MaxLengthValidator(5)
            };

        // Act
        var result = ImmutableDomainProperty<string>.Create("This is too long", validators);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("exceeds", result.GetErrorMessage());
    }

    [Fact]
    public void CreateOrThrow_ReturnsInstance_WhenValueIsValid()
    {
        // Arrange
        var validators = new IValidator<int>[] { new AlwaysValidValidator<int>() };

        // Act
        var property = ImmutableDomainProperty<int>.CreateOrThrow(42, validators);

        // Assert
        Assert.Equal(42, property.Value);
        // Implicit operator check
        int primitive = property;
        Assert.Equal(42, primitive);
    }

    [Fact]
    public void CreateOrThrow_ThrowsArgumentException_WhenValueIsInvalid()
    {
        // Arrange
        var validators = new IValidator<string>[] { new NotEmptyStringValidator() };

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(
            () => ImmutableDomainProperty<string>.CreateOrThrow("", validators));

        Assert.Contains("Value cannot be empty", ex.Message);
    }

    [Fact]
    public void ToString_ReturnsUnderlyingValue()
    {
        // Arrange
        var validators = new IValidator<string>[] { new AlwaysValidValidator<string>() };
        var prop = ImmutableDomainProperty<string>.CreateOrThrow("Kepler", validators);

        // Act
        var text = prop.ToString();

        // Assert
        Assert.Equal("Kepler", text);
    }

    [Fact]
    public void ImplicitOperator_ReturnsUnderlyingValue()
    {
        // Arrange
        var validators = new IValidator<string>[] { new AlwaysValidValidator<string>() };
        var prop = ImmutableDomainProperty<string>.CreateOrThrow("Galaxy", validators);

        // Act
        string value = prop; // implicit conversion

        // Assert
        Assert.Equal("Galaxy", value);
    }
}