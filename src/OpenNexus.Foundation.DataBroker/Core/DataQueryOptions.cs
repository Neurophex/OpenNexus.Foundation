using OpenNexus.Foundation.DataBroker.Abstractions;

namespace OpenNexus.Foundation.DataBroker;

/// <summary>
/// Defines options for querying data.
/// </summary>
/// <param name="Skip">The amount of items to skip</param>
/// <param name="Take">The amount of items to take from the result set</param>
/// <param name="OrderBy">The data field to order by</param>
/// <param name="Ascending">True if ordering in ascending order</param>
/// <param name="Filter">Filters that are applied to the query</param>
public record DataQueryOptions(
    int Skip = 0,
    int Take = 50,
    string? OrderBy = null,
    bool Ascending = true,
    string? Filter = null) : IDataQueryOptions
{
    /// <summary>
    /// Number of items to skip in the result set.
    /// </summary>
    /// <remarks>
    /// A negative value will be treated as zero.
    /// </remarks>
    public int Skip { get; init; } = Math.Max(0, Skip);

    /// <summary>
    /// Number of items to take in the result set.
    /// </summary>
    /// <remarks>
    /// A negative value will be treated as zero.
    /// </remarks>
    public int Take { get; init; } = Take < 0 ? 0 : Take;
}