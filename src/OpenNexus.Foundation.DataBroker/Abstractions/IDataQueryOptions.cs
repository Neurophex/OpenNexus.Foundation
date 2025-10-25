namespace OpenNexus.Foundation.DataBroker.Abstractions;

/// <summary>
/// Defines options for querying data.
/// </summary>
public interface IDataQueryOptions
{
    /// <summary>
    /// Number of items to skip in the result set.
    /// </summary>
    public int Skip { get; }

    /// <summary>
    /// Number of items to take in the result set.
    /// </summary>
    public int Take { get; }

    /// <summary>
    /// Field to order the results by.
    /// </summary>
    public string? OrderBy { get; }

    /// <summary>
    /// Indicates whether the ordering is ascending.
    /// </summary>
    public bool Ascending { get; }

    /// <summary>
    /// Filter expression for querying the data.
    /// </summary>
    string? Filter { get; }
}
