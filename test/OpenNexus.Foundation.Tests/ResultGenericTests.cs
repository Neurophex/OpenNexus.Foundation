using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.Tests;

public class ResultGenericTests
{
    [Fact]
    public void Success_ResultT_Should_Return_Value()
    {
        var result = Result<string>.Success("Hello");

        Assert.True(result.IsSuccess);
        Assert.Equal("Hello", result.Value);
    }

    [Fact]
    public void Error_ResultT_Should_Throw_On_Value_Access()
    {
        var result = Result<string>.Error("Error occurred");

        Assert.False(result.IsSuccess);
        var ex = Assert.Throws<InvalidOperationException>(() => _ = result.Value);
        Assert.Equal("Cannot access Value when Result is not successful.", ex.Message);
    }

    [Fact]
    public void Error_ResultT_Should_Return_Error_Message()
    {
        var result = Result<string>.Error("Failure");

        Assert.Equal("Failure", result.GetErrorMessage());
    }

    [Fact]
    public void Success_ResultT_Should_Return_Empty_Error_Message()
    {
        var result = Result<string>.Success("OK");

        Assert.Equal(string.Empty, result.GetErrorMessage());
    }

    [Fact]
    public void Implicit_Conversion_To_ResultT_Should_Work()
    {
        Result<string> result = "Implicit";

        Assert.True(result.IsSuccess);
        Assert.Equal("Implicit", result.Value);
    }
}