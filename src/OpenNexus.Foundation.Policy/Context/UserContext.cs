using OpenNexus.Foundation.Policy.Abstractions;

namespace OpenNexus.Foundation.Policy;

/// <summary>
/// Default implementation of <see cref="IUserContext"/>.
/// </summary>
public sealed class UserContext : IUserContext
{
    public required string UserId { get; init; }
    public required string Organization { get; init; }
    public IReadOnlySet<string> Roles { get; init; } = new HashSet<string>();
    public IReadOnlySet<string> Permissions { get; init; } = new HashSet<string>();
    public bool IsSystem { get; init; } = false;
    public string? TenantId { get; init; }
}
