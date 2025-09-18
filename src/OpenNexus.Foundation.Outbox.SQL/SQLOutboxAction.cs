namespace OpenNexus.Foundation.Outbox.SQL;

/// <summary>
/// Enumeration of SQL outbox actions for which SQL scripts are available.
/// </summary>
internal enum SQLOutboxAction
{
    CreateTable,
    DropTable,
    Enqueue,
    MarkAsFailed,
    MarkAsProcessed,
    ReleaseExpiredLeases,
    ReleaseLease
}

/// <summary>
/// Provides extension methods for the SQLOutboxAction enum.
/// </summary>
internal static class SQLOutboxActionExtensions
{
    /// <summary>
    /// Convert the SQL outbox action enum to a string representation.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    internal static string AsString(this SQLOutboxAction action) => action switch
    {
        SQLOutboxAction.CreateTable => "CreateTable",
        SQLOutboxAction.DropTable => "DropTable",
        SQLOutboxAction.Enqueue => "EnqueueMessage",
        SQLOutboxAction.MarkAsFailed => "MarkMessageAsFailed",
        SQLOutboxAction.MarkAsProcessed => "MarkMessageAsProcessed",
        SQLOutboxAction.ReleaseExpiredLeases => "ReleaseExpiredLeases",
        SQLOutboxAction.ReleaseLease => "ReleaseLease",
        _ => throw new NotSupportedException($"The SQL outbox action '{action}' is not supported.")
    };
}
