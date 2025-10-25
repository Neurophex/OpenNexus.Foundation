namespace OpenNexus.Foundation.Policy.Tests.User;

public class UserContextTests
{
    [Fact]
    public void ShouldInitialize_AllRequiredFields()
    {
        var ctx = new UserContext
        {
            UserId = "123",
            Organization = "Neurophex",
            Roles = new HashSet<string> { "Admin" },
            Permissions = new HashSet<string> { "user.create" },
            IsSystem = true,
            TenantId = "tenant-1"
        };

        Assert.Equal("123", ctx.UserId);
        Assert.Equal("Neurophex", ctx.Organization);
        Assert.Contains("Admin", ctx.Roles);
        Assert.Contains("user.create", ctx.Permissions);
        Assert.True(ctx.IsSystem);
        Assert.Equal("tenant-1", ctx.TenantId);
    }

    [Fact]
    public void ShouldUseDefaults_WhenNotSpecified()
    {
        var ctx = new UserContext
        {
            UserId = "U1",
            Organization = "Org"
        };

        Assert.Empty(ctx.Roles);
        Assert.Empty(ctx.Permissions);
        Assert.False(ctx.IsSystem);
        Assert.Null(ctx.TenantId);
    }
}

