using Microsoft.Extensions.Logging;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.Outbox.SQL;

/// <summary>
/// SQL-based implementation of the outbox store.
/// </summary>
public sealed class SQLOutboxStore : IOutboxStore
{
    private readonly ILogger<SQLOutboxStore> _logger;
    private readonly SQLOutboxStoreOptions _options;

    public SQLOutboxStore(ILogger<SQLOutboxStore> logger, SQLOutboxStoreOptions options)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<Result> EnqueueAsync(OutboxMessage message, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> MarkAsFailedAsync(Guid messageId, string errorMessage, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> MarkAsProcessedAsync(Guid messageId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ReleaseExpiredLeasesAsync(CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result> ReleaseLeaseAsync(Guid messageId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IReadOnlyList<OutboxMessage>>> ReserveBatchAsync(int batchSize, TimeSpan leaseTime, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}