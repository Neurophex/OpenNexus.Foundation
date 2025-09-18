using OpenNexus.Foundation.DDD.Core;

namespace OpenNexus.Foundation.DDD.Tests.Core;

// Concrete testclass
public sealed class Money : ValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }

    public Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}

public class ValueObjectTests
{
    [Fact]
    public void Equals_Should_Return_True_For_Same_Values()
    {
        var money1 = new Money(10.0m, "EUR");
        var money2 = new Money(10.0m, "EUR");

        Assert.True(money1.Equals(money2));
        Assert.True(money1 == money2);
    }

    [Fact]
    public void Equals_Should_Return_False_For_Different_Values()
    {
        var money1 = new Money(10.0m, "EUR");
        var money2 = new Money(20.0m, "USD");

        Assert.False(money1.Equals(money2));
        Assert.True(money1 != money2);
    }

    [Fact]
    public void Equals_Should_Return_False_For_Null()
    {
        var money = new Money(10.0m, "EUR");

        Assert.False(money.Equals(null));
        Assert.True(money != null);
    }

    [Fact]
    public void GetHashCode_Should_Be_Equal_For_Same_Values()
    {
        var money1 = new Money(10.0m, "EUR");
        var money2 = new Money(10.0m, "EUR");

        Assert.Equal(money1.GetHashCode(), money2.GetHashCode());
    }

    [Fact]
    public void ToString_Should_Contain_TypeName_And_Values()
    {
        var money = new Money(10.0m, "EUR");

        var result = money.ToString();

        Assert.Contains("Money", result);
        Assert.Contains("10.0", result); // Might vary on culture/format
        Assert.Contains("EUR", result);
    }
}