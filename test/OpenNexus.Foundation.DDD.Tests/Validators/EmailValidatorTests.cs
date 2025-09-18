using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Tests.Validators;

public class EmailValidatorTests
{
    /// <summary>
    /// Tests that AgainstInvalidInput throws ArgumentException for invalid email formats.
    /// </summary> 
    [Fact]
    public void AgainstIsEmail_InvalidEmail_ThrowsArgumentException()
    {
        // Arrange
        string email = "testhotmail.com";
        //act
        var ex = Assert.Throws<ArgumentException>(() =>
            Guard.AgainstInvalidInput(
                email,
                nameof(email),
                e => EmailValidator.IsValid(e)
            )
        );
        // Assert
        Assert.Contains(nameof(email), ex.Message);
        Assert.Contains("testhotmail.com", ex.Message);
    }
}
