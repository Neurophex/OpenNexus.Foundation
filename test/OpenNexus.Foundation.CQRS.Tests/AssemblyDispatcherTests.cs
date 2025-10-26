using Microsoft.Extensions.DependencyInjection;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS.Tests;

public class AssemblyDispatcherTests
{
    private readonly ServiceProvider _provider;
    private readonly HandlerRegistry _registry;
    private readonly AssemblyDispatcher _dispatcher;

    public AssemblyDispatcherTests()
    {
        // Setup DI container
        var services = new ServiceCollection();
        services.AddSingleton<IHandlerRegistry, HandlerRegistry>();
        services.AddTransient<DelayedTestCommandHandler>();
        services.AddTransient<DelayedTestQueryHandler>();
        services.AddTransient<IDispatcher, AssemblyDispatcher>();

        _provider = services.BuildServiceProvider();
        _registry = (HandlerRegistry)_provider.GetRequiredService<IHandlerRegistry>();

        // Register handlers manually
        _registry.RegisterHandlersFromAssembly(typeof(DelayedTestCommandHandler).Assembly);

        _dispatcher = new AssemblyDispatcher(_provider);
    }

    [Fact]
    public async Task DispatchCommand_Should_Invoke_Handler_And_Return_Success()
    {
        // Arrange
        var command = new DelayedTestCommand();

        // Act
        var result = await _dispatcher.Command(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("CommandHandled", result.Value);
    }

    [Fact]
    public async Task DispatchQuery_Should_Invoke_Handler_And_Return_Success()
    {
        // Arrange
        var query = new DelayedTestQuery();

        // Act
        var result = await _dispatcher.Query(query);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }

    [Fact]
    public async Task DispatchCommand_Should_Return_Failed_When_Handler_Not_Registered()
    {
        // Arrange
        var unregisteredCommand = new FakeCommand();

        // Act & Assert
        var result = await _dispatcher.Command(unregisteredCommand);

        Assert.False(result.IsSuccess);
        Assert.Contains("No command handler registered", result.GetErrorMessage());
    }

    [Fact]
    public async Task DispatchQuery_Should_Throw_When_Handler_Not_Registered()
    {
        // Arrange
        var unregisteredQuery = new FakeQuery();

        // Act & Assert
        var result = await _dispatcher.Query(unregisteredQuery);

        Assert.False(result.IsSuccess);
        Assert.Contains("No query handler registered", result.GetErrorMessage());
    }

    [Fact]
    public async Task DispatchCommand_Should_Log_And_Return_Failure_On_Exception()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddSingleton<IHandlerRegistry, HandlerRegistry>();
        services.AddSingleton<IDispatcher, AssemblyDispatcher>();
        services.AddTransient<ThrowingCommandHandler>();
        var provider = services.BuildServiceProvider();

        var registry = (HandlerRegistry)provider.GetRequiredService<IHandlerRegistry>();
        registry.RegisterHandlersFromAssembly(typeof(ThrowingCommandHandler).Assembly);

        var dispatcher = new AssemblyDispatcher(provider);

        // Act
        var result = await dispatcher.Command(new ThrowingCommand());

        // Assert
        Assert.False(result.IsSuccess);
        Assert.NotNull(result.GetErrorMessage());
    }

    private class FakeCommand : ICommand<string> { }
    private class FakeQuery : IQuery<int> { }

    private class ThrowingCommand : ICommand<string> { }

    private class ThrowingCommandHandler : ICommandHandler<ThrowingCommand, string>
    {
        public Task<Result<string>> HandleAsync(ThrowingCommand command, CancellationToken token)
        {
            throw new InvalidOperationException("Simulated handler failure");
        }
    }
}
