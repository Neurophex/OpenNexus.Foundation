using Microsoft.Extensions.Logging;
using OpenNexus.Foundation.CQRS.Core;
using OpenNexus.Foundation.Persistence;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Application;

/// <summary>
/// Unit of Work Pipeline Behavior for Commands
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public sealed class UnitOfWorkPipelineBehavior<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
    // The unit of work instance to manage transactions
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UnitOfWorkPipelineBehavior<TCommand, TResponse>>? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkPipelineBehavior{TCommand, TResponse}"/> class.
    /// </summary>
    /// <param name="unitOfWork"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UnitOfWorkPipelineBehavior(IUnitOfWork unitOfWork, ILogger<UnitOfWorkPipelineBehavior<TCommand, TResponse>>? logger)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _logger = logger;
    }

    /// <summary>
    /// Handles the command request and commits the changes to the unit of work if successful.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Result<TResponse>> HandleAsync(TCommand request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Call the next handler in the pipeline
        var result = await next(cancellationToken);

        // Check if the result is an error so we can log it and return an error result
        if (!result.IsSuccess)
        {
            // If the result is an error, log it and return the error result
            _logger?.LogError("Command execution failed: {CommandName} - Error: {ErrorMessage}", typeof(TCommand).Name, result.GetErrorMessage());
            return Result<TResponse>.Error(result.GetErrorMessage());
        }

        try
        {
            // Commit the changes to the unit of work
            await _unitOfWork.CommitAsync(cancellationToken);
        }
        catch(Exception e)
        {
            // Log the exception
            _logger?.LogError(e, "An error occurred while trying to commit the command: {CommandName}", typeof(TCommand).Name);
            // Rollback the changes in case of an exception
            // TODO: await _unitOfWork.RollbackAsync(cancellationToken);

            // Return an error result with the exception message
            return Result<TResponse>.Error(e.Message);
        }

        // Return the result of the command execution
        return result;
    }
}
