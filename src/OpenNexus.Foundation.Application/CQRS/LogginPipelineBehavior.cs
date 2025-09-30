using System.Diagnostics;
using Microsoft.Extensions.Logging;
using OpenNexus.Foundation.CQRS;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Application;

/// <summary>
/// Pipeline behavior for logging request and response details.
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class LogginPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
    where TResponse : notnull
{
    /// <summary>
    /// Logger for the <see cref="LogginPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    private readonly ILogger<LogginPipelineBehavior<TRequest, TResponse>> _logger;
    private readonly IClock _clock;

    /// <summary>
    /// Initializes a new instance of the <see cref="LogginPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LogginPipelineBehavior(ILogger<LogginPipelineBehavior<TRequest, TResponse>> logger, IClock clock)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    /// <summary>
    /// Handles the specified request and logs the request details before passing it to the next handler in the pipeline.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="next"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<Result<TResponse>> HandleAsync(TRequest request, PipelineDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Get the request name and the current time for logging purposes
        var requestName = typeof(TRequest).Name;
        var curTime = _clock.Now;

        // Log the start of handling the request with its name and timestamp
        _logger.LogInformation("Handling {RequestName} started at: {TimeStamp}", requestName, curTime);

        // Create a new stopwatch to measure the duration of the request handling
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Call the next handler in the pipeline to process the request
            var result = await next(cancellationToken);

            // Stop the stopwatch to measure the elapsed time
            stopwatch.Stop();

            // Check if the result is successful and log the completion of handling the request
            if (result.IsSuccess)
            {
                _logger.LogInformation("Handling {RequestName} completed successfully in {ElapsedMilliseconds} ms at: {TimeStamp}",
                    requestName, stopwatch.ElapsedMilliseconds, _clock.Now);
            }
            else
            {
                // Log the failure of handling the request as an warning with the error message
                _logger.LogWarning("Handling {RequestName} failed with error: {ErrorMessage} at: {TimeStamp}",
                    requestName, result.GetErrorMessage(), _clock.Now);
            }

            // Return the result of the request handling
            return result;
        }
        catch (Exception ex)
        {
            // Stop the stopwatch
            stopwatch.Stop();

            // Log the exception that occurred during the request handling as an error
            _logger.LogError(ex, "An error occurred while handling {RequestName} at: {TimeStamp}",
                requestName, _clock.Now);

            // Return an error result with the exception message
            return Result<TResponse>.Error($"An unexpected error occurred in {requestName}: {ex.Message}");
        }
    }
}
