namespace OpenNexus.Foundation.Outbox;

/// <summary>
/// Enumeration representing the status of an outbox message.
/// </summary>
public enum EnumOutboxMessageStatus
{
    /// <summary>
    /// The message is pending processing.
    /// </summary>
    Pending,

    /// <summary>
    /// The message is reserved for processing.
    /// </summary>
    Reserved,

    /// <summary>
    /// The message has been successfully processed.
    /// </summary>
    Processed,

    /// <summary>
    /// The message processing has failed.
    /// </summary>
    Failed
}
