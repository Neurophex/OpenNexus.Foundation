using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Tests.Types;

// Dummy entity for testing
public sealed class EntitySetTests_GuidEntity : Entity<Guid>
{
    public string Name { get; }

    public EntitySetTests_GuidEntity(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public override string ToString() => $"{Name} ({Id})";
}

public sealed class EntitySetTests_IntEntity : Entity<int>
{
    public string Name { get; }
    public EntitySetTests_IntEntity(int id, string name) : base(id) => Name = name;
    public override string ToString() => $"{Name} ({Id})";
}

public sealed class EntitySetTests_StringEntity : Entity<string>
{
    public string Description { get; }
    public EntitySetTests_StringEntity(string id, string description) : base(id) => Description = description;
    public override string ToString() => $"{Description} ({Id})";
}

public record EntitySetTests_CustomId(int Value);

public sealed class EntitySetTests_CustomIdEntity : Entity<EntitySetTests_CustomId>
{
    public string Label { get; }
    public EntitySetTests_CustomIdEntity(EntitySetTests_CustomId id, string label) : base(id) => Label = label;
    public override string ToString() => $"{Label} ({Id.Value})";
}

public sealed class EntitySetTests_MixedEntity : Entity<int>
{
    public string Label { get; }
    public EntitySetTests_MixedEntity(int id, string label) : base(id) => Label = label;
    public override string ToString() => $"{Label} ({Id})";
}

public class EntitySetTests_RejectXValidator<TEntity, TId> : ISetValidator<TEntity>
        where TEntity : Entity<TId>
        where TId : notnull
{
    public Result Validate(TEntity item, IReadOnlySet<TEntity> existingItems)
    {
        if (item.ToString().Contains("X"))
            return Result.Error($"Entity '{item}' is not allowed.");
        return Result.Success();
    }
}

// Validator that rejects entities with a given name
public class EntitySetTests_RejectNameValidator : ISetValidator<EntitySetTests_GuidEntity>
{
    private readonly string _reject;
    public EntitySetTests_RejectNameValidator(string reject) => _reject = reject;

    public Result Validate(EntitySetTests_GuidEntity item, IReadOnlySet<EntitySetTests_GuidEntity> existingItems)
    {
        if (item.Name == _reject)
            return Result.Error($"Entity with name '{_reject}' is not allowed.");
        return Result.Success();
    }
}

public class EntitySetTests
{
    [Fact]
    public void Add_ShouldAddEntity()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();
        var entity = new EntitySetTests_GuidEntity(Guid.NewGuid(), "Alice");

        var result = set.Add(entity);

        Assert.True(result.IsSuccess);
        Assert.Contains(entity, set.AsEnumerable());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldRejectDuplicateEntityById()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();
        var id = Guid.NewGuid();
        var e1 = new EntitySetTests_GuidEntity(id, "First");
        var e2 = new EntitySetTests_GuidEntity(id, "Second");

        set.Add(e1);
        var result = set.Add(e2);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldRespectValidator()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>(new EntitySetTests_RejectNameValidator("Forbidden"));
        var entity = new EntitySetTests_GuidEntity(Guid.NewGuid(), "Forbidden");

        var result = set.Add(entity);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity with name 'Forbidden' is not allowed.", result.GetErrorMessage());
        Assert.Empty(set.AsEnumerable());
    }

    [Fact]
    public void FindById_ShouldReturnEntity_WhenExists()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();
        var id = Guid.NewGuid();
        var entity = new EntitySetTests_GuidEntity(id, "Alice");
        set.Add(entity);

        var result = set.FindById(id);

        Assert.True(result.IsSuccess);
        Assert.Equal(entity, result.Value);
    }

    [Fact]
    public void FindById_ShouldReturnError_WhenNotFound()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();

        var result = set.FindById(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Item not found.", result.GetErrorMessage());
    }

    [Fact]
    public void ContainsId_ShouldReturnTrue_WhenExists()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();
        var id = Guid.NewGuid();
        set.Add(new EntitySetTests_GuidEntity(id, "Alice"));

        Assert.True(set.ContainsId(id));
    }

    [Fact]
    public void ContainsId_ShouldReturnFalse_WhenNotExists()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();

        Assert.False(set.ContainsId(Guid.NewGuid()));
    }

    [Fact]
    public void ReplaceAll_ShouldReplaceEntities()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();
        set.Add(new EntitySetTests_GuidEntity(Guid.NewGuid(), "Old"));

        var newEntities = new[]
        {
                new EntitySetTests_GuidEntity(Guid.NewGuid(), "New1"),
                new EntitySetTests_GuidEntity(Guid.NewGuid(), "New2")
            };

        var result = set.ReplaceAll(newEntities);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, set.Count);
        Assert.All(newEntities, e => Assert.Contains(e, set.AsEnumerable()));
    }

    [Fact]
    public void CreateValidated_ShouldReturnSuccess_WhenAllValid()
    {
        var items = new[]
        {
                new EntitySetTests_GuidEntity(Guid.NewGuid(), "Alice"),
                new EntitySetTests_GuidEntity(Guid.NewGuid(), "Bob")
            };

        var result = EntitySet<EntitySetTests_GuidEntity, Guid>.CreateValidated(items);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public void CreateValidated_ShouldFail_WhenValidatorRejects()
    {
        var items = new[]
        {
                new EntitySetTests_GuidEntity(Guid.NewGuid(), "Alice"),
                new EntitySetTests_GuidEntity(Guid.NewGuid(), "Forbidden")
            };

        var result = EntitySet<EntitySetTests_GuidEntity, Guid>.CreateValidated(items, new EntitySetTests_RejectNameValidator("Forbidden"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity with name 'Forbidden' is not allowed.", result.GetErrorMessage());
    }

    // === INT identity ===
    [Fact]
    public void IntEntitySet_ShouldFindById()
    {
        var set = new EntitySet<EntitySetTests_IntEntity, int>();
        var e = new EntitySetTests_IntEntity(42, "Answer");
        set.Add(e);

        var result = set.FindById(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(e, result.Value);
    }

    [Fact]
    public void IntEntitySet_ShouldRejectDuplicateId()
    {
        var set = new EntitySet<EntitySetTests_IntEntity, int>();
        var e1 = new EntitySetTests_IntEntity(1, "First");
        var e2 = new EntitySetTests_IntEntity(1, "Duplicate");
        set.Add(e1);

        var result = set.Add(e2);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());
    }

    // === STRING identity ===
    [Fact]
    public void StringEntitySet_ShouldContainId()
    {
        var set = new EntitySet<EntitySetTests_StringEntity, string>();
        var e = new EntitySetTests_StringEntity("abc", "Test");
        set.Add(e);

        Assert.True(set.ContainsId("abc"));
        Assert.False(set.ContainsId("zzz"));
    }

    [Fact]
    public void StringEntitySet_CreateValidated_ShouldApplyValidator()
    {
        var items = new[]
        {
                new EntitySetTests_StringEntity("ok", "Good"),
                new EntitySetTests_StringEntity("bad", "X-Ray") // contains "X"
            };

        var result = EntitySet<EntitySetTests_StringEntity, string>.CreateValidated(items, new EntitySetTests_RejectXValidator<EntitySetTests_StringEntity, string>());

        Assert.False(result.IsSuccess);
        Assert.Contains("not allowed", result.GetErrorMessage());
    }

    // === CUSTOM ID identity ===
    [Fact]
    public void CustomIdEntitySet_ShouldWorkWithRecordId()
    {
        var set = new EntitySet<EntitySetTests_CustomIdEntity, EntitySetTests_CustomId>();
        var id = new EntitySetTests_CustomId(99);
        var entity = new EntitySetTests_CustomIdEntity(id, "Special");
        set.Add(entity);

        Assert.True(set.ContainsId(id));
        var result = set.FindById(new EntitySetTests_CustomId(99));

        Assert.True(result.IsSuccess);
        Assert.Equal(entity, result.Value);
    }

    [Fact]
    public void CustomIdEntitySet_ShouldRejectDuplicates_ByCustomIdEquality()
    {
        var set = new EntitySet<EntitySetTests_CustomIdEntity, EntitySetTests_CustomId>();
        var e1 = new EntitySetTests_CustomIdEntity(new EntitySetTests_CustomId(5), "One");
        var e2 = new EntitySetTests_CustomIdEntity(new EntitySetTests_CustomId(5), "Two");

        set.Add(e1);
        var result = set.Add(e2);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());
    }

    [Fact]
    public void ReplaceAll_ShouldResetAndAllowNewFinds()
    {
        var set = new EntitySet<EntitySetTests_MixedEntity, int>();
        var e1 = new EntitySetTests_MixedEntity(1, "First");
        var e2 = new EntitySetTests_MixedEntity(2, "Second");
        set.AddRange(new[] { e1, e2 });

        // Replace with new set
        var newEntities = new[]
        {
                new EntitySetTests_MixedEntity(10, "Ten"),
                new EntitySetTests_MixedEntity(20, "Twenty")
            };
        set.ReplaceAll(newEntities);

        Assert.Equal(2, set.Count);
        Assert.False(set.ContainsId(1));
        Assert.True(set.ContainsId(10));

        var result = set.FindById(20);
        Assert.True(result.IsSuccess);
        Assert.Equal("Twenty", result.Value.Label);
    }

    [Fact]
    public void Add_Remove_And_Find_ShouldRemainConsistent()
    {
        var set = new EntitySet<EntitySetTests_MixedEntity, int>();
        var a = new EntitySetTests_MixedEntity(1, "Alpha");
        var b = new EntitySetTests_MixedEntity(2, "Beta");

        set.Add(a);
        set.Add(b);

        Assert.True(set.ContainsId(1));
        Assert.True(set.ContainsId(2));

        // Remove Beta
        set.Remove(b);

        Assert.False(set.ContainsId(2));
        Assert.True(set.ContainsId(1));

        // Find Alpha by Id
        var result = set.FindById(1);
        Assert.True(result.IsSuccess);
        Assert.Equal(a, result.Value);
    }

    [Fact]
    public void ReplaceAll_ThenAdd_ShouldMaintainUniquenessById()
    {
        var set = new EntitySet<EntitySetTests_MixedEntity, int>();
        set.ReplaceAll(new[]
        {
                new EntitySetTests_MixedEntity(100, "Hundred"),
                new EntitySetTests_MixedEntity(200, "TwoHundred")
            });

        // Attempt to add duplicate by ID
        var duplicate = new EntitySetTests_MixedEntity(100, "Duplicate");
        var result = set.Add(duplicate);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());

        // Ensure original entity still there
        var found = set.FindById(100);
        Assert.True(found.IsSuccess);
        Assert.Equal("Hundred", found.Value.Label);
    }

    [Fact]
    public void AddRange_ShouldAddAllOrStopOnValidatorFailure()
    {
        var validator = new EntitySetTests_RejectXValidator<EntitySetTests_MixedEntity, int>();
        var set = new EntitySet<EntitySetTests_MixedEntity, int>(validator);

        var entities = new[]
        {
                new EntitySetTests_MixedEntity(1, "Good"),
                new EntitySetTests_MixedEntity(2, "X-Ray"),   // should trigger failure
                new EntitySetTests_MixedEntity(3, "Later")
            };

        var result = set.AddRange(entities);

        Assert.False(result.IsSuccess);
        Assert.Contains("not allowed", result.GetErrorMessage());

        // Should only contain the first "Good"
        Assert.True(set.ContainsId(1));
        Assert.False(set.ContainsId(2));
        Assert.False(set.ContainsId(3));
    }

    [Fact]
    public void ReplaceAll_FollowedByContainsAndFind_ShouldWorkAcrossMultipleOps()
    {
        var set = new EntitySet<EntitySetTests_MixedEntity, int>();
        set.Add(new EntitySetTests_MixedEntity(5, "Old"));

        // Replace with fresh items
        set.ReplaceAll(new[]
        {
                new EntitySetTests_MixedEntity(50, "Fifty"),
                new EntitySetTests_MixedEntity(60, "Sixty")
            });

        Assert.False(set.ContainsId(5));
        Assert.True(set.ContainsId(50));

        var find60 = set.FindById(60);
        Assert.True(find60.IsSuccess);
        Assert.Equal("Sixty", find60.Value.Label);

        // Add another after replace
        set.Add(new EntitySetTests_MixedEntity(70, "Seventy"));
        Assert.True(set.ContainsId(70));
    }
}

