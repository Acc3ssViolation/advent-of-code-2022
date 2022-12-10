using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day10_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var length = input.Count;
            const int targetCycleIncrement = 40;
            var x = 1;
            var targetCycle = 20;
            var signalSum = 0;
            var cycle = 1;

            for (int i = 0; i < length; i++)
            {
                int nextCycle = cycle + 1;
                var op = input[i];
                var dx = 0;
                if (op[0] != 'n')
                {
                    // x incrementing
                    nextCycle++;
                    dx = int.Parse(op.AsSpan(5));
                }

                if (targetCycle >= cycle && targetCycle < nextCycle )
                {
                    signalSum += targetCycle * x;
                    if (targetCycle == 220)
                        break;

                    targetCycle += targetCycleIncrement;
                }

                x += dx;
                cycle = nextCycle;
            }

            return signalSum.ToString();
        }

        
    }
}
