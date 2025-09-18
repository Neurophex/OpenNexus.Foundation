using OpenNexus.Foundation.DDD.Core;
using OpenNexus.Foundation.DDD.Events;

namespace OpenNexus.Foundation.DDD.Tests.Core;

/// <summary>
/// Represents a test domain event for unit testing purposes.
/// </summary>
public sealed class TestDomainEvent : IDomainEvent
{
    public string Name { get; init; }

    public TestDomainEvent(string name)
    {
        Name = name;
    }

    public DateTime OccuredOn { get; init; }

    public string? CorrelationId { get; init;}

    public Guid EventId { get; init;}
}

/// <summary>
/// Represents a test entity for unit testing purposes.
/// </summary>
public sealed class TestEntity : Entity<Guid>
{
    public string Name { get; set; }

    public TestEntity(Guid id, string name = "TestEntity") : base(id)
    {
        Name = name;
    }
}

/// <summary>
/// Unit tests for the Entity class.
/// </summary>
public class EntityTests
{
    /// <summary>
    /// Tests the constructor of the TestEntity class to ensure it initializes the Id property correctly.
    /// </summary>
    [Fact]
    public void Constructor_ShouldInitializeId_WhenCalledWithValidId()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id, "Test Entity");

        // Act
        var isEqual = entity.Id.Equals(id);

        // Assert
        Assert.True(isEqual);
    }

    /// <summary>
    /// Tests the ToString method of the TestEntity class to ensure it returns the correct format.
    /// </summary>
    [Fact]
    public void ToString_ShouldReturnCorrectFormat_WhenCalled()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id, "Test Entity");

        // Act
        var result = entity.ToString();

        // Assert
        Assert.Equal($"TestEntity [Id: {id}]", result);
    }

    /// <summary>
    /// Tests the Equals method of the TestEntity class to ensure it returns true when comparing entities with the same Id.
    /// </summary>
    [Fact]
    public void Equals_Should_Return_True_For_Same_Id()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id, "Entity 1");
        var entity2 = new TestEntity(id, "Entity 2");

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        Assert.True(areEqual);
    }

    /// <summary>
    /// Tests the Equals method of the TestEntity class to ensure it returns false when comparing entities with different Ids.
    /// </summary>
    [Fact]
    public void Equals_Should_Return_False_Different_Ids()
    {
        // Arrange
        var entity1 = new TestEntity(Guid.NewGuid(), "Entity 1");
        var entity2 = new TestEntity(Guid.NewGuid(), "Entity 2");

        // Act
        var areEqual = entity1.Equals(entity2);

        // Assert
        Assert.False(areEqual);
    }

    /// <summary>
    /// Tests the GetHashCode method of the TestEntity class to ensure it returns the same hash code for entities with the same Id.
    /// </summary>
    [Fact]
    public void GetHashCode_Should_Be_Based_On_Id()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity1 = new TestEntity(id);
        var entity2 = new TestEntity(id);

        // Act
        var areEqual = entity1.GetHashCode() == entity2.GetHashCode();

        // Assert
        Assert.True(areEqual);
    }

    /// <summary>
    /// Tests the ToString method of the TestEntity class to ensure it returns the correct format.
    /// </summary>
    [Fact]
    public void ToString_Should_Return_Correct_Format()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new TestEntity(id);

        // Act
        var result = entity.ToString();

        // Assert
        Assert.Contains("TestEntity", result);
        Assert.Contains(id.ToString(), result);
    }
}