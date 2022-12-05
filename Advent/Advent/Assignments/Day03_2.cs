using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day03_2 : IAssignment
    {
        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            var sum = 0;

            ulong bag0 = 0;
            ulong bag1 = 0;
            ulong bag2 = 0;

            for (int i = 0; i < input.Count; i++)
            {
                var phase = i % 3;
                var backpack = input[i];
                foreach (var item in backpack)
                {
                    var shift = GetPriority(item) - 1;
                    if (phase == 0)
                        bag0 |= 1ul << shift;
                    else if (phase == 1)
                        bag1 |= 1ul << shift;
                    else
                        bag2 |= 1ul << shift;
                }

                if (phase == 2)
                {
                    var sharedBag = bag0 & bag1 & bag2;
                    var prio = 64 - BitOperations.LeadingZeroCount(sharedBag);

                    // Logger.DebugLine($"{bag0:X8}&{bag1:X8}&{bag2:X8}={sharedBag:X8} --> {prio}");

                    sum += prio;

                    bag0 = 0;
                    bag1 = 0;
                    bag2 = 0;
                }
            }

            return Task.FromResult(sum.ToString());
        }

        private static int GetPriority(char item)
        {
            if (item >= 'a' && item <= 'z')
                return item - 'a' + 1;
            else if (item >= 'A' && item <= 'Z')
                return item - 'A' + 27;
            throw new ArgumentException(null, nameof(item));
        }
    }
}
