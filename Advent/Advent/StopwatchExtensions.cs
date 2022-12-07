using System.Diagnostics;

namespace Advent
{
    internal static class StopwatchExtensions
    {
        public static double GetMilliseconds(this Stopwatch stopwatch)
        {
            return 1000 * ((double)stopwatch.ElapsedTicks / Stopwatch.Frequency);
        }
    }
}
