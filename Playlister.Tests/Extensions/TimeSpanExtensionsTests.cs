using Playlister.Extensions;

namespace Playlister.Tests.Extensions
{
    public class TimeSpanExtensionsTests
    {
        [Theory]
        [InlineData(0, 0, 0, 0, "00:00:00")]
        [InlineData(0, 0, 5, 200, "5.2ms")]
        [InlineData(0, 0, 0, 278, "0.278ms")]
        [InlineData(0, 0, 1, 0, "1ms")]
        [InlineData(0, 0, 10, 0, "10ms")]
        [InlineData(0, 0, 100, 1234, "101.234ms")]
        [InlineData(0, 1, 0, 0, "1s 0ms")]
        [InlineData(1, 0, 0, 370, "1min 0s 0.37ms")]
        [InlineData(1, 1, 0, 371, "1min 1s 0.371ms")]
        [InlineData(1, 1, 0, 0, "1min 1s 0ms")]
        [InlineData(999, 1, 34, 12, "999min 1s 34.012ms")]
        public void ToLogString_ShouldReturnFormattedString(int min, int s, int ms, int us, string expectedResult)
        {
            // arrange
            TimeSpan elapsed = new(0, 0, min, s, ms, us);
            // act
            // assert
            elapsed.ToLogString().Should().Be(expectedResult);
        }
    }
}
