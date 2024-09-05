using Playlister.Extensions;

namespace Playlister.Tests.Extensions;

public class TimeSpanExtensionsTests
{
    [InlineData( 0, 0, 0, 0, 0, 0, "under 1ns" )]
    [InlineData( 0, 0, 0, 0, 0, 278, "278μs" )]
    [InlineData( 0, 0, 0, 0, 1, 0, "1.00ms" )]
    [InlineData( 0, 0, 0, 0, 10, 0, "10.00ms" )]
    [InlineData( 0, 0, 0, 0, 100, 1234, "101.23ms" )]
    [InlineData( 0, 0, 0, 0, 5, 200, "5.20ms" )]
    [InlineData( 0, 0, 0, 1, 0, 0, "1.000s" )]
    [InlineData( 0, 0, 0, 1, 1, 0, "1.001s" )]
    [InlineData( 0, 0, 0, 9, 120, 370, "9.120s" )]
    [InlineData( 0, 0, 0, 9, 0, 370, "9.000s" )]
    [InlineData( 0, 0, 0, 10, 0, 370, "10.00s" )]
    [InlineData( 0, 0, 1, 0, 0, 370, "60.00s" )]
    [InlineData( 0, 0, 1, 1, 0, 0, "61.00s" )]
    [InlineData( 0, 0, 1, 1, 0, 371, "61.00s" )]
    [InlineData( 0, 0, 2, 0, 0, 371, "2min0.0s" )]
    [InlineData( 0, 0, 2, 0, 3, 371, "2min0.3s" )]
    [InlineData( 0, 0, 2, 0, 30, 371, "2min0.30s" )]
    [InlineData( 0, 0, 2, 24, 30, 371, "2min24.30s" )]
    [InlineData( 0, 0, 2, 0, 300, 371, "2min0.300s" )]
    [InlineData( 0, 1, 19, 1, 34, 12, "1h19min1s" )]
    [Theory]
    public void ToLogString_ShouldReturnFormattedString( int d, int h, int min, int s, int ms, int μs, string expectedResult )
    {
        new TimeSpan( d, h, min, s, ms, μs ).ToDisplayString()
            .Should()
            .Be( expectedResult );
    }
}
