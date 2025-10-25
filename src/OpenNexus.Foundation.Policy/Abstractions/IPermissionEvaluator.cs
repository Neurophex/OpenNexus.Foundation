namespace OpenNexus.Foundation.Policy.Abstractions;

/// <summary>
/// Evaluates user permissions.
/// </summary>
public interface IPermissionEvaluator
{
    /// <summary>
    /// Evaluates if the user has the specified permission.
    /// </summary>
    /// <param name="user">The user context.</param>
    /// <param name="permission">The permission to evaluate.</param>
    /// <returns>True if the user has the permission; otherwise, false.</returns>
    bool HasPermission(IUserContext user, string permission);

    /// <summary>
    /// Asynchronously evaluates if the user has the specified permission.
    /// </summary>
    /// <param name="user">The user context.</param>
    /// <param name="permission">The permission to evaluate.</param>
    /// <returns>True if the user has the permission; otherwise, false.</returns>
    Task<bool> HasPermissionAsync(IUserContext user, string permission);
}