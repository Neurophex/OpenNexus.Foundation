namespace OpenNexus.Foundation.Policy.Abstractions;

/// <summary>
/// Defines policies related to tenant access.
/// </summary>
public interface ITenantPolicy
{
    /// <summary>
    /// Determines if the user belongs to the same tenant as the specified tenant ID.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="tenantId"></param>
    /// <returns></returns>
    bool IsSameTenant(IUserContext user, string tenantId);
}