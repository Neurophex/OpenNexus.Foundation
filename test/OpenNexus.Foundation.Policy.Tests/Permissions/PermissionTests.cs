using OpenNexus.Foundation.Policy.Permissions;

namespace OpenNexus.Foundation.Policy.Tests.Permissions;

public class PermissionTests
{
    [Fact]
    public void Constructor_ShouldNormalizeKey_ToLowerCaseAndTrim()
    {
        var permission = new Permission("  ADMIN.CREATE  ", "Can create admins");

        Assert.Equal("admin.create", permission.Key);
        Assert.Equal("Can create admins", permission.Description);
    }

    [Fact]
    public void Constructor_ShouldThrow_WhenKeyIsNullOrWhitespace()
    {
        Assert.Throws<ArgumentException>(() => new Permission("  ", "Invalid"));
    }

    [Fact]
    public void Equals_ShouldBeCaseInsensitive()
    {
        var a = new Permission("USER.VIEW", null);
        var b = new Permission("user.view", null);

        Assert.True(a.Equals(b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnKey()
    {
        var p = new Permission("test.permission", "desc");
        Assert.Equal("test.permission", p.ToString());
    }
}
