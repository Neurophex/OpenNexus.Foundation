namespace OpenNexus.Foundation.Policy.Abstractions;

/// <summary>
/// Provides access to the current <see cref="IUserContext"/>.
/// </summary>
public interface IUserContextAccessor
{
    /// <summary>
    /// Gets the current user context.
    /// </summary>
    IUserContext UserContext { get; }
}
