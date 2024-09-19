using Playlister.Controllers;
using Playlister.Utilities;

namespace Playlister.Tests.Controllers;

public class BaseApiControllerTests {
    [Fact]
    public void BaseController_ShouldThrow_WhenIAccessTokenUtilityIsNull( ) {
        Action act = ( ) => _ = new TestApiController( null! );

        act.Should( ).ThrowExactly<ArgumentNullException>( );
    }

    [Fact]
    public async Task RunInTimer_ShouldReturnDataAndTimeElapsed_WhenRunningAnAsyncFunctionThatReturnsData( ) {
        // ARRANGE
        const int expected = 42;

        // ACT
        (int data, TimeSpan elapsed) = await TestApiController.RunFunctionInTimer(
                async ( ) => {
                    await Task.Delay( 500 );

                    return expected;
                }
            )
            .ConfigureAwait( true );

        // ASSERT
        data.Should( ).Be( expected );
        elapsed.TotalMilliseconds.Should( ).BeInRange( 490 , 510 ); // CICD sometimes runs _under_ 500ms
    }

    [Fact]
    public async Task RunInTimer_ShouldReturnTimeElapsed_WhenAsyncFunctionIsNull( ) {
        // ARRANGE
        // ACT
        TimeSpan elapsed = await TestApiController.RunFunction( async ( ) => await Task.Delay( 500 ) );

        // ASSERT
        elapsed.TotalMilliseconds.Should( ).BeInRange( 490 , 510 ); // CICD sometimes runs _under_ 500ms
    }

    [Fact]
    public async Task RunInTimer_ShouldThrow_WhenFuncIsNull( ) {
        // ARRANGE
        Func<Task> f = async ( ) => await TestApiController.RunFunction( null );

        // ACT

        // ASSERT
        await f.Should( ).ThrowExactlyAsync<ArgumentNullException>( );
    }

    [Fact]
    public async Task RunInTimer_ShouldThrow_WhenAsyncFuncWithReturnTypeIsNull( ) {
        // ARRANGE
        Func<Task> f = async ( ) => await TestApiController.RunFunctionInTimer<int>( null! );

        // ASSERT
        await f.Should( ).ThrowExactlyAsync<ArgumentNullException>( );
    }

    [Fact]
    public void CookieToken_ShouldReturnToken( ) {
        // ARRANGE
        Mock<IAccessTokenUtility> tokenUtility = new( MockBehavior.Strict );
        tokenUtility.Setup( t => t.GetTokenFromUserCookie( ) ).Returns( "foo_bar_baz" );

        TestApiController sut = new( tokenUtility.Object );

        // ACT
        string result = sut.Token;

        // ASSERT
        result.Should( ).Be( "foo_bar_baz" );
    }

    private class TestApiController( IAccessTokenUtility tokenUtility ) : BaseApiController( tokenUtility ) {
        public string Token => CookieToken;

        public static async Task<(T data, TimeSpan elapsed)> RunFunctionInTimer<T>( Func<Task<T>> func ) {
            return await RunInTimer( func );
        }

        public static async Task<TimeSpan> RunFunction( Func<Task>? func ) {
            return await RunInTimer( func );
        }
    }
}
