using System.Diagnostics;
using Moq;
using Playlister.Controllers;
using Playlister.Utilities;

namespace Playlister.Tests.Controllers;

public class BaseControllerTests
{
    [Fact]
    public void BaseController_ShouldSetName_WhenCalled()
    {
        // ARRANGE

        // ACT
        TestController sut = new(new Mock<IAccessTokenUtility>().Object);

        // ASSERT
        sut.Should().NotBeNull();
        sut.Name.Should().Be("Test");
    }

    [Fact]
    public void BaseController_ShouldThrow_WhenIAccessTokenUtilityIsNull()
    {
        Action act = () => _ = new TestController(null);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task RunInTimer_ShouldReturnDataAndTimeElapses_WhenRunningAnAsyncFunctionThatReturnsData()
    {
        // Arrange
        TestController sut = new(new Mock<IAccessTokenUtility>().Object);
        const int expected = 42;

        // Act
        (int data, TimeSpan elapsed) = await TestController.RunFunctionInTimer((Func<Task<int>>)Func);

        // Assert
        data.Should().Be(expected);
        elapsed.TotalMilliseconds.Should().BeInRange(500, 510); // seems a wide enough range, but may have to increase

        return;

        async Task<int> Func()
        {
            await Task.Delay(500);

            return 42;
        }
    }

    [Fact]
    public async Task RunInTimer_ShouldReturnTimeElapse_WhenRunWithAnAsyncFunction()
    {
        // Arrange
        TestController sut = new(new Mock<IAccessTokenUtility>().Object);
        const int expected = 42;

        // Act
        TimeSpan elapsed = await TestController.RunFunction(Func);

        // Assert
        elapsed.TotalMilliseconds.Should().BeInRange(500, 510); // seems a wide enough range, but may have to increase

        return;

        async Task Func()
        {
            await Task.Delay(500);
        }
    }

    private class TestController : BaseController
    {
        public TestController(IAccessTokenUtility tokenUtility) : base(tokenUtility) { }

        public static async Task<(T data, TimeSpan elapsed)> RunFunctionInTimer<T>(Func<Task<T>> func)
        {
            return await RunInTimer(func);
        }

        public static async Task<TimeSpan> RunFunction(Func<Task> func)
        {
            return await RunInTimer(func);
        }
    }
}
