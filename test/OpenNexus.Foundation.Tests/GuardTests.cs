using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Tests;

public class GuardTests
{
    /// <summary>
    /// Tests that AgainstInvalidEmail throws ArgumentException for invalid email formats.
    /// </summary>
    [Fact]
    public void AgainstNull_NullValue_ThrowsArgumentNullException()
    {
        // Arrange
        // Act & Assert
        object? obj = null;
        Assert.Throws<ArgumentNullException>(() => Guard.AgainstNull(obj, nameof(obj)));
    }


    // AgainstNullOrEmpty
    /// <summary>
    /// Tests that AgainstNullOrEmpty throws ArgumentException for null or empty strings.
    /// </summary>
    /// <param name="value"></param>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void AgainstNullOrEmpty_InvalidString_ThrowsArgumentException(string? value)
    {
        Assert.Throws<ArgumentException>(() => Guard.AgainstNullOrEmpty(value, "name"));
    }


    // AgainstOutOfRange
    /// <summary>
    /// Tests that AgainstOutOfRange throws ArgumentOutOfRangeException for values outside the specified
    /// range.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="min"></param>
    /// <param name="max"></param> 
    [Theory]
    [InlineData(0, 1, 10)]   // below min
    [InlineData(11, 1, 10)]  // above max
    public void AgainstOutOfRange_InvalidValue_ThrowsArgumentOutOfRangeException(int value, int min, int max)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Guard.AgainstOutOfRange(value, min, max, nameof(value)));
    }

    /// <summary>
    /// Tests that AgainstInvalidEmail throws ArgumentException for invalid email formats.
    /// </summary>
    // [Fact]
    // public void AgainstInvalidEmail_InvalidEmail_ThrowsArgumentException()
    // {
    //     string invalidEmail = "invalidMail";
    //     Assert.Throws<ArgumentException>(() =>
    //         Guard.AgainstInvalidEmail(invalidEmail, nameof(invalidEmail)));
    // }
}
