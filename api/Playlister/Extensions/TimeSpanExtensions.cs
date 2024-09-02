namespace Playlister.Extensions;

public static class TimeSpanExtensions
{
    /// <summary>
    ///     Format <paramref name="elapsed" /> based on the best units to use
    /// </summary>
    /// <param name="elapsed"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <returns>A number with units (e.g. <c>"1.23s"</c>,<c>123.2ms</c>, etc.</returns>
    public static string ToDisplayString( this TimeSpan elapsed )
    {
        // <https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-timespan-format-strings>
        if (elapsed.TotalNanoseconds == 0)
        {
            return "under 1ns";
        }

        return elapsed.TotalSeconds switch
        {
            // elapsed >= 1 day
            >= 86400 => $"{elapsed.Hours}h{elapsed.Minutes}min{elapsed.Seconds}s",
            // elapsed >= 1 hour
            >= 3600 => $"{elapsed.Hours}h{elapsed.Minutes}min{elapsed.Seconds}s",
            // elapsed > 2 minutes
            > 120 => $"{elapsed.Minutes}min{elapsed.Seconds}.{elapsed.Milliseconds}s",
            // 2 minutes > elapsed >= 1 minute
            >= 60 => $"{elapsed.TotalSeconds:N2}s",
            //  1 minute > elapsed >= 1 second
            >= 1 => $"{elapsed.TotalSeconds:N4}s",
            // elapsed < 1 second
            _ => elapsed.TotalMicroseconds switch
            {
                // elapsed >= 1 millisecond
                >= 1000 => $"{elapsed.TotalMilliseconds:N2}ms",
                // elapsed >= 1 microsecond
                >= 1 => $"{elapsed.TotalMicroseconds:N0}μs",
                // elapsed < 1 microsecond
                _ => $"{elapsed.TotalNanoseconds:N0}ns"
            }
        };
    }
}
