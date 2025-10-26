using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS.Tests;

public class TestCommand : ICommand<string> { }
public class TestQuery : IQuery<int> { }

public class TestDependency
{
    public string Name { get; } = "InjectedDependency";
}

public class TestCommandHandler : ICommandHandler<TestCommand, string>
{
    private readonly TestDependency _dependency;
    public bool Handled { get; private set; }

    public TestCommandHandler(TestDependency dependency)
    {
        _dependency = dependency;
    }

    public Task<Result<string>> HandleAsync(TestCommand command, CancellationToken cancellationToken)
    {
        Handled = true;
        return Task.FromResult(Result<string>.Success(_dependency.Name));
    }

    Task<Result<string>> ICommandHandler<TestCommand, string>.HandleAsync(TestCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class TestQueryHandler : IQueryHandler<TestQuery, int>
{
    public Task<Result<int>> HandleAsync(TestQuery query, CancellationToken cancellationToken)
        => Task.FromResult(Result<int>.Success(42));
}

public class DelayedTestCommand : ICommand<string> { }
public class DelayedTestQuery : IQuery<int> { }

public class DelayedTestCommandHandler : ICommandHandler<DelayedTestCommand, string>
{
    public bool WasHandled { get; private set; }

    public async Task<Result<string>> HandleAsync(DelayedTestCommand command, CancellationToken token)
    {
        WasHandled = true;
        await Task.Delay(10, token);
        return Result<string>.Success("CommandHandled");
    }
}

public class DelayedTestQueryHandler : IQueryHandler<DelayedTestQuery, int>
{
    public bool WasHandled { get; private set; }

    public async Task<Result<int>> HandleAsync(DelayedTestQuery query, CancellationToken token)
    {
        WasHandled = true;
        await Task.Delay(10, token);
        return Result<int>.Success(42);
    }
}