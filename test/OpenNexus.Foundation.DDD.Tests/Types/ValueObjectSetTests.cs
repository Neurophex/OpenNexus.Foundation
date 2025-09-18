using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Types;

namespace OpenNexus.Foundation.DDD.Tests.Types;

// ----- Simple fake value object for testing -----
internal sealed class DummyValueObject : ValueObject
{
    public string Name { get; }

    public DummyValueObject(string name) => Name = name;

    // ValueObject equality members
    public override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}

public class ValueObjectCollectionTests
{
    [Fact]
    public void Constructor_Empty_CreatesEmptyCollection()
    {
        var collection = new ValueObjectSet<DummyValueObject>();

        Assert.Equal(0, collection.Count);
        Assert.Empty(collection.AsEnumerable());
    }

    [Fact]
    public void Constructor_WithItems_PopulatesCollection()
    {
        var a = new DummyValueObject("A");
        var b = new DummyValueObject("B");

        var collection = new ValueObjectSet<DummyValueObject>(new[] { a, b });

        Assert.Equal(2, collection.Count);
        Assert.True(collection.Contains(a));
        Assert.True(collection.Contains(b));
    }

    [Fact]
    public void Add_AddsNewItem()
    {
        var collection = new ValueObjectSet<DummyValueObject>();
        var item = new DummyValueObject("X");

        var result = collection.Add(item);

        Assert.True(result.IsSuccess);
        Assert.True(collection.Contains(item));
        Assert.Equal(1, collection.Count);
    }

    [Fact]
    public void Add_ReturnsError_WhenItemAlreadyExists()
    {
        var item = new DummyValueObject("X");
        var collection = new ValueObjectSet<DummyValueObject>(new[] { item });

        var result = collection.Add(new DummyValueObject("X")); // same value

        Assert.False(result.IsSuccess);
        Assert.Contains("exists", result.GetErrorMessage(), StringComparison.OrdinalIgnoreCase);
        Assert.Equal(1, collection.Count);
    }

    [Fact]
    public void Remove_RemovesExistingItem()
    {
        var item = new DummyValueObject("Y");
        var collection = new ValueObjectSet<DummyValueObject>(new[] { item });

        var result = collection.Remove(item);

        Assert.True(result.IsSuccess);
        Assert.False(collection.Contains(item));
        Assert.Equal(0, collection.Count);
    }

    [Fact]
    public void Remove_ReturnsError_WhenItemNotFound()
    {
        var collection = new ValueObjectSet<DummyValueObject>();
        var item = new DummyValueObject("Z");

        var result = collection.Remove(item);

        Assert.False(result.IsSuccess);
        Assert.Contains("does not exist", result.GetErrorMessage(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReplaceAll_ReplacesAllItems()
    {
        var collection = new ValueObjectSet<DummyValueObject>(
            new[] { new DummyValueObject("Old") });
        var newItems = new[] { new DummyValueObject("New1"), new DummyValueObject("New2") };

        var result = collection.ReplaceAll(newItems);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, collection.Count);
        Assert.True(collection.Contains(newItems[0]));
        Assert.True(collection.Contains(newItems[1]));
    }

    [Fact]
    public void AddRange_AddsMultipleItems()
    {
        var collection = new ValueObjectSet<DummyValueObject>();
        var items = new[] { new DummyValueObject("A"), new DummyValueObject("B") };

        var result = collection.AddRange(items);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, collection.Count);
        Assert.All(items, i => Assert.True(collection.Contains(i)));
    }

    [Fact]
    public void AddRange_ReturnsError_OnFirstDuplicate()
    {
        var a = new DummyValueObject("A");
        var b = new DummyValueObject("B");
        var c = new DummyValueObject("A"); // duplicate of a

        var collection = new ValueObjectSet<DummyValueObject>(new[] { a });
        var result = collection.AddRange(new[] { b, c });

        Assert.False(result.IsSuccess);
        Assert.Contains("exists", result.GetErrorMessage(), StringComparison.OrdinalIgnoreCase);
        Assert.True(collection.Contains(b)); // B should have been added before failure
    }

    [Fact]
    public void Find_ReturnsMatchingItem_WhenExists()
    {
        var a = new DummyValueObject("FindMe");
        var collection = new ValueObjectSet<DummyValueObject>(new[] { a });

        var result = collection.Find(i => i.Name == "FindMe");

        Assert.True(result.IsSuccess);
        Assert.Equal("FindMe", result.Value.Name);
    }

    [Fact]
    public void Find_ReturnsError_WhenNotFound()
    {
        var collection = new ValueObjectSet<DummyValueObject>();

        var result = collection.Find(i => i.Name == "Missing");

        Assert.False(result.IsSuccess);
        Assert.Contains("not found", result.GetErrorMessage(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void FindAll_ReturnsAllMatchingItems()
    {
        var a = new DummyValueObject("Match1");
        var b = new DummyValueObject("Nope");
        var c = new DummyValueObject("Match2");
        var collection = new ValueObjectSet<DummyValueObject>(new[] { a, b, c });

        var result = collection.FindAll(i => i.Name.Contains("Match"));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Length);
        Assert.Contains(a, collection.AsEnumerable());
        Assert.Contains(c, collection.AsEnumerable());
    }

    [Fact]
    public void AsReadOnlySet_ReturnsSameItemsAndCannotBeModified()
    {
        var a = new DummyValueObject("A");
        var collection = new ValueObjectSet<DummyValueObject>(new[] { a });

        var readOnly = collection.AsReadOnlySet();

        Assert.True(readOnly.Contains(a));
        Assert.Equal(collection.Count, readOnly.Count);

        // readOnly is an IReadOnlySet<T>; we can enumerate but not modify
        Assert.Throws<NotSupportedException>(() =>
        {
            // Cast to ICollection<T> to test immutability
            var col = (ICollection<DummyValueObject>)readOnly;
            col.Add(new DummyValueObject("B"));
        });
    }
}