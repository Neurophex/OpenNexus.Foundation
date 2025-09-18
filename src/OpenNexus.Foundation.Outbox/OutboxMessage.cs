namespace OpenNexus.Foundation.Outbox;

/// <summary>
/// Represents a message stored in the outbox for reliable event delivery.
/// </summary>
public sealed class OutboxMessage
{
    #region Immutable Properties
    /// <summary>
    /// Unique identifier for the outbox message.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Type of the event/message.
    /// </summary>
    public DateTime OccurredOn { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Type of the event/message.
    /// </summary>
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// Serialized payload of the event/message.
    /// </summary>
    public string Payload { get; init; } = string.Empty;

    /// <summary>
    /// Correlation identifier for tracing the message through systems.
    /// </summary>
    public string CorrelationId { get; init; } = string.Empty;

    /// <summary>
    /// Causation identifier linking related messages.
    /// </summary>
    public string CausationId { get; init; } = string.Empty;

    #endregion

    #region Mutable Properties

    /// <summary>
    /// Status of the message (e.g., Pending, Processed, Failed).
    /// </summary>
    public EnumOutboxMessageStatus Status { get; set; } = EnumOutboxMessageStatus.Pending;

    /// <summary>
    /// Timestamp when the message was processed.
    /// </summary>
    public DateTime? ProcessedOn { get; set; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Number of processing attempts.
    /// </summary>
    public int AttemptCount { get; set; } = 0;

    /// <summary>
    /// Timestamp until which the message is leased for processing.
    /// </summary>
    public DateTime? LeaseUtil { get; set; }

    #endregion
}
