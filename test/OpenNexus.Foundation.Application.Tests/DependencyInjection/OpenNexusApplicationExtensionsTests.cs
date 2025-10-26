using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace OpenNexus.Foundation.Application.Tests.DependencyInjection;

public class OpenNexusApplicationExtensionsTests
{
    [Fact]
    public async Task InitializeApplicationServices_Should_Invoke_All_Initializers()
    {
        // Arrange
        var init1 = new Mock<IServiceInitializer>();
        var init2 = new Mock<IServiceInitializer>();

        var services = new ServiceCollection()
            .AddSingleton(init1.Object)
            .AddSingleton(init2.Object)
            .BuildServiceProvider();

        // Act
        await services.InitializeApplicationServices();

        // Assert
        init1.Verify(i => i.Initialize(services, It.IsAny<CancellationToken>()), Times.Once);
        init2.Verify(i => i.Initialize(services, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeApplicationServices_Should_Log_Progress_And_Completion()
    {
        // Arrange
        var initializer = new Mock<IServiceInitializer>();
        var loggerMock = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger>();
        loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        var services = new ServiceCollection()
            .AddSingleton(initializer.Object)
            .AddSingleton(loggerMock.Object)
            .BuildServiceProvider();

        // Act
        await services.InitializeApplicationServices();

        // Assert
        loggerMock.Verify(l => l.CreateLogger("AppInitialization"), Times.Once);
        initializer.Verify(i => i.Initialize(services, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task InitializeApplicationServices_Should_Log_And_Propagate_Exception()
    {
        // Arrange
        var failingInitializer = new Mock<IServiceInitializer>();
        failingInitializer
            .Setup(i => i.Initialize(It.IsAny<IServiceProvider>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var loggerMock = new Mock<ILoggerFactory>();
        var logger = new Mock<ILogger>();
        loggerMock.Setup(l => l.CreateLogger(It.IsAny<string>())).Returns(logger.Object);

        var services = new ServiceCollection()
            .AddSingleton(failingInitializer.Object)
            .AddSingleton(loggerMock.Object)
            .BuildServiceProvider();

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => services.InitializeApplicationServices());

        // Verify that error was logged
        logger.Verify(l =>
            l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.Is<InvalidOperationException>(ex => ex.Message == "boom"),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task InitializeApplicationServices_Should_Handle_Empty_Initializers()
    {
        // Arrange
        var services = new ServiceCollection().BuildServiceProvider();

        // Act (should not throw)
        await services.InitializeApplicationServices();

        // Assert - no exception
    }
}
