using System.Reflection;
using OpenNexus.Foundation.DDD;

namespace OpenNexus.Foundation.DDD.Core;

/// <summary>
/// A domain event indicating that an order was created.
/// </summary>
public class AggregateRootEventTests_OrderCreated(Guid orderId) : DomainEventBase
{
    public Guid OrderId { get; } = orderId;
}

/// <summary>
/// A domain event indicating that an order line was added.
/// </summary>
public class AggregateRootEventTests_OrderLineAdded(Guid orderId, Guid lineId) : DomainEventBase
{
    public Guid OrderId { get; } = orderId;
    public Guid LineId { get; } = lineId;
}

/// <summary>
/// A domain event indicating that a discount was applied to an order line.
/// </summary>
public class AggregateRootEventTests_DiscountApplied(Guid lineId, string code) : DomainEventBase
{
    public Guid LineId { get; } = lineId;
    public string Code { get; } = code;
}

/// <summary>
/// A discount entity that raises a domain event when created.
/// </summary>
public class AggregateRootEventTests_Discount : EntityBase<Guid>
{
    public string Code { get; }

    public AggregateRootEventTests_Discount(Guid id, string code) : base(id)
    {
        Code = code;
        RaiseDomainEvent(new AggregateRootEventTests_DiscountApplied(id, code));
    }
}

/// <summary>
/// An order line entity that can have discounts and raises a domain event when created.
/// </summary>
public class AggregateRootEventTests_OrderLine : EntityBase<Guid>
{
    private readonly List<AggregateRootEventTests_Discount> _discounts = new();

    public AggregateRootEventTests_OrderLine(Guid id) : base(id)
    {
        RaiseDomainEvent(new AggregateRootEventTests_OrderLineAdded(Guid.NewGuid(), id));
    }

    public void AddDiscount(string code)
    {
        var discount = new AggregateRootEventTests_Discount(Guid.NewGuid(), code);
        _discounts.Add(discount);
    }

    public override IEnumerable<IEntityNode> GetChildNodes() => _discounts;
}

/// <summary>
/// An order aggregate root that raises a domain event when created and can have order lines.
/// </summary>
public class AggregateRootEventTests_Order : AggregateRootBase<Guid>
{
    private readonly List<AggregateRootEventTests_OrderLine> _lines = new();

    public AggregateRootEventTests_Order(Guid id) : base(id)
    {
        RaiseDomainEvent(new AggregateRootEventTests_OrderCreated(id));
    }

    public void AddLine()
    {
        var line = new AggregateRootEventTests_OrderLine(Guid.NewGuid());
        _lines.Add(line);
    }

    public override IEnumerable<IEntityNode> GetChildNodes() => _lines;
}

public class AggregateRootEventTests
{

    [Fact]
    public void PullDomainEvents_ShouldReturnRootEventsOnly_WhenNoChildren()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());

        var events = order.PullDomainEvents().ToList();

        Assert.Single(events);
        Assert.Contains(events, e => e is AggregateRootEventTests_OrderCreated);
        Assert.Empty(order.PollDomainEvents());
    }

    [Fact]
    public void PullDomainEvents_ShouldReturnChildEventsOnly_WhenRootHasNone()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());
        order.ClearDomainEvents(); // clear root’s OrderCreated

        order.AddLine(); // raises OrderLineAdded

        var events = order.PullDomainEvents().ToList();

        Assert.Single(events);
        Assert.Contains(events, e => e is AggregateRootEventTests_OrderLineAdded);
    }

    [Fact]
    public void PullDomainEvents_ShouldReturnGrandchildEvents()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());
        order.ClearDomainEvents(); // clear root events

        order.AddLine();
        var line = order
            .GetType()
            .GetField("_lines", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(order) as List<AggregateRootEventTests_OrderLine>;

        line![0].ClearDomainEvents(); // clear line’s event

        line[0].AddDiscount("XMAS2025");

        var events = order.PullDomainEvents().ToList();

        Assert.Single(events);
        Assert.Contains(events, e => e is AggregateRootEventTests_DiscountApplied);
    }

    [Fact]
    public void PullDomainEvents_ShouldClearEventsFromAllNodes()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());
        order.AddLine();
        var line = order
            .GetType()
            .GetField("_lines", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(order) as List<AggregateRootEventTests_OrderLine>;

        line![0].AddDiscount("WELCOME");

        var events = order.PullDomainEvents().ToList();
        Assert.NotEmpty(events);

        // Poll again → should be empty
        Assert.Empty(order.PollDomainEvents());
    }

    [Fact]
    public void Poll_ShouldNotClearEvents_WhereasPullShould()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());

        var polled = order.PollDomainEvents().ToList();
        Assert.NotEmpty(polled);

        // After poll, events are still there
        Assert.NotEmpty(order.PollDomainEvents());

        var pulled = order.PullDomainEvents().ToList();
        Assert.NotEmpty(pulled);

        // After pull, everything is cleared
        Assert.Empty(order.PollDomainEvents());
    }

    [Fact]
    public void PullDomainEvents_ShouldReturnEmptyOnSecondCall()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());

        var firstPull = order.PullDomainEvents().ToList();
        Assert.NotEmpty(firstPull);

        var secondPull = order.PullDomainEvents().ToList();
        Assert.Empty(secondPull);
    }

    [Fact]
    public void PullDomainEvents_ShouldPreserveEventOrder()
    {
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());
        order.AddLine();
        order.AddLine();

        var events = order.PullDomainEvents().ToList();

        Assert.True(events[0] is AggregateRootEventTests_OrderCreated);
        Assert.Contains(events, e => e is AggregateRootEventTests_OrderLineAdded);
    }

    [Fact]
    public void PullDomainEvents_ShouldCollectAndClear_AllLevels()
    {
        // Arrange
        var order = new AggregateRootEventTests_Order(Guid.NewGuid());
        order.AddLine();
        order.AddLine();

        // Add a discount to the first line
        var firstLine = order.PollDomainEvents()
                             .OfType<AggregateRootEventTests_OrderLineAdded>()
                             .First();
        // Simulate adding a discount to a line
        var line = order
            .GetType()
            .GetField("_lines", global::System.Reflection.BindingFlags.NonPublic | global::System.Reflection.BindingFlags.Instance)!
            .GetValue(order) as List<AggregateRootEventTests_OrderLine>;

        line![0].AddDiscount("WELCOME10");

        // Act
        var events = order.PullDomainEvents().ToList();

        // Assert: root, lines, discount
        Assert.Contains(events, e => e is AggregateRootEventTests_OrderCreated);
        Assert.Contains(events, e => e is AggregateRootEventTests_OrderLineAdded);
        Assert.Contains(events, e => e is AggregateRootEventTests_DiscountApplied);

        // All should be cleared afterwards
        Assert.Empty(order.PollDomainEvents());
    }
}
