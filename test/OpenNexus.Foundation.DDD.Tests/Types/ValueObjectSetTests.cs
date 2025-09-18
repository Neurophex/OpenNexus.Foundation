using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Tests.Types;

// Dummy ValueObject for testing
public sealed class DummyValueObject : ValueObject
{
    public string Name { get; }

    public DummyValueObject(string name) => Name = name;

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Name;
    }

    public override string ToString() => Name;
}

// Dummy validator
public class RejectNameValidator : ISetValidator<DummyValueObject>
{
    private readonly string _reject;
    public RejectNameValidator(string reject) => _reject = reject;

    public Result Validate(DummyValueObject item, IReadOnlySet<DummyValueObject> existingItems)
    {
        if (item.Name == _reject)
            return Result.Error($"Item '{_reject}' is not allowed.");
        return Result.Success();
    }
}

public class ValueObjectSetTests
{
    [Fact]
    public void NewSet_ShouldBeEmpty()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        Assert.Empty(set);
    }

    [Fact]
    public void Add_ShouldAddValueObject()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        var item = new DummyValueObject("A");

        var result = set.Add(item);

        Assert.True(result.IsSuccess);
        Assert.Contains(item, set.AsEnumerable());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldFailOnDuplicate()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        var item = new DummyValueObject("A");
        set.Add(item);

        var result = set.Add(new DummyValueObject("A")); // same equality

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldRespectValidators()
    {
        var set = new ValueObjectSet<DummyValueObject>(new RejectNameValidator("X"));
        var item = new DummyValueObject("X");

        var result = set.Add(item);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item 'X' is not allowed.", result.GetErrorMessage());
        Assert.Empty(set.AsEnumerable());
    }

    [Fact]
    public void Remove_ShouldWork()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        var item = new DummyValueObject("A");
        set.Add(item);

        var result = set.Remove(item);

        Assert.True(result.IsSuccess);
        Assert.Empty(set.AsEnumerable());
    }

    [Fact]
    public void ReplaceAll_ShouldReplace()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        set.Add(new DummyValueObject("Old"));

        var result = set.ReplaceAll(new[]
        {
                new DummyValueObject("New1"),
                new DummyValueObject("New2")
            });

        Assert.True(result.IsSuccess);
        Assert.DoesNotContain(set.AsEnumerable(), i => i.Name == "Old");
        Assert.Contains(set.AsEnumerable(), i => i.Name == "New1");
        Assert.Contains(set.AsEnumerable(), i => i.Name == "New2");
    }

    [Fact]
    public void Find_ShouldReturnMatch()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        var a = new DummyValueObject("A");
        set.Add(a);

        var result = set.Find(i => i.Name == "A");

        Assert.True(result.IsSuccess);
        Assert.Equal(a, result.Value);
    }

    [Fact]
    public void Find_ShouldReturnError_WhenNotFound()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        set.Add(new DummyValueObject("A"));

        var result = set.Find(i => i.Name == "B");

        Assert.False(result.IsSuccess);
        Assert.Equal("Item not found.", result.GetErrorMessage());
    }

    [Fact]
    public void FindAll_ShouldReturnMultiple()
    {
        var set = new ValueObjectSet<DummyValueObject>();
        set.Add(new DummyValueObject("A1"));
        set.Add(new DummyValueObject("A2"));
        set.Add(new DummyValueObject("B1"));

        var result = set.FindAll(i => i.Name.StartsWith("A"));

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Length);
    }

    [Fact]
    public void CreateValidated_ShouldReturnSuccess_WhenAllValid()
    {
        var items = new[]
        {
                new DummyValueObject("A"),
                new DummyValueObject("B")
            };

        var result = ValueObjectSet<DummyValueObject>.CreateValidated(items);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public void CreateValidated_ShouldFail_WhenValidatorRejects()
    {
        var items = new[]
        {
                new DummyValueObject("A"),
                new DummyValueObject("X")
            };

        var result = ValueObjectSet<DummyValueObject>.CreateValidated(items, new RejectNameValidator("X"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Item 'X' is not allowed.", result.GetErrorMessage());
    }
}
