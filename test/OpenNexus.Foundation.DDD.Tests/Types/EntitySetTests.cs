using OpenNexus.Foundation.DDD;
using OpenNexus.Foundation.Primitives;

namespace OpenNexus.Foundation.DDD.Tests.Types;

/// <summary>
/// Dummy entity with GUID identity for testing
/// </summary>
public sealed class EntitySetTests_GuidEntity : Entity<Guid>
{
    public string Name { get; }

    public EntitySetTests_GuidEntity(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public override string ToString() => $"{Name} ({Id})";
}

/// <summary>
/// Dummy entity with INT identity for testing
/// </summary>
public sealed class EntitySetTests_IntEntity : Entity<int>
{
    public string Name { get; }
    public EntitySetTests_IntEntity(int id, string name) : base(id) => Name = name;
    public override string ToString() => $"{Name} ({Id})";
}

/// <summary>
/// Dummy entity with STRING identity for testing
/// </summary>
public sealed class EntitySetTests_StringEntity : Entity<string>
{
    public string Description { get; }
    public EntitySetTests_StringEntity(string id, string description) : base(id) => Description = description;
    public override string ToString() => $"{Description} ({Id})";
}

/// <summary>
/// Custom ID type for testing
/// </summary>
/// <param name="Value"></param>
public record EntitySetTests_CustomId(int Value);

/// <summary>
/// Dummy entity with CUSTOM ID type for testing
/// </summary>
public sealed class EntitySetTests_CustomIdEntity : Entity<EntitySetTests_CustomId>
{
    public string Label { get; }
    public EntitySetTests_CustomIdEntity(EntitySetTests_CustomId id, string label) : base(id) => Label = label;
    public override string ToString() => $"{Label} ({Id.Value})";
}

/// <summary>
/// Dummy entity with INT identity for mixed tests
/// </summary>
public sealed class EntitySetTests_MixedEntity : Entity<int>
{
    public string Label { get; }
    public EntitySetTests_MixedEntity(int id, string label) : base(id) => Label = label;
    public override string ToString() => $"{Label} ({Id})";
}

/// <summary>
/// Validator that rejects entities whose string representation contains "X"
/// </summary>
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


/// <summary>
/// Validator that rejects entities whose Name matches a specific value
/// </summary>
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
    /// <summary>
    /// Tests that Add should successfully add a new entity to the set.
    /// </summary>
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

    /// <summary>
    /// Tests that Add should reject duplicate entities with the same ID.
    /// </summary>
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

    /// <summary>
    /// Tests that Add should respect the provided validator and reject invalid entities.
    /// </summary>
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

    /// <summary>
    /// Tests that FindById should return the correct entity when it exists.
    /// </summary>
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

    /// <summary>
    /// Tests that FindById should return an error when the entity is not found.
    /// </summary>
    [Fact]
    public void FindById_ShouldReturnError_WhenNotFound()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();

        var result = set.FindById(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Item not found.", result.GetErrorMessage());
    }

    /// <summary>
    /// Tests that ContainsId should return true when an entity with the given ID exists in the set.
    /// </summary>
    [Fact]
    public void ContainsId_ShouldReturnTrue_WhenExists()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();
        var id = Guid.NewGuid();
        set.Add(new EntitySetTests_GuidEntity(id, "Alice"));

        Assert.True(set.ContainsId(id));
    }

    /// <summary>
    /// Tests that ContainsId should return false when no entity with the given ID exists in the set.
    /// </summary>
    [Fact]
    public void ContainsId_ShouldReturnFalse_WhenNotExists()
    {
        var set = new EntitySet<EntitySetTests_GuidEntity, Guid>();

        Assert.False(set.ContainsId(Guid.NewGuid()));
    }

    /// <summary>
    /// Tests that Remove should successfully remove an existing entity from the set.
    /// </summary>
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

    /// <summary>
    /// Tests that CreateValidated should return a successful result when all entities pass validation.
    /// </summary>
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

    /// <summary>
    /// Tests that CreateValidated should return an error when the validator rejects an entity.
    /// </summary>
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

    /// <summary>
    /// Tests that an EntitySet with INT identity works correctly.
    /// </summary>
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

    /// <summary>
    /// Tests that adding an entity with a duplicate INT ID is rejected.
    /// </summary>
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

    /// <summary>
    /// Tests that an EntitySet with STRING identity can check for existence by ID.
    /// </summary>
    [Fact]
    public void StringEntitySet_ShouldContainId()
    {
        var set = new EntitySet<EntitySetTests_StringEntity, string>();
        var e = new EntitySetTests_StringEntity("abc", "Test");
        set.Add(e);

        Assert.True(set.ContainsId("abc"));
        Assert.False(set.ContainsId("zzz"));
    }

    /// <summary>
    /// Tests that CreateValidated should apply the validator and reject invalid entities.
    /// </summary>
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

    /// <summary>
    /// Tests that an EntitySet with CUSTOM ID type works correctly.
    /// </summary>
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

    /// <summary>
    /// Tests that adding an entity with a duplicate CUSTOM ID is rejected based on custom equality.
    /// </summary>
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

    /// <summary>
    /// Tests that ReplaceAll should clear existing entities and add new ones.
    /// </summary>
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

    /// <summary>
    /// Tests that Add, Remove, and FindById operations remain consistent across multiple operations.
    /// </summary>
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

    /// <summary>
    /// Tests that after ReplaceAll, adding an entity with a duplicate ID is rejected.
    /// </summary>
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

    /// <summary>
    /// Tests that AddRange should add all valid entities or stop on the first validator failure.
    /// </summary>
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

    /// <summary>
    /// Tests that after ReplaceAll, ContainsId and FindById work correctly across multiple operations.
    /// </summary>
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

