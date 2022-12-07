using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day06_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            int counter = 0;
            var stream = input[0];

            for (int i = 3; i < stream.Length; i++)
            {
                uint flags = 0;
                flags |= 1u << (stream[i - 3] - 'a');
                flags |= 1u << (stream[i - 2] - 'a');
                flags |= 1u << (stream[i - 1] - 'a');
                flags |= 1u << (stream[i - 0] - 'a');

                if (BitOperations.PopCount(flags) == 4)
                {
                    counter = i + 1;
                    break;
                }
            }

            return counter.ToString();
        }
    }
}
