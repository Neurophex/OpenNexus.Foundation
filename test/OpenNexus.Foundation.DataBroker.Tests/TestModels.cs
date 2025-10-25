// Test models
using OpenNexus.Foundation.DataBroker.Abstractions;

public class TestProjection : IDataProjection
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public object GetIdentity() => Id;
}

public class TestSource
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}

// Simple query options
public class TestQueryOptions : IDataQueryOptions
{
    public int Skip { get; init; }
    public int Take { get; init; }
    public string? OrderBy { get; init; }
    public bool Ascending { get; init; } = true;
    public string? Filter { get; init; }
}