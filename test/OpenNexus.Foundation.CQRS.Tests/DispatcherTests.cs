using Microsoft.Extensions.DependencyInjection;
using OpenNexus.Foundation.CQRS.Core;
using OpenNexus.Foundation.CQRS.Exceptions;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.CQRS.Tests;

/// <summary>
/// A pipeline behavior that records the order of execution in a trace list.
/// </summary>
/// <typeparam name="TCmd"></typeparam>
/// <typeparam name="TRes"></typeparam>
public sealed class RecordingBehaviorOuter<TCmd,TRes> : IPipelineBehavior<TCmd,TRes>
    where TCmd : notnull
    where TRes : notnull
{
    // Shared trace list to record the order of execution
    private readonly List<string> _trace;

    /// <summary>
    /// Creates a new instance of the outer recording behavior with a shared trace list.
    /// </summary>
    /// <param name="trace"></param>
    public RecordingBehaviorOuter(List<string> trace) => _trace = trace;

    public async Task<Result<TRes>> HandleAsync(TCmd request, PipelineDelegate<TRes> next, CancellationToken ct)
    {
        // This behavior runs before the inner behavior and the command handler
        _trace.Add("outer:before");

        // Call the next behavior in the pipeline, which could be another behavior or the command handler
        var r = await next(ct);

        // After the inner behavior runs, we can do some additional processing
        _trace.Add("outer:after");

        // Return the result to the caller
        return r;
    }
}

/// <summary>
/// A nested pipeline behavior that runs after the outer behavior and before the command handler.
/// </summary>
/// <typeparam name="TCmd"></typeparam>
/// <typeparam name="TRes"></typeparam>
public sealed class RecordingBehaviorInner<TCmd,TRes> : IPipelineBehavior<TCmd,TRes>
    where TCmd : notnull
    where TRes : notnull
{
    private readonly List<string> _trace;
    public RecordingBehaviorInner(List<string> trace) => _trace = trace;

    public async Task<Result<TRes>> HandleAsync(TCmd request, PipelineDelegate<TRes> next, CancellationToken ct)
    {
        // This behavior is nested inside the outer one, so it runs after the outer's "before" and before the outer's "after"
        _trace.Add("inner:before");

        // Call the next behavior in the pipeline
        var r = await next(ct);

        // After the inner behavior runs, we can do some additional processing
        _trace.Add("inner:after");

        // Return the result to the outer behavior
        return r;
    }
}

/// <summary>
/// A pipeline behavior that short-circuits the command processing by returning a predefined result.
/// </summary>
/// <typeparam name="TCmd"></typeparam>
/// <typeparam name="TRes"></typeparam>
public sealed class ShortCircuitBehaviorCommand<TCmd,TRes> : IPipelineBehavior<TCmd,TRes>
    where TCmd : ICommand<TRes>
    where TRes : notnull
{
    private readonly Result<TRes> _result;

    /// <summary>
    /// Creates a new instance of the short-circuit behavior with a predefined result.
    /// </summary>
    /// <param name="result"></param>
    public ShortCircuitBehaviorCommand(Result<TRes> result) => _result = result;

    public Task<Result<TRes>> HandleAsync(TCmd request, PipelineDelegate<TRes> next, CancellationToken ct)
    {
        // Short-circuit the pipeline by returning a predefined result
        return Task.FromResult(_result);
    }
}

/// <summary>
/// A pipeline behavior that short-circuits the command processing by returning a predefined result.
/// </summary>
/// <typeparam name="TCmd"></typeparam>
/// <typeparam name="TRes"></typeparam>
public sealed class ShortCircuitBehaviorQuery<TQuery,TRes> : IPipelineBehavior<TQuery,TRes>
    where TQuery : IQuery<TRes>
    where TRes : notnull
{
    private readonly Result<TRes> _result;

    /// <summary>
    /// Creates a new instance of the short-circuit behavior with a predefined result.
    /// </summary>
    /// <param name="result"></param>
    public ShortCircuitBehaviorQuery(Result<TRes> result) => _result = result;

    public Task<Result<TRes>> HandleAsync(TQuery request, PipelineDelegate<TRes> next, CancellationToken ct)
    {
        // Short-circuit the pipeline by returning a predefined result
        return Task.FromResult(_result);
    }
}

/// <summary>
/// A command to create a user with an email address.
/// </summary>
/// <param name="Email"></param>
public sealed record CreateUserCmd(string Email) : ICommand<Guid>;

/// <summary>
/// A command handler that creates a user and returns a fixed user ID.
/// </summary>
public sealed class CreateUserHandler : ICommandHandler<CreateUserCmd, Guid>
{
    public List<CancellationToken> SeenTokens { get; } = new();
    public TaskCompletionSource<bool> Called { get; } = new();

    Task<Result<Guid>> ICommandHandler<CreateUserCmd, Guid>.HandleAsync(CreateUserCmd command, CancellationToken ct)
    {
        // Simulate some work
        SeenTokens.Add(ct);             // Record the cancellation token
        Called.TrySetResult(true);      // Indicate handler was called

        // Return a fixed user ID for testing purposes
        return Task.FromResult(Result<Guid>.Success(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));
    }
}

/// <summary>
/// A command that represents a missing command, used for testing purposes.
/// </summary>
/// <param name="Name"></param>
public record MissingCommand(string Name) : ICommand<string>;

/// <summary>
/// A query that represents a query command, used for testing purposes.
/// </summary>
/// <param name="Name"></param>
public record MissingQuery(string Name) : IQuery<string>;

/// <summary>
/// A query to get a user by their ID and return their email address.
/// </summary>
/// <param name="UserId"></param>
public sealed record GetUserQuery(Guid UserId) : IQuery<string>;

/// <summary>
/// A query handler that retrieves a user's email address based on their ID.
/// </summary>
public sealed class GetUserHandler : IQueryHandler<GetUserQuery, string>
{
    public List<CancellationToken> SeenTokens { get; } = new();
    public TaskCompletionSource<bool> Called { get; } = new();

    public Task<Result<string>> HandleAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        // Simulate some work
        SeenTokens.Add(cancellationToken);             // Record the cancellation token
        Called.TrySetResult(true);                     // Indicate handler was called

        // Return a fixed user email for testing purposes
        return Task.FromResult(Result<string>.Success("john.doe@test.com"));
    }
}

public class DispatcherTests
{
    /// <summary>
    /// Tests that the Dispatcher correctly runs pipeline behaviors in order and calls the command handler.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchCommand_runs_behaviors_in_order_and_calls_handler()
    {
        var trace = new List<string>();
        using var sp = BuildCommandServices(trace);
        var dispatcher = sp.GetRequiredService<Dispatcher>();

        var ct = new CancellationTokenSource().Token;
        var result = await dispatcher.DispatchCommand(new CreateUserCmd("a@b.com"), ct);

        // Assert handler result
        Assert.True(result.IsSuccess);
        Assert.Equal(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), result.Value);

        // Assert order: outer before → inner before → handler → inner after → outer after
        // We can’t see "handler" directly from trace; assert count/shape around it:
        Assert.Equal(new[] { "outer:before", "inner:before", "inner:after", "outer:after" }, trace);

        // Optionally assert handler was actually invoked and saw the CT
        var handler = sp.GetRequiredService<ICommandHandler<CreateUserCmd, Guid>>() as CreateUserHandler;
        Assert.True(await handler!.Called.Task);
        Assert.Contains(ct, handler.SeenTokens);
    }

    /// <summary>
    /// Tests that the Dispatcher short-circuits the pipeline when a behavior returns an error, stopping further processing.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchCommand_short_circuit_stops_chain_and_handler()
    {
        var trace = new List<string>();
        using var sp = BuildCommandServices(trace, withShortCircuit: true);
        var dispatcher = sp.GetRequiredService<Dispatcher>();

        var result = await dispatcher.DispatchCommand(new CreateUserCmd("x@y.com"));

        Assert.False(result.IsSuccess);
        Assert.Equal("stop", result.GetErrorMessage());

        // Only outer:before runs, because short-circuit sits after it; no inner, no handler
        Assert.Equal(["outer:before", "outer:after" ], trace);

        var handler = sp.GetRequiredService<ICommandHandler<CreateUserCmd, Guid>>() as CreateUserHandler;
        // Handler wasn't called (TaskCompletionSource not set)
        Assert.False(handler!.Called.Task.IsCompleted);
    }

    /// <summary>
    /// Tests that the Dispatcher propagates exceptions thrown by the command handler, allowing them to bubble up.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchCommand_propagates_exception_when_handler_throws()
    {
        // Replace handler with one that throws
        var sc = new ServiceCollection();
        sc.AddScoped<ICommandHandler<CreateUserCmd, Guid>, ThrowingHandlerCreateUser>();
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorOuter<,>));
        sc.AddScoped(sp => new Dispatcher(sp));
        using var sp = sc.BuildServiceProvider();
        var dispatcher = sp.GetRequiredService<Dispatcher>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            dispatcher.DispatchCommand(new CreateUserCmd("boom")));
    }

    [Fact]
    public async Task DispatchQuery_runs_behaviors_in_order_and_calls_handler()
    {
        var trace = new List<string>();
        using var sp = BuildQueryServices(trace);
        var dispatcher = sp.GetRequiredService<Dispatcher>();

        var ct = new CancellationTokenSource().Token;
        var result = await dispatcher.DispatchQuery(new GetUserQuery(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")), ct);

        // Assert handler result
        Assert.True(result.IsSuccess);
        Assert.Equal("john.doe@test.com", result.Value);

        // Assert order: outer before → inner before → handler → inner after → outer after
        // We can’t see "handler" directly from trace; assert count/shape around it:
        Assert.Equal(new[] { "outer:before", "inner:before", "inner:after", "outer:after" }, trace);

        // Optionally assert handler was actually invoked and saw the CT
        var handler = sp.GetRequiredService<IQueryHandler<GetUserQuery, string>>() as GetUserHandler;
        Assert.True(await handler!.Called.Task);
        Assert.Contains(ct, handler.SeenTokens);
    }

    [Fact]
    public async Task DispatchQuery_short_circuit_stops_chain_and_handler()
    {
        var trace = new List<string>();
        using var sp = BuildQueryServices(trace, withShortCircuit: true);
        var dispatcher = sp.GetRequiredService<Dispatcher>();

        var result = await dispatcher.DispatchQuery(new GetUserQuery(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa")));

        Assert.False(result.IsSuccess);
        Assert.Equal("stop", result.GetErrorMessage());

        // Only outer:before runs, because short-circuit sits after it; no inner, no handler
        Assert.Equal(["outer:before", "outer:after" ], trace);

        var handler = sp.GetRequiredService<IQueryHandler<GetUserQuery, string>>() as GetUserHandler;
        // Handler wasn't called (TaskCompletionSource not set)
        Assert.False(handler!.Called.Task.IsCompleted);
    }

    [Fact]
    public async Task DispatchQuery_propagates_exception_when_handler_throws()
    {
        // Replace handler with one that throws
        var sc = new ServiceCollection();
        sc.AddScoped<IQueryHandler<GetUserQuery, string>, ThrowingHandlerGetUser>();
        sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorOuter<,>));
        sc.AddScoped(sp => new Dispatcher(sp));
        using var sp = sc.BuildServiceProvider();
        var dispatcher = sp.GetRequiredService<Dispatcher>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            dispatcher.DispatchQuery(new GetUserQuery(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"))));
    }

    /// <summary>
    /// Tests that the Dispatcher throws a HandlerNotRegisteredException when trying to dispatch a command
    /// without a registered handler.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchCommand_throws_HandlerNotRegisteredException_when_handler_missing()
    {
        // Set up a service collection without the CreateUserCmd handler
        var sc = new ServiceCollection();
        sc.AddScoped<IDispatcher, Dispatcher>();
        using var sp = sc.BuildServiceProvider();

        // Attempt to dispatch a command with no handler registered
        var dispatcher = sp.GetRequiredService<IDispatcher>();
        var cmd = new MissingCommand("test");

        // Expect a HandlerNotRegisteredException to be thrown
        var ex = await Assert.ThrowsAsync<HandlerNotRegisteredException>(() =>
            dispatcher.DispatchCommand(cmd));

        // Assert the exception details
        Assert.Equal(typeof(MissingCommand), ex.RequestType);
        Assert.Equal(typeof(ICommandHandler<MissingCommand, string>), ex.HandlerInterfaceType);
    }

    /// <summary>
    /// Tests that the Dispatcher throws a HandlerNotRegisteredException when trying to dispatch a query
    /// that has no registered handler.
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task DispatchQuery_throws_HandlerNotRegisteredException_when_handler_missing()
    {
        // Set up a service collection without the CreateUserCmd handler
        var sc = new ServiceCollection();
        sc.AddScoped<IDispatcher, Dispatcher>();
        using var sp = sc.BuildServiceProvider();

        // Attempt to dispatch a command with no handler registered
        var dispatcher = sp.GetRequiredService<IDispatcher>();
        var cmd = new MissingQuery("test");

        // Expect a HandlerNotRegisteredException to be thrown
        var ex = await Assert.ThrowsAsync<HandlerNotRegisteredException>(() =>
            dispatcher.DispatchQuery(cmd));

        // Assert the exception details
        Assert.Equal(typeof(MissingQuery), ex.RequestType);
        Assert.Equal(typeof(IQueryHandler<MissingQuery, string>), ex.HandlerInterfaceType);
    }

    /// <summary>
    /// A command handler that simulates a failure by throwing an exception.
    /// </summary>
    private sealed class ThrowingHandlerCreateUser : ICommandHandler<CreateUserCmd, Guid>
    {
        public Task<Result<Guid>> HandleAsync(CreateUserCmd command, CancellationToken ct)
            => throw new InvalidOperationException("boom");
    }

    /// <summary>
    /// A command handler that simulates a failure by throwing an exception.
    /// </summary>
    private sealed class ThrowingHandlerGetUser : IQueryHandler<GetUserQuery, string>
    {
        public Task<Result<string>> HandleAsync(GetUserQuery command, CancellationToken ct)
            => throw new InvalidOperationException("boom");
    }

    /// <summary>
    /// Builds a service provider with the necessary services for testing the Dispatcher with commands.
    /// </summary>
    /// <param name="trace"></param>
    /// <param name="withShortCircuit"></param>
    /// <returns></returns>
    private static ServiceProvider BuildCommandServices(List<string> trace, bool withShortCircuit = false)
    {
        var sc = new ServiceCollection();

        // Handlers
        sc.AddScoped<ICommandHandler<CreateUserCmd, Guid>, CreateUserHandler>();

        // Behaviors: registration order == execution order (outer → inner)
        if (withShortCircuit)
        {
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorOuter<,>));
            sc.AddScoped<IPipelineBehavior<CreateUserCmd, Guid>>(
                _ => new ShortCircuitBehaviorCommand<CreateUserCmd, Guid>(Result<Guid>.Error("stop")));
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorInner<,>)); // will not be hit
        }
        else
        {
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorOuter<,>));
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorInner<,>));
        }

        // Shared trace list
        sc.AddSingleton(trace);

        // The Dispatcher under test
        sc.AddScoped(sp => new Dispatcher(sp));

        return sc.BuildServiceProvider();
    }

    /// <summary>
    /// Builds a service provider with the necessary services for testing the Dispatcher with queries.
    /// </summary>
    /// <param name="trace"></param>
    /// <param name="withShortCircuit"></param>
    /// <returns></returns>
    private static ServiceProvider BuildQueryServices(List<string> trace, bool withShortCircuit = false)
    {
        var sc = new ServiceCollection();

        // Handlers
        sc.AddScoped<IQueryHandler<GetUserQuery, string>, GetUserHandler>();

        // Behaviors: registration order == execution order (outer → inner)
        if (withShortCircuit)
        {
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorOuter<,>));
            sc.AddScoped<IPipelineBehavior<GetUserQuery, string>>(
                _ => new ShortCircuitBehaviorQuery<GetUserQuery, string>(Result<string>.Error("stop")));
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorInner<,>)); // will not be hit
        }
        else
        {
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorOuter<,>));
            sc.AddScoped(typeof(IPipelineBehavior<,>), typeof(RecordingBehaviorInner<,>));
        }

        // Shared trace list
        sc.AddSingleton(trace);

        // The dispatcher
        sc.AddScoped(sp => new Dispatcher(sp));

        return sc.BuildServiceProvider();
    }
}
