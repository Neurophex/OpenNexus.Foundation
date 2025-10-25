namespace OpenNexus.Foundation.Policy.Abstractions;

/// <summary>
/// Represents the result of an access decision.
/// </summary>
public interface IAccessDecision
{
    /// <summary>
    /// Determines if access is granted.
    /// </summary>
    bool IsAccessGranted { get; }

    /// <summary>
    /// Provides the reason for denial if access is not granted.
    /// </summary>
    string? DenialReason { get; }
}
