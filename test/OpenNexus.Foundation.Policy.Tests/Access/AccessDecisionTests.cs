using OpenNexus.Foundation.Policy.Access;

namespace OpenNexus.Foundation.Policy.Tests.Access;

public class AccessDecisionTests
{
    [Fact]
    public void Allow_ShouldReturnAllowedDecision()
    {
        var result = AccessDecision.Allow();

        Assert.True(result.IsAccessGranted);
        Assert.Null(result.DenialReason);
    }

    [Fact]
    public void Deny_ShouldReturnDeniedDecision_WithReason()
    {
        var result = AccessDecision.Deny("Forbidden");

        Assert.False(result.IsAccessGranted);
        Assert.Equal("Forbidden", result.DenialReason);
    }

    [Fact]
    public void ToString_ShouldContainStatus()
    {
        var allow = AccessDecision.Allow();
        var deny = AccessDecision.Deny("Not allowed");

        Assert.Contains("Allowed", allow.ToString());
        Assert.Contains("Not allowed", deny.ToString());
    }
}

