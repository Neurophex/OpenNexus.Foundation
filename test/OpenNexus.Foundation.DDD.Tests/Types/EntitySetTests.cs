using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Types;
using OpenNexus.Foundation.DDD.Validators;
using OpenNexus.Foundation.Utils;

namespace OpenNexus.Foundation.DDD.Tests.Types;

// Dummy entity for testing
public sealed class EntitySetTests_DummyEntity : Entity<Guid>
{
    public string Name { get; }

    public EntitySetTests_DummyEntity(Guid id, string name) : base(id)
    {
        Name = name;
    }

    public override string ToString() => $"{Name} ({Id})";
}

// Validator that rejects entities with a given name
public class EntitySetTests_RejectNameValidator : ISetValidator<EntitySetTests_DummyEntity>
{
    private readonly string _reject;
    public EntitySetTests_RejectNameValidator(string reject) => _reject = reject;

    public Result Validate(EntitySetTests_DummyEntity item, IReadOnlySet<EntitySetTests_DummyEntity> existingItems)
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
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();
        var entity = new EntitySetTests_DummyEntity(Guid.NewGuid(), "Alice");

        var result = set.Add(entity);

        Assert.True(result.IsSuccess);
        Assert.Contains(entity, set.AsEnumerable());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldRejectDuplicateEntityById()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();
        var id = Guid.NewGuid();
        var e1 = new EntitySetTests_DummyEntity(id, "First");
        var e2 = new EntitySetTests_DummyEntity(id, "Second");

        set.Add(e1);
        var result = set.Add(e2);

        Assert.False(result.IsSuccess);
        Assert.Equal("Item already exists in the collection.", result.GetErrorMessage());
        Assert.Single(set);
    }

    [Fact]
    public void Add_ShouldRespectValidator()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>(new EntitySetTests_RejectNameValidator("Forbidden"));
        var entity = new EntitySetTests_DummyEntity(Guid.NewGuid(), "Forbidden");

        var result = set.Add(entity);

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity with name 'Forbidden' is not allowed.", result.GetErrorMessage());
        Assert.Empty(set.AsEnumerable());
    }

    [Fact]
    public void FindById_ShouldReturnEntity_WhenExists()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();
        var id = Guid.NewGuid();
        var entity = new EntitySetTests_DummyEntity(id, "Alice");
        set.Add(entity);

        var result = set.FindById(id);

        Assert.True(result.IsSuccess);
        Assert.Equal(entity, result.Value);
    }

    [Fact]
    public void FindById_ShouldReturnError_WhenNotFound()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();

        var result = set.FindById(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Item not found.", result.GetErrorMessage());
    }

    [Fact]
    public void ContainsId_ShouldReturnTrue_WhenExists()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();
        var id = Guid.NewGuid();
        set.Add(new EntitySetTests_DummyEntity(id, "Alice"));

        Assert.True(set.ContainsId(id));
    }

    [Fact]
    public void ContainsId_ShouldReturnFalse_WhenNotExists()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();

        Assert.False(set.ContainsId(Guid.NewGuid()));
    }

    [Fact]
    public void ReplaceAll_ShouldReplaceEntities()
    {
        var set = new EntitySet<EntitySetTests_DummyEntity, Guid>();
        set.Add(new EntitySetTests_DummyEntity(Guid.NewGuid(), "Old"));

        var newEntities = new[]
        {
                new EntitySetTests_DummyEntity(Guid.NewGuid(), "New1"),
                new EntitySetTests_DummyEntity(Guid.NewGuid(), "New2")
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
                new EntitySetTests_DummyEntity(Guid.NewGuid(), "Alice"),
                new EntitySetTests_DummyEntity(Guid.NewGuid(), "Bob")
            };

        var result = EntitySet<EntitySetTests_DummyEntity, Guid>.CreateValidated(items);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public void CreateValidated_ShouldFail_WhenValidatorRejects()
    {
        var items = new[]
        {
                new EntitySetTests_DummyEntity(Guid.NewGuid(), "Alice"),
                new EntitySetTests_DummyEntity(Guid.NewGuid(), "Forbidden")
            };

        var result = EntitySet<EntitySetTests_DummyEntity, Guid>.CreateValidated(items, new EntitySetTests_RejectNameValidator("Forbidden"));

        Assert.False(result.IsSuccess);
        Assert.Equal("Entity with name 'Forbidden' is not allowed.", result.GetErrorMessage());
    }
}