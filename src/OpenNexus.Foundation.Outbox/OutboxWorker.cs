using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace OpenNexus.Foundation.Outbox;

public sealed class OutboxWorker : BackgroundService
{
    // Unique ID for this worker instance (useful for logging and debugging)
    private readonly Guid _instanceId = Guid.NewGuid();

    // The logger instance
    private readonly ILogger<OutboxWorker> _logger;

    // The outbox store and dispatcher instances
    private readonly IOutboxStore _store;
    private readonly IOutboxDispatcher _dispatcher;

    // Configuration options - could be made configurable via DI in the future
    private readonly OutboxWorkerOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="OutboxWorker"/> class.
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="store">The store object</param>
    /// <param name="dispatcher">The dispatcher object</param>
    /// <exception cref="ArgumentNullException"></exception>
    public OutboxWorker(ILogger<OutboxWorker> logger, IOutboxStore store, IOutboxDispatcher dispatcher, IOptions<OutboxWorkerOptions> options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _store = store ?? throw new ArgumentNullException(nameof(store));
        _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));

        // Load options or use defaults if not provided
        _options = options?.Value ?? new OutboxWorkerOptions();
    }

    /// <summary>
    /// This method is called when the <see cref="IHostedService"/> starts.
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Log that the worker is starting
        _logger.LogInformation("Outbox Worker({InstanceId}) started.", _instanceId);

        // Main processing loop
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Release all the expired leases first so they can be picked up in the next batch
                var releaseResult = await _store.ReleaseExpiredLeasesAsync(stoppingToken);
                if (!releaseResult.IsSuccess)
                {
                    _logger.LogError("Failed to release expired leases: {ErrorMessage}", releaseResult.GetErrorMessage());
                }

                // Reserve a batch of messages to process
                var reserveResult = await _store.ReserveBatchAsync(_options.MaxBatchSize, _options.LeaseDuration, stoppingToken);

                // Check if reserving the batch was successful
                if (!reserveResult.IsSuccess)
                {
                    // Log the error and wait for the next poll interval
                    _logger.LogError("Failed to reserve outbox batch: {ErrorMessage}", reserveResult.GetErrorMessage());
                    await Task.Delay(_options.PollInterval, stoppingToken);
                    continue;
                }

                // Get the messages to process from the result
                var messages = reserveResult.Value;

                // If there are no messages, wait for the next poll interval
                if (messages.Count == 0)
                {
                    // Log a debug message indicating no messages to process
                    _logger.LogDebug("No outbox messages to process. Waiting for the next poll interval.");

                    // Wait for the next poll interval before checking again
                    await Task.Delay(_options.PollInterval, stoppingToken);
                    continue;
                }

                // Log the number of messages reserved for processing
                _logger.LogInformation("Worker {InstanceId} reserved {Count} messages.", _instanceId, messages.Count);

                // Process each message in the reserved batch
                foreach (var message in messages)
                {
                    // Check if cancellation was requested to gracefully exit
                    if (stoppingToken.IsCancellationRequested)
                    {
                        _logger.LogInformation("Cancellation requested while processing messages. Stopping outbox worker {InstanceId}.", _instanceId);
                        break;
                    }

                    // Wrap in a additional try-catch to ensure one failing message doesn't stop the whole batch
                    try
                    {
                        // Dispatch the message to the external system
                        var dispatchResult = await _dispatcher.DispatchAsync(message, stoppingToken);

                        // Check if dispatch was successful
                        if (dispatchResult.IsSuccess)
                        {
                            // If dispatch was successful, mark the message as processed
                            var markResult = await _store.MarkAsProcessedAsync(message.Id, stoppingToken);
                            if (!markResult.IsSuccess)
                            {
                                // Log an error if marking as processed failed
                                _logger.LogError("Failed to mark outbox message {MessageId} as processed: {ErrorMessage}", message.Id, markResult.GetErrorMessage());
                            }
                        }
                        else
                        {
                            // If dispatch failed, mark the message as failed with the error message
                            var markResult = await _store.MarkAsFailedAsync(message.Id, dispatchResult.GetErrorMessage(), stoppingToken);
                            if (!markResult.IsSuccess)
                            {
                                // Log an error if marking as failed failed
                                _logger.LogError("Failed to mark outbox message {MessageId} as failed: {ErrorMessage}", message.Id, markResult.GetErrorMessage());
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process outbox message {MessageId}.", message.Id);
                        // Release the lease so it can be retried later
                        var releaseLeaseResult = await _store.ReleaseLeaseAsync(message.Id, stoppingToken);
                        if (!releaseLeaseResult.IsSuccess)
                        {
                            _logger.LogError("Failed to release lease for message {MessageId}: {ErrorMessage}", message.Id, releaseLeaseResult.GetErrorMessage());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox loop failed.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        // Log that the worker is stopping
        _logger.LogInformation("Outbox Worker({InstanceId}) stopped.", _instanceId);
    }
}
