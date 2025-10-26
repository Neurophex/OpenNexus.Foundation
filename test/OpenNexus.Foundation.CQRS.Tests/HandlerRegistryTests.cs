using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using OpenNexus.Foundation.CQRS;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.CQRS.Tests;

public class HandlerRegistryTests
{
    [Fact]
    public void RegisterCommandHandler_Should_Store_Handler()
    {
        // Arrange
        var registry = new HandlerRegistry();

        // Act
        registry.RegisterCommandHandler(typeof(TestCommand), typeof(TestCommandHandler));

        // Assert
        var provider = new ServiceCollection()
            .AddSingleton<TestDependency>()
            .BuildServiceProvider();

        var handler = registry.CreateCommandHandler(typeof(TestCommand), provider);

        Assert.NotNull(handler);
        Assert.IsType<TestCommandHandler>(handler);
    }

    [Fact]
    public void RegisterQueryHandler_Should_Store_Handler()
    {
        // Arrange
        var registry = new HandlerRegistry();

        // Act
        registry.RegisterQueryHandler(typeof(TestQuery), typeof(TestQueryHandler));

        // Assert
        var provider = new ServiceCollection().BuildServiceProvider();
        var handler = registry.CreateQueryHandler(typeof(TestQuery), provider);

        Assert.NotNull(handler);
        Assert.IsType<TestQueryHandler>(handler);
    }

    [Fact]
    public void CreateCommandHandler_Should_Throw_When_NotRegistered()
    {
        var registry = new HandlerRegistry();
        var provider = new ServiceCollection().BuildServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(
            () => registry.CreateCommandHandler(typeof(TestCommand), provider));

        Assert.Contains("No command handler registered", ex.Message);
    }

    [Fact]
    public void CreateQueryHandler_Should_Throw_When_NotRegistered()
    {
        var registry = new HandlerRegistry();
        var provider = new ServiceCollection().BuildServiceProvider();

        var ex = Assert.Throws<InvalidOperationException>(
            () => registry.CreateQueryHandler(typeof(TestQuery), provider));

        Assert.Contains("No query handler registered", ex.Message);
    }

    [Fact]
    public void RegisterHandlersFromAssembly_Should_Discover_Handlers()
    {
        // Arrange
        var registry = new HandlerRegistry();
        var logger = NullLogger.Instance;

        // Act
        registry.RegisterHandlersFromAssembly(typeof(TestCommandHandler).Assembly, logger);

        // Assert
        var provider = new ServiceCollection()
            .AddSingleton<TestDependency>()
            .BuildServiceProvider();

        var commandHandler = registry.CreateCommandHandler(typeof(TestCommand), provider);
        var queryHandler = registry.CreateQueryHandler(typeof(TestQuery), provider);

        Assert.NotNull(commandHandler);
        Assert.NotNull(queryHandler);
        Assert.IsType<TestCommandHandler>(commandHandler);
        Assert.IsType<TestQueryHandler>(queryHandler);
    }

    [Fact]
    public async Task CreateCommandHandler_Should_Resolve_Dependencies()
    {
        // Arrange
        var registry = new HandlerRegistry();
        registry.RegisterCommandHandler(typeof(TestCommand), typeof(TestCommandHandler));

        var provider = new ServiceCollection()
            .AddSingleton<TestDependency>()
            .BuildServiceProvider();

        // Act
        var handler = (TestCommandHandler)registry.CreateCommandHandler(typeof(TestCommand), provider);
        var result = await handler.HandleAsync(new TestCommand(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("InjectedDependency", result.Value);
        Assert.True(handler.Handled);
    }

    [Fact]
    public async Task CreateQueryHandler_Should_Invoke_Handle_Successfully()
    {
        // Arrange
        var registry = new HandlerRegistry();
        registry.RegisterQueryHandler(typeof(TestQuery), typeof(TestQueryHandler));
        var provider = new ServiceCollection().BuildServiceProvider();

        // Act
        var handler = (TestQueryHandler)registry.CreateQueryHandler(typeof(TestQuery), provider);
        var result = await handler.HandleAsync(new TestQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(42, result.Value);
    }
}
