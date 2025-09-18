using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Tests.Types;

// Simple test implementation of DomainSetBase
public class TestDomainSet : DomainSetBase<string>
{
    public TestDomainSet() : base() { }
    public TestDomainSet(params ISetValidator<string>[] validators) : base(validators) { }
}

// Dummy validator for testing
public class StringRejectValueValidator : ISetValidator<string>
{
    private readonly string _reject;
    public StringRejectValueValidator(string reject) => _reject = reject;

    public Result Validate(string item, IReadOnlySet<string> existingItems)
    {
        if (item == _reject)
            return Result.Error($"Item '{_reject}' is not allowed.");
        return Result.Success();
    }
}

public class DomainSetBaseTests
{
    [Fact]
    public void Add_ShouldAddItem_WhenValid()
    {
        var set = new TestDomainSet();

        var result = set.Add("A");

        Assert.True(result.IsSuccess);
        Assert.True(set.Contains("A"));
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldFail_WhenDuplicate()
    {
        var set = new TestDomainSet();
        set.Add("A");

        var result = set.Add("A");

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldFail_WhenValidatorRejects()
    {
        var set = new TestDomainSet(new StringRejectValueValidator("X"));

        var result = set.Add("X");

        Assert.False(result.IsSuccess);
        Assert.Equal("Item 'X' is not allowed.", result.GetErrorMessage());
        Assert.Empty(set);
    }

    [Fact]
    public void Remove_ShouldRemove_WhenItemExists()
    {
        var set = new TestDomainSet();
        set.Add("A");

        var result = set.Remove("A");

        Assert.True(result.IsSuccess);
        Assert.False(set.Contains("A"));
        Assert.Empty(set);
    }

    [Fact]
    public void Remove_ShouldFail_WhenItemNotExists()
    {
        var set = new TestDomainSet();

        var result = set.Remove("B");

        Assert.False(result.IsSuccess);
        Assert.Equal("Item does not exist in the collection.", result.GetErrorMessage());
    }

    [Fact]
    public void ReplaceAll_ShouldReplaceItems()
    {
        var set = new TestDomainSet();
        set.Add("A");
        set.Add("B");

        var result = set.ReplaceAll(new[] { "X", "Y" });

        Assert.True(result.IsSuccess);
        Assert.False(set.Contains("A"));
        Assert.Contains("X", set.AsEnumerable());
        Assert.Equal(2, set.Count);
    }

    [Fact]
    public void AsReadOnlySet_ShouldReturnImmutableSet()
    {
        var set = new TestDomainSet();
        set.Add("A");

        var readOnly = set.AsReadOnlySet();

        Assert.Contains("A", readOnly);
        Assert.IsAssignableFrom<IReadOnlySet<string>>(readOnly);
    }

    [Fact]
    public void Find_ShouldReturnItem_WhenExists()
    {
        var set = new TestDomainSet();
        set.Add("A");

        var result = set.Find(i => i == "A");

        Assert.True(result.IsSuccess);
        Assert.Equal("A", result.Value);
    }

    [Fact]
    public void Find_ShouldFail_WhenNotExists()
    {
        var set = new TestDomainSet();

        var result = set.Find(i => i == "Z");

        Assert.False(result.IsSuccess);
        Assert.Equal("Item not found.", result.GetErrorMessage());
    }

    [Fact]
    public void FindAll_ShouldReturnMatchingItems()
    {
        var set = new TestDomainSet();
        set.AddRange(new[] { "Apple", "Banana", "Apricot" });

        var result = set.FindAll(i => i.StartsWith("A"));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Length);
        Assert.Contains("Apple", result.Value);
        Assert.Contains("Apricot", result.Value);
    }

    [Fact]
    public void AddRange_ShouldStopOnFirstFailure()
    {
        var set = new TestDomainSet(new StringRejectValueValidator("X"));
        var result = set.AddRange(new[] { "A", "X", "B" });

        Assert.False(result.IsSuccess);
        Assert.Contains("A", set.AsEnumerable());
        Assert.DoesNotContain("B", set.AsEnumerable());
    }
}