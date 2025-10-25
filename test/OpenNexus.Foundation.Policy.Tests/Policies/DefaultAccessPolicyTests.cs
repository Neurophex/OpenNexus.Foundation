using OpenNexus.Foundation.Policy.Abstractions;
using OpenNexus.Foundation.Policy.Access;
using Moq;

namespace OpenNexus.Foundation.Policy.Tests.Policies;

public class DummyResource { }

public class DefaultAccessPolicyTests
{
    private readonly IUserContext _user = Mock.Of<IUserContext>(u => u.UserId == "U1");

    private class TestPolicy : IAccessPolicy<DummyResource>
    {
        public IAccessDecision CanViewResource(IUserContext user, DummyResource resource) => AccessDecision.Allow();
    }

    [Fact]
    public void DefaultMethods_ShouldReturnDeny()
    {
        IAccessPolicy<DummyResource> policy = new TestPolicy();
        var resource = new DummyResource();

        Assert.False(policy.CanCreateResource(_user).IsAccessGranted);
        Assert.False(policy.CanCreateResource(_user, resource).IsAccessGranted);
        Assert.False(policy.CanEditResource(_user, resource).IsAccessGranted);
        Assert.False(policy.CanDeleteResource(_user, resource).IsAccessGranted);
    }

    [Fact]
    public void CanViewResource_ShouldBeAllowed_ForDefaultImplementation()
    {
        var policy = new TestPolicy();
        var result = policy.CanViewResource(_user, new DummyResource());

        Assert.True(result.IsAccessGranted);
    }

    [Fact]
    public void CanCreateSpecifiedResource_ShouldFallbackToCanCreateResource()
    {
        var mock = new Mock<IAccessPolicy<DummyResource>>();
        mock.Setup(p => p.CanCreateResource(It.IsAny<IUserContext>()))
            .Returns(AccessDecision.Deny("Denied by default"));

        mock.Setup(p => p.CanCreateResource(It.IsAny<IUserContext>(), It.IsAny<DummyResource>()))
            .Returns<IUserContext, DummyResource>((u, r) => mock.Object.CanCreateResource(u));

        var policy = mock.Object;
        var result = policy.CanCreateResource(_user, new DummyResource());

        Assert.False(result.IsAccessGranted);
    }
}

