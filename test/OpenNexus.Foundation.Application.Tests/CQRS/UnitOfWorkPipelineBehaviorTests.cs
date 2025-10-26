// using Microsoft.Extensions.Logging;
// using Moq;
// using OpenNexus.Foundation.Application;
// using OpenNexus.Foundation.CQRS;
// using OpenNexus.Foundation.Persistence;
// using OpenNexus.Foundation.Primitives;

// namespace OpenNexus.Foundation.Application.Tests.CQRS;

// public class UnitOfWorkPipelineBehaviorTests
// {
//     private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
//     private readonly Mock<ILogger<UnitOfWorkPipelineBehavior<TestCommand, string>>> _mockLogger = new();
//     private readonly CancellationToken _ct = CancellationToken.None;

//     private UnitOfWorkPipelineBehavior<TestCommand, string> CreateSut() =>
//         new(_mockUnitOfWork.Object, _mockLogger.Object);

//     private static readonly Result<string> _successResult = Result<string>.Success("ok");
//     private static readonly Result<string> _errorResult = Result<string>.Error("failure");

//     [Fact]
//     public async Task HandleAsync_Commits_OnSuccess()
//     {
//         // Arrange
//         var sut = CreateSut();
//         var command = new TestCommand();

//         PipelineDelegate<string> next = _ => Task.FromResult(_successResult);

//         // Act
//         var result = await sut.HandleAsync(command, next, _ct);

//         // Assert
//         Assert.True(result.IsSuccess);
//         _mockUnitOfWork.Verify(u => u.CommitAsync(_ct), Times.Once);
//     }

//     [Fact]
//     public async Task HandleAsync_DoesNotCommit_OnFailure()
//     {
//         // Arrange
//         var sut = CreateSut();
//         var command = new TestCommand();

//         PipelineDelegate<string> next = _ => Task.FromResult(_errorResult);

//         // Act
//         var result = await sut.HandleAsync(command, next, _ct);

//         // Assert
//         Assert.False(result.IsSuccess);
//         _mockUnitOfWork.Verify(u => u.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
//     }

//     [Fact]
//     public async Task HandleAsync_ReturnsError_WhenCommitThrows()
//     {
//         // Arrange
//         var sut = CreateSut();
//         var command = new TestCommand();

//         _mockUnitOfWork.Setup(u => u.CommitAsync(_ct)).ThrowsAsync(new Exception("boom"));

//         PipelineDelegate<string> next = _ => Task.FromResult(_successResult);

//         // Act
//         var result = await sut.HandleAsync(command, next, _ct);

//         // Assert
//         Assert.False(result.IsSuccess);
//         Assert.Equal("boom", result.GetErrorMessage());

//         Assert.Single(_mockLogger.Invocations);

//         Assert.NotNull(_mockLogger.Invocations.First(
//             i => i.Arguments[0] is LogLevel level && level == LogLevel.Error && 
//             i.Arguments[2].ToString()!.Contains("An error occurred while trying to commit the command: TestCommand")
//         ));

//     }

//     public record TestCommand : ICommand<string>;
// }
