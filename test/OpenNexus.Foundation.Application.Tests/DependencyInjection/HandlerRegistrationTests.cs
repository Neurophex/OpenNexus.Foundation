using Microsoft.Extensions.DependencyInjection;
using OpenNexus.Foundation.Application.DependencyInjection;
using OpenNexus.Foundation.CQRS.Core;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Application.Tests.DependencyInjection;

// This section contains the test data used in the tests below.
#region TestData

// Commands & Queries
public sealed record CmdA(int N) : ICommand<int>;
public sealed record CmdB(string S) : ICommand<string>;
public sealed record QryA(Guid Id) : IQuery<string>;

// Handlers
public sealed class HandlerA : ICommandHandler<CmdA, int>
{
    /// <summary>
    /// Handles the command and returns a result.
    /// </summary>
    /// <remarks>
    /// This method increments the value of N by 1 and returns it as a result.
    /// </remarks>
    /// <param name="c"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    public Task<Result<int>> HandleAsync(CmdA c, CancellationToken ct)
        => Task.FromResult(Result<int>.Success(c.N + 1));
}

// One class implements multiple command interfaces
public sealed class MultiCmdHandler :
    ICommandHandler<CmdA, int>,
    ICommandHandler<CmdB, string>
{
    public Task<Result<int>> HandleAsync(CmdA c, CancellationToken ct)
        => Task.FromResult(Result<int>.Success(c.N + 10));

    public Task<Result<string>> HandleAsync(CmdB c, CancellationToken ct)
        => Task.FromResult(Result<string>.Success(c.S.ToUpperInvariant()));
}

// Query handler
public sealed class QryAHandler : IQueryHandler<QryA, string>
{
    public Task<Result<string>> HandleAsync(QryA q, CancellationToken ct)
        => Task.FromResult(Result<string>.Success(q.Id.ToString("N")));
}

// Types that must be ignored
public abstract class AbstractHandler : ICommandHandler<CmdA, int>
{
    public abstract Task<Result<int>> HandleAsync(CmdA c, CancellationToken ct);
}
public class OpenGenericHandler<T> : ICommandHandler<CmdA, int>
{
    public Task<Result<int>> HandleAsync(CmdA c, CancellationToken ct)
        => Task.FromResult(Result<int>.Success(c.N));
}

#endregion


/// <summary>
/// Tests for the registration of command and query handlers in the service collection.
/// </summary>
public class HandlerRegistrationTests
{
    /// <summary>
    /// Tests that command handlers can be registered and resolved from the service collection.
    /// </summary>
    /// <remarks>
    /// This test verifies that the command handlers are correctly registered in the service collection
    /// and can be resolved as expected.
    /// </remarks>
    [Fact]
    public void ShouldRegisterCommandHandlers()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddCommandHandlers([typeof(HandlerA), typeof(MultiCmdHandler)]);

        // Act
        var serviceProvider = services.BuildServiceProvider();
        var commandHandler = serviceProvider.GetService<ICommandHandler<CmdA, int>>();

        // Assert
        Assert.NotNull(commandHandler);
    }

    /// <summary>
    /// Tests that a single command handler is registered as scoped and can be resolved.
    /// </summary>
    /// <remarks>
    /// This test verifies that a single command handler is registered with the correct lifetime
    /// and can be resolved from a scoped service provider.
    /// </remarks>
    [Fact]
    public async void AddCommandHandlers_registers_single_handler_as_scoped_and_resolves()
    {
        var services = new ServiceCollection();
        var types = new[] { typeof(HandlerA) };

        services.AddCommandHandlers(types);

        var desc = services.Single(d =>
            d.ServiceType == typeof(ICommandHandler<CmdA, int>) &&
            d.ImplementationType == typeof(HandlerA));

        Assert.Equal(ServiceLifetime.Scoped, desc.Lifetime);

        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();

        var handler = scope.ServiceProvider.GetRequiredService<ICommandHandler<CmdA, int>>();
        var res = await handler.HandleAsync(new CmdA(41), default);

        Assert.True(res.IsSuccess);
        Assert.Equal(42, res.Value);
    }

    /// <summary>
    /// Tests that multiple command handlers can be registered for different command interfaces
    /// and can be resolved correctly.
    /// </summary>
    /// <remarks>
    /// This test verifies that a single class can implement multiple command interfaces
    /// and that each interface can be resolved independently.
    /// </remarks>
    [Fact]
    public async void AddCommandHandlers_registers_all_interfaces_for_multi_handler()
    {
        var services = new ServiceCollection();
        var types = new[] { typeof(MultiCmdHandler) };

        services.AddCommandHandlers(types);

        Assert.Contains(services, d =>
            d.ServiceType == typeof(ICommandHandler<CmdA, int>) &&
            d.ImplementationType == typeof(MultiCmdHandler));

        Assert.Contains(services, d =>
            d.ServiceType == typeof(ICommandHandler<CmdB, string>) &&
            d.ImplementationType == typeof(MultiCmdHandler));

        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();

        var ha = scope.ServiceProvider.GetRequiredService<ICommandHandler<CmdA, int>>();
        var hb = scope.ServiceProvider.GetRequiredService<ICommandHandler<CmdB, string>>();

        var resultA = await ha.HandleAsync(new CmdA(41), default);
        var resultB = await hb.HandleAsync(new CmdB("hello"), default);

        Assert.Equal(51, resultA.Value);
        Assert.Equal("HELLO", resultB.Value);
    }

    /// <summary>
    /// Tests that query handlers can be registered and resolved from the service collection.
    /// </summary>
    /// <remarks>
    /// This test verifies that the query handlers are correctly registered in the service collection
    /// and can be resolved as expected.
    /// </remarks>
    [Fact]
    public async void AddQueryHandlers_registers_query_handler_and_resolves()
    {
        var services = new ServiceCollection();
        var types = new[] { typeof(QryAHandler) };

        services.AddQueryHandlers(types);

        var desc = services.Single(d =>
            d.ServiceType == typeof(IQueryHandler<QryA, string>) &&
            d.ImplementationType == typeof(QryAHandler));

        Assert.Equal(ServiceLifetime.Scoped, desc.Lifetime);

        using var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();

        var h = scope.ServiceProvider.GetRequiredService<IQueryHandler<QryA, string>>();
        var id = Guid.NewGuid();
        var res = await h.HandleAsync(new QryA(id), default);

        Assert.True(res.IsSuccess);
        Assert.Equal(id.ToString("N"), res.Value);
    }

    /// <summary>
    /// Tests that command handlers can be registered from an array of types.
    /// </summary>
    /// <remarks>
    /// This test verifies that the `AddCommandHandlers` method can accept an array of types
    /// and register them correctly in the service collection.
    /// </remarks>
    [Fact]
    public void Skips_abstract_and_open_generic_types()
    {
        var services = new ServiceCollection();
        var types = new[] { typeof(AbstractHandler), typeof(OpenGenericHandler<>) };

        services.AddCommandHandlers(types);

        Assert.DoesNotContain(services, d => d.ImplementationType == typeof(AbstractHandler));
        Assert.DoesNotContain(services, d => d.ImplementationType == typeof(OpenGenericHandler<>));
    }

    /// <summary>
    /// Tests that duplicate type entries in the command handler registration are deduplicated.
    /// </summary>
    /// <remarks>
    /// This test verifies that if the same type is registered multiple times,
    /// it is only registered once in the service collection.
    /// </remarks>
    [Fact]
    public void De_dupes_duplicate_type_entries()
    {
        var services = new ServiceCollection();
        var types = new[] { typeof(HandlerA), typeof(HandlerA) }; // repeated

        services.AddCommandHandlers(types);

        var matches = services.Where(d =>
            d.ServiceType == typeof(ICommandHandler<CmdA, int>) &&
            d.ImplementationType == typeof(HandlerA)).ToList();

        Assert.Single(matches); // registered once
    }

    /// <summary>
    /// Tests that null arguments to the `AddCommandHandlers` method throw an `ArgumentNullException`.
    /// </summary>
    /// <remarks>
    /// This test verifies that the method correctly guards against null arguments
    /// and throws the appropriate exception.
    /// </remarks>
    [Fact]
    public void Null_guards_throw()
    {
        var services = new ServiceCollection();

        Assert.Throws<ArgumentNullException>(() =>
            ServiceCollectionNullShim.AddCommandHandlers(null!, new[] { typeof(HandlerA) }));

        Assert.Throws<ArgumentNullException>(() =>
            services.AddCommandHandlers(null!));
    }
}

/// <summary>
/// Shim class to allow testing of the `AddCommandHandlers` method
/// </summary>
internal static class ServiceCollectionNullShim
{
    /// <summary>
    /// Adds command handlers to the service collection from an array of types.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="types"></param>
    /// <returns></returns>
    public static IServiceCollection AddCommandHandlers(IServiceCollection services, IEnumerable<Type> types)
        => CQRSServiceCollectionExtensions.AddCommandHandlers(services, types);
}