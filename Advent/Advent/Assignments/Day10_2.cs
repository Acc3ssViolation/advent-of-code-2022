using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day10_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var length = input.Count;
            var x = 1;
            var cycle = 0;

            const int width = 40;
            const int height = 6;
            const int cycleCount = width * height;
            var screen = new char[width * height];

            for (int i = 0; i < length; i++)
            {
                var op = input[i];
                var dx = 0;
                var dc = 1;
                if (op[0] != 'n')
                {
                    // x incrementing
                    dc++;
                    dx = int.Parse(op.AsSpan(5));
                }

                for (int k = cycle; k < cycle + dc; k++)
                {
                    char pixel = '.';
                    var displayX = k % width;
                    if (displayX >= (x - 1) && displayX <= (x + 1))
                    {
                        pixel = '#';
                    }
                    screen[k] = pixel;
                }

                x += dx;
                cycle += dc;
                if (cycle >= cycleCount)
                    break;
            }

            for (int y = 0; y < height; y++)
            {
                Console.Out.Write(screen.AsSpan(y * width, width));
                Console.Out.Write('\n');
            }

            return "";
        }

        
    }
}
