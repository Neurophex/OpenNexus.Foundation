namespace OpenNexus.Foundation.DataBroker.Tests.Core;

public class DataQueryOptionsTests
{
    [Fact]
    public void Skip_ShouldBeClampedToZero_WhenNegative()
    {
        // Arrange
        var options = new DataQueryOptions(Skip: -10, Take: 10);

        // Act
        var result = options.Skip;

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void Take_ShouldBeClampedToZero_WhenNegative()
    {
        // Arrange
        var options = new DataQueryOptions(Skip: 5, Take: -50);

        // Act
        var result = options.Take;

        // Assert
        Assert.Equal(0, result);
    }

    [Fact]
    public void SkipAndTake_ShouldRemainUnchanged_WhenValid()
    {
        var options = new DataQueryOptions(Skip: 10, Take: 25);

        Assert.Equal(10, options.Skip);
        Assert.Equal(25, options.Take);
    }

    [Fact]
    public void Defaults_ShouldBeApplied_WhenNoValuesProvided()
    {
        var options = new DataQueryOptions();

        Assert.Equal(0, options.Skip);
        Assert.Equal(50, options.Take);
        Assert.True(options.Ascending);
        Assert.Null(options.OrderBy);
        Assert.Null(options.Filter);
    }

    [Fact]
    public void Properties_ShouldBeAssignable()
    {
        var options = new DataQueryOptions
        {
            Skip = 100,
            Take = 200,
            OrderBy = "Name",
            Ascending = false,
            Filter = "Category eq 'Apps'"
        };

        Assert.Equal(100, options.Skip);
        Assert.Equal(200, options.Take);
        Assert.Equal("Name", options.OrderBy);
        Assert.False(options.Ascending);
        Assert.Equal("Category eq 'Apps'", options.Filter);
    }

    [Fact]
    public void Record_ShouldSupportValueEquality()
    {
        var o1 = new DataQueryOptions(Skip: 5, Take: 10, OrderBy: "Name");
        var o2 = new DataQueryOptions(Skip: 5, Take: 10, OrderBy: "Name");

        Assert.Equal(o1, o2);
        Assert.True(o1.Equals(o2));
        Assert.Equal(o1.GetHashCode(), o2.GetHashCode());
    }
}