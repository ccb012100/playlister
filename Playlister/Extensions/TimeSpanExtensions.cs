using System;
using System.Collections.Generic;
using System.Diagnostics;
using Playlister.CQRS.Commands;

namespace Playlister.Extensions
{
    public static class TimeSpanExtensions
    {

        /// <summary>
        /// Format <paramref name="elapsed"/> as a human-friendly number with units.
        /// </summary>
        /// <param name="elapsed"></param>
        /// <returns></returns>
        public static string ToLogString(this TimeSpan elapsed)
        {
            Debug.Assert(elapsed.TotalNanoseconds >= 0);

            if (elapsed.TotalNanoseconds == 0)
            {
                return elapsed.ToString();
            }

            List<string> times = new();

            uint defaultPrecision = 4;

            if (elapsed.TotalMinutes >= 1) // Xmin Ys Z.ZZZZms
            {
                int minutes = (int)(elapsed.TotalSeconds / 60);
                double seconds = elapsed.TotalSeconds % 60;

                times.Add($"{minutes}min");
                times.Add(ToSecondsAndMilliseconds(seconds, defaultPrecision));
            }
            else if (elapsed.TotalSeconds >= 1) // Ys Z.ZZZZms
            {
                times.Add(ToSecondsAndMilliseconds(elapsed.TotalSeconds, defaultPrecision));
            }
            else // Z.ZZZZms
            {
                times.Add($"{elapsed.TotalMilliseconds.Format(defaultPrecision)}ms");
            }

            return string.Join(" ", times);
        }

        /// <summary>
        /// Format <paramref name="seconds"/> as seconds and milliseconds.
        /// </summary>
        /// <param name="seconds">A span of time in seconds</param>
        /// <param name="precision">The number of significant figures to use for the milliseconds portion</param>
        /// <returns>A string in the format "{seconds}s {milliseconds}ms</returns>
        private static string ToSecondsAndMilliseconds(double seconds, uint precision)
        {
            Debug.Assert(seconds > 0);

            int s = (int)(seconds / 1);
            double ms = seconds % 1 * 1000;

            return $"{s}s {ms.Format(precision)}ms";
        }

        /// <summary>
        /// Format <paramref name="d"/> as a string with up <paramref name="precision"/> number of signifcant digits
        /// (trailing zeroes will be removed)
        /// </summary>
        /// <param name="d"></param>
        /// <param name="units"></param>
        /// <param name="precision">The number of significant digits</param>
        /// <returns></returns>
        private static string Format(this double d, uint precision)
        {
            Debug.Assert(d >= 0);

            /*
             * Get the remainder portion of the duration;
             * if the remainder is 0, set precision to 0;
             * otherwise, set the precision to the supplied value.
             */
            uint sigfigs = d % 1 == 0 ? 0 : precision;
            string dstr = d.ToString($"N{sigfigs}");

            return sigfigs == 0 ? dstr : dstr.TrimEnd('0');
        }
    }
}
