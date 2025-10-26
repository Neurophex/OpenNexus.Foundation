// using Moq;
// using Microsoft.Extensions.Logging;
// using OpenNexus.Foundation.CQRS;
// using OpenNexus.Foundation.Primitives;
// using OpenNexus.Foundation.Application;

// namespace OpenNexus.Foundation.Application.Tests.CQRS;

// public class LoggingPipelineBehaviorTests
// {
//     private readonly Mock<ILogger<LogginPipelineBehavior<TestCommand, string>>> _mockLogger = new();
//     private readonly Mock<IClock> _mockClock = new();

//     public record TestCommand(string Name) : ICommand<string>;

//     [Fact]
//     public async Task HandleAsync_LogsStartAndSuccess_WhenCommandSucceeds()
//     {
//         // Arrange
//         var fakeTime = DateTime.UtcNow;
//         _mockClock.Setup(c => c.Now).Returns(fakeTime);

//         var behavior = new LogginPipelineBehavior<TestCommand, string>(_mockLogger.Object, _mockClock.Object);
//         var command = new TestCommand("Test");

//         PipelineDelegate<string> next = _ => Task.FromResult(Result<string>.Success("ok"));

//         // Act
//         var result = await behavior.HandleAsync(command, next, CancellationToken.None);

//         // Assert
//         Assert.True(result.IsSuccess);
//         _mockLogger.Verify(l => l.Log(
//             LogLevel.Information,
//             It.IsAny<EventId>(),
//             It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("Handling TestCommand started")),
//             null,
//             It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);

//         _mockLogger.Verify(l => l.Log(
//             LogLevel.Information,
//             It.IsAny<EventId>(),
//             It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("completed successfully")),
//             null,
//             It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
//     }

//     [Fact]
//     public async Task HandleAsync_LogsWarning_WhenCommandReturnsError()
//     {
//         // Arrange
//         _mockClock.Setup(c => c.Now).Returns(DateTime.UtcNow);
//         var behavior = new LogginPipelineBehavior<TestCommand, string>(_mockLogger.Object, _mockClock.Object);
//         var command = new TestCommand("Test");

//         var errorResult = Result<string>.Error("failed");

//         PipelineDelegate<string> next = _ => Task.FromResult(errorResult);

//         // Act
//         var result = await behavior.HandleAsync(command, next, CancellationToken.None);

//         // Assert
//         Assert.False(result.IsSuccess);
//         _mockLogger.Verify(l => l.Log(
//             LogLevel.Warning,
//             It.IsAny<EventId>(),
//             It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("failed with error")),
//             null,
//             It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
//     }

//     [Fact]
//     public async Task HandleAsync_LogsError_WhenExceptionIsThrown()
//     {
//         // Arrange
//         _mockClock.Setup(c => c.Now).Returns(DateTime.UtcNow);
//         var behavior = new LogginPipelineBehavior<TestCommand, string>(_mockLogger.Object, _mockClock.Object);
//         var command = new TestCommand("Test");

//         PipelineDelegate<string> next = _ => throw new InvalidOperationException("Boom");

//         // Act
//         var result = await behavior.HandleAsync(command, next, CancellationToken.None);

//         // Assert
//         Assert.False(result.IsSuccess);
//         Assert.Contains("Boom", result.GetErrorMessage());

//         _mockLogger.Verify(l => l.Log(
//             LogLevel.Error,
//             It.IsAny<EventId>(),
//             It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("An error occurred")),
//             It.IsAny<Exception>(),
//             It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
//     }
// }
