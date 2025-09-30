using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Tests;

public class ResultTests
{
    [Fact]
    public void Success_Result_Should_Be_Successful()
    {
        var result = Result.Success();

        Assert.True(result.IsSuccess);
        Assert.Equal("An unknown error occurred.", result.GetErrorMessage()); // Geen fout opgegeven
    }

    [Fact]
    public void Error_Result_Should_Not_Be_Successful()
    {
        var result = Result.Error("Something went wrong");

        Assert.False(result.IsSuccess);
        Assert.Equal("Something went wrong", result.GetErrorMessage());
    }
}