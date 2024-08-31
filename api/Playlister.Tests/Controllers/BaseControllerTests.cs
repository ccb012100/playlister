using Moq;
using Playlister.Controllers;
using Playlister.Utilities;

namespace Playlister.Tests.Controllers;

public class BaseControllerTests
{
    [Fact]
    public void BaseController_ShouldSetName()
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
        Action act = () => _ = new TestController(null!);

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public async Task RunInTimer_ShouldReturnDataAndTimeElapsed_WhenRunningAnAsyncFunctionThatReturnsData()
    {
        // ARRANGE
        const int expected = 42;

        // ACT
        (int data, TimeSpan elapsed) = await TestController.RunFunctionInTimer(
                async () =>
                {
                    await Task.Delay(500);

                    return expected;
                }
            )
            .ConfigureAwait(true);

        // ASSERT
        data.Should().Be(expected);
        elapsed.TotalMilliseconds.Should().BeInRange(490, 510); // CICD sometimes runs _under_ 500ms
    }

    [Fact]
    public async Task RunInTimer_ShouldReturnTimeElapsed_WhenAsyncFunctionIsNull()
    {
        // ARRANGE
        // ACT
        TimeSpan elapsed = await TestController.RunFunction(async () => await Task.Delay(500));

        // ASSERT
        elapsed.TotalMilliseconds.Should().BeInRange(490, 510); // CICD sometimes runs _under_ 500ms
    }

    [Fact]
    public async Task RunInTimer_ShouldThrow_WhenFuncIsNull()
    {
        // ARRANGE
        Func<Task> f = async () => await TestController.RunFunction(null);

        // ACT

        // ASSERT
        await f.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RunInTimer_ShouldThrow_WhenAsyncFuncWithReturnTypeIsNull()
    {
        // ARRANGE
        Func<Task> f = async () => await TestController.RunFunctionInTimer<int>(null!);

        // ASSERT
        await f.Should().ThrowExactlyAsync<ArgumentNullException>();
    }

    [Fact]
    public void CookieToken_ShouldReturnToken()
    {
        // ARRANGE
        Mock<IAccessTokenUtility> tokenUtility = new(MockBehavior.Strict);
        tokenUtility.Setup(t => t.GetTokenFromUserCookie()).Returns("foo_bar_baz");

        TestController sut = new(tokenUtility.Object);

        // ACT
        string result = sut.Token;

        // ASSERT
        result.Should().Be("foo_bar_baz");
    }

    private class TestController : BaseController
    {
        public TestController(IAccessTokenUtility tokenUtility) : base(tokenUtility) { }

        public string Token => CookieToken;

        public static async Task<(T data, TimeSpan elapsed)> RunFunctionInTimer<T>(Func<Task<T>> func)
        {
            return await RunInTimer(func);
        }

        public static async Task<TimeSpan> RunFunction(Func<Task>? func)
        {
            return await RunInTimer(func);
        }
    }
}
