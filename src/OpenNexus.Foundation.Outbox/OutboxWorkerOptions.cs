namespace OpenNexus.Foundation.Outbox;

/// <summary>
/// Configuration options for the Outbox Worker.
/// </summary>
public class OutboxWorkerOptions
{
    /// <summary>
    /// The interval at which the outbox worker checks for new messages to process.
    /// Default is 5 seconds.
    /// </summary>
    public TimeSpan PollInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// The maximum number of messages to process in a single batch.
    /// Default is 10.
    /// </summary>
    public int MaxBatchSize { get; set; } = 10;

    /// <summary>
    /// The duration for which a message lease is valid.
    /// Default is 1 minute.
    /// </summary>
    public TimeSpan LeaseDuration { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// The maximum number of retry attempts for a failed message before marking it as permanently failed.
    /// Default is 5.
    /// </summary>
    public int MaxRetryCount { get; set; } = 5;
}
