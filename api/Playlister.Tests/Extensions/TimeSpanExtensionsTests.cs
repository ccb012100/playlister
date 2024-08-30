using Playlister.Extensions;

namespace Playlister.Tests.Extensions;

[TestFixture]
public class TimeSpanExtensionsTests
{
    [TestCase(0, 0, 0, 0, 0, 0, "under 1ns")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 0, 0, 5, 200, "5.20ms")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 0, 0, 0, 278, "278μs")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 0, 0, 1, 0, "1.00ms")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 0, 0, 10, 0, "10.00ms")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 0, 0, 100, 1234, "101.23ms")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 0, 1, 0, 0, "1.0000s")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 1, 0, 0, 370, "60.00s")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 1, 1, 0, 371, "61.00s")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 0, 1, 1, 0, 0, "61.00s")]
    //        d  h  m   s  ms  μs
    [TestCase(0, 1, 19, 1, 34, 12, "1h19min1s")]
    //        d  h  m   s  ms  μs
    public void ToLogString_ShouldReturnFormattedString(int d, int h, int min, int s, int ms, int μs, string expectedResult)
    {
        // arrange
        TimeSpan elapsed = new(d, h, min, s, ms, μs);
        // act
        // assert
        elapsed.ToDisplayString().Should().Be(expectedResult);
    }
}
