namespace OpenNexus.Foundation.DDD;

/// <summary>
/// Base class for domain events.
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    private readonly Guid _eventId = Guid.NewGuid();
    private readonly DateTime _occurredOn = DateTime.UtcNow;
    private readonly string? _correlationId = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEventBase"/> class.
    /// </summary>
    /// <param name="correlationId"></param>
    public DomainEventBase(string? correlationId = null)
    {
        _correlationId = correlationId;
    }

    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    public DateTime OccuredOn => _occurredOn;

    /// <summary>
    /// Gets the correlation ID for the event, if any.
    /// </summary>
    public string? CorrelationId => _correlationId;

    /// <summary>
    /// Gets the unique identifier for the event.
    /// </summary>
    public Guid EventId => _eventId;
}
