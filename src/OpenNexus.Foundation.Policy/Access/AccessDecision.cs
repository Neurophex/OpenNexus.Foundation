using OpenNexus.Foundation.Policy.Abstractions;

namespace OpenNexus.Foundation.Policy.Access;

/// <summary>
/// Default implementation of <see cref="IAccessDecision"/>.
/// </summary>
public sealed class AccessDecision : IAccessDecision
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AccessDecision"/> class.
    /// </summary>
    /// <param name="isAccessGranted"></param>
    /// <param name="denialReason"></param>
    public AccessDecision(bool isAccessGranted, string? denialReason = null)
    {
        IsAccessGranted = isAccessGranted;
        DenialReason = denialReason;
    }

    /// <summary>
    /// Determines if access is granted.
    /// </summary>
    public bool IsAccessGranted { get; }

    /// <summary>
    /// Provides the reason for denial if access is not granted.
    /// </summary>
    public string? DenialReason { get; }

    /// <summary>
    /// Creates an <see cref="AccessDecision"/> that allows access.
    /// </summary>
    /// <returns></returns>
    public static AccessDecision Allow() => new(true);

    /// <summary>
    /// Creates an <see cref="AccessDecision"/> that denies access with a specified reason.
    /// </summary>
    /// <param name="reason"></param>
    /// <returns></returns>
    public static AccessDecision Deny(string reason) => new(false, reason);
}