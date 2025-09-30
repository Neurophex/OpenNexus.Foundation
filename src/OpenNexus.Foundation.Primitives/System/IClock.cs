namespace OpenNexus.Foundation.Primitives;

/// <summary>
/// Interface for a clock service that provides the current date and time.
/// </summary>
public interface IClock
{
    /// <summary>
    /// Gets the current date and time.
    /// </summary>
    /// <returns>The current date and time.</returns>
    public DateTime Now { get; }

    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    /// <returns>The current UTC date and time.</returns>
    public DateTime UtcNow { get; }
}