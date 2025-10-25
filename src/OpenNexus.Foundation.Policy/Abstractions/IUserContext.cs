namespace OpenNexus.Foundation.Policy.Abstractions;

/// <summary>
/// Represents the context of the current user.
/// </summary>
public interface IUserContext
{
    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    string UserId { get; }

    /// <summary>
    /// Gets the organization associated with the current user.
    /// </summary>
    string Organization { get; }

    /// <summary>
    /// Gets the roles assigned to the current user.
    /// </summary>
    IReadOnlySet<string> Roles { get; }

    /// <summary>
    /// Gets the permissions granted to the current user.
    /// </summary>
    IReadOnlySet<string> Permissions { get; }

    /// <summary>
    /// Indicates whether the current user is a system user.
    /// </summary>
    bool IsSystem { get; }

    /// <summary>
    /// Gets the tenant identifier associated with the current user, if applicable.
    /// </summary>
    string? TenantId { get; }
}
