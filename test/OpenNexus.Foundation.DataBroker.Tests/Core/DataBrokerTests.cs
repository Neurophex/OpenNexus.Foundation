using Moq;
using OpenNexus.Foundation.Policy.Abstractions;
using OpenNexus.Foundation.Policy.Access;

namespace OpenNexus.Foundation.DataBroker.Tests;

public class DataBrokerTests
{
    private readonly IUserContext _user = Mock.Of<IUserContext>(u => u.UserId == "U1");

    private List<TestSource> CreateSampleData() => new()
    {
        new TestSource { Id = 1, Name = "Alpha" },
        new TestSource { Id = 2, Name = "Bravo" },
        new TestSource { Id = 3, Name = "Charlie" },
    };

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProjection_WhenAccessAllowed()
    {
        // Arrange
        var policy = Mock.Of<IAccessPolicy<TestSource>>(p => p.CanViewResource(_user, It.IsAny<TestSource>())
            == AccessDecision.Allow());
        var broker = new TestDataBroker(CreateSampleData(), policy);

        // Act
        var result = await broker.GetByIdAsync(1, _user);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("Alpha", result.Value!.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldFail_WhenAccessDenied()
    {
        var policy = Mock.Of<IAccessPolicy<TestSource>>(p => p.CanViewResource(_user, It.IsAny<TestSource>())
            == AccessDecision.Deny("Denied"));
        var broker = new TestDataBroker(CreateSampleData(), policy);

        var result = await broker.GetByIdAsync(1, _user);

        Assert.False(result.IsSuccess);
        Assert.Equal("Denied", result.GetErrorMessage());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnFailure_WhenNotFound()
    {
        var policy = Mock.Of<IAccessPolicy<TestSource>>(p => p.CanViewResource(_user, It.IsAny<TestSource>())
            == AccessDecision.Allow());
        var broker = new TestDataBroker(CreateSampleData(), policy);

        var result = await broker.GetByIdAsync(999, _user);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not found", result.GetErrorMessage());
    }

    [Fact]
    public async Task GetAllAsync_ShouldFilterResults_ByAccessPolicy()
    {
        var policyMock = new Mock<IAccessPolicy<TestSource>>();
        policyMock.Setup(p => p.CanViewResource(_user, It.Is<TestSource>(s => s.Id == 1)))
            .Returns(AccessDecision.Allow());
        policyMock.Setup(p => p.CanViewResource(_user, It.Is<TestSource>(s => s.Id != 1)))
            .Returns(AccessDecision.Deny("Hidden"));

        var broker = new TestDataBroker(CreateSampleData(), policyMock.Object);

        var result = await broker.GetAllAsync(_user, null);

        Assert.True(result.IsSuccess);
        var projections = result.Value!.ToList();
        Assert.Single(projections);
        Assert.Equal("Alpha", projections.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldApplySkipAndTake()
    {
        var policy = Mock.Of<IAccessPolicy<TestSource>>(p => p.CanViewResource(_user, It.IsAny<TestSource>())
            == AccessDecision.Allow());
        var broker = new TestDataBroker(CreateSampleData(), policy);

        var options = new TestQueryOptions { Skip = 1, Take = 1 };

        var result = await broker.GetAllAsync(_user, options);

        Assert.True(result.IsSuccess);
        var projections = result.Value!.ToList();
        Assert.Single(projections);
        Assert.Equal("Bravo", projections.First().Name);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmpty_WhenAllDenied()
    {
        var policy = Mock.Of<IAccessPolicy<TestSource>>(p => p.CanViewResource(_user, It.IsAny<TestSource>())
            == AccessDecision.Deny("Forbidden"));
        var broker = new TestDataBroker(CreateSampleData(), policy);

        var result = await broker.GetAllAsync(_user, null);

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value!);
    }
}
