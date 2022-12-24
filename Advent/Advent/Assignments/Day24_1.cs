namespace Advent.Assignments
{
    internal class Day24_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            // 120x24
            // 5x5

            // We exclude the edges. The expedition can always move to the square below it on turn 1 (at least in the example and my input)
            var width = input[0].Length - 2;
            var height = input.Count - 2;

            // We only need 4 masks containing the blizzards in each direction
            // Directions: 0 = N, 1 = E, 2 = S, 3 = W
            var blizzardStride = width * height;
            var blizzards = new bool[blizzardStride * 4];

            for (int y = 0; y < height; y++)
            {
                var line = input[y + 1];
                for (int x = 0; x < width; x++)
                {
                    var chr = line[x + 1];
                    var blizzardIndex = chr switch
                    {
                        '<' => blizzardStride * 3,
                        'v' => blizzardStride * 2,
                        '>' => blizzardStride * 1,
                        '^' => 0,
                        _ => -1,
                    };
                    if (blizzardIndex < 0)
                        continue;
                    blizzards[blizzardIndex + x + y * width] = true;
                }
            }

            PrintBlizzards(width, height, blizzards);

            // They are cyclic
            return string.Empty;
        }

        private static void PrintBlizzards(int width, int height, bool[] blizzards)
        {
            var blizzardStride = width * height;
            var blizzardChars = new char[] { '^', '>', 'v', '<' };

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var numBlizzards = 0;
                    var blizzardDir = 0;
                    for (var d = 0; d < 4; d++)
                    {
                        if (blizzards[d * blizzardStride + x + y * width])
                        {
                            numBlizzards++;
                            blizzardDir = d;
                        }
                    }

                    if (numBlizzards == 1)
                    {
                        Console.Out.Write(blizzardChars[blizzardDir]);
                    }
                    else if (numBlizzards > 0)
                    {
                        Console.Out.Write((char)(numBlizzards + '0'));
                    }
                    else
                    {
                        Console.Out.Write('.');
                    }
                }
                Console.Out.Write('\n');
            }
        }
    }
}
