using OpenNexus.Foundation.DataBroker.Abstractions;
using OpenNexus.Foundation.Policy.Abstractions;
using OpenNexus.Foundation.Primitives;

public class TestDataBroker : IDataBroker<TestProjection, int>
{
    private readonly List<TestSource> _data;
    private readonly IAccessPolicy<TestSource> _policy;

    public TestDataBroker(List<TestSource> data, IAccessPolicy<TestSource> policy)
    {
        _data = data;
        _policy = policy;
    }

    public Task<Result<TestProjection>> GetByIdAsync(int id, IUserContext user, CancellationToken ct = default)
    {
        var source = _data.FirstOrDefault(x => x.Id == id);
        if (source == null)
            return Task.FromResult(Result<TestProjection>.Error("Not found"));

        var decision = _policy.CanViewResource(user, source);
        if (!decision.IsAccessGranted)
            return Task.FromResult(Result<TestProjection>.Error(decision.DenialReason ?? "Access denied"));

        var projection = new TestProjection { Id = source.Id, Name = source.Name };
        return Task.FromResult(Result<TestProjection>.Success(projection));
    }

    public Task<Result<IEnumerable<TestProjection>>> GetAllAsync(IUserContext user, IDataQueryOptions? options, CancellationToken ct = default)
    {
        var results = new List<TestProjection>();

        foreach (var src in _data)
        {
            if (_policy.CanViewResource(user, src).IsAccessGranted)
                results.Add(new TestProjection { Id = src.Id, Name = src.Name });
        }

        if (options is not null)
            results = results.Skip(Math.Max(0, options.Skip)).Take(Math.Max(0, options.Take)).ToList();

        return Task.FromResult(Result<IEnumerable<TestProjection>>.Success(results));
    }
}