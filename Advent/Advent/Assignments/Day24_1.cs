namespace Advent.Assignments
{
    internal class Day24_1 : IAssignment
    {
        private class World
        {
            private int _width;
            private int _height;
            private int _blizzardStride;
            // We only need 4 masks containing the blizzards in each direction
            // Directions: 0 = N, 1 = E, 2 = S, 3 = W
            private bool[] _blizzards;

            public World(int width, int height)
            {
                _width = width;
                _height = height;
                _blizzardStride = width* height;
                _blizzards = new bool[_blizzardStride * 4];
            }

            public bool IsCoveredByBlizzard(int x, int y, int t)
            {
                for (var d = 0; d < 4; d++)
                {
                    var positionInBlizzard = GetPositionInBlizzard(x, y, t, d);
                    if (_blizzards[positionInBlizzard + d * _blizzardStride])
                        return true;
                }
                return false;
            }

            private int GetPositionInBlizzard(int x, int y, int t, int d)
            {
                if (d == 0)
                {
                    // North moving blizzards
                    // Every tick they move up. This means we need to look at y + t instead of y.
                    y = (y + t) % _height;
                }
                else if (d == 1)
                {
                    // East moving blizzards
                    // Look at x - t
                    x = (x + (_width - 1) * t) % _width;
                }
                else if (d == 2)
                {
                    // South moving blizzards
                    // Look at y - t
                    y = (y + (_height - 1) * t) % _height;
                }
                else
                {
                    // East moving blizzards
                    // Look at x + t
                    x = (x + t) % _width;
                }

                return (x + y * _width) % _blizzardStride;
            }

            public void ParseLine(int y, string line)
            {
                for (int x = 0; x < _width; x++)
                {
                    var chr = line[x + 1];
                    var blizzardIndex = chr switch
                    {
                        '<' => _blizzardStride * 3,
                        'v' => _blizzardStride * 2,
                        '>' => _blizzardStride * 1,
                        '^' => 0,
                        _ => -1,
                    };
                    if (blizzardIndex < 0)
                        continue;
                    _blizzards[blizzardIndex + x + y * _width] = true;
                }
            }

            public void Print(int t = 0)
            {
                var blizzardChars = new char[] { '^', '>', 'v', '<' };

                for (var y = 0; y < _height; y++)
                {
                    for (var x = 0; x < _width; x++)
                    {
                        var numBlizzards = 0;
                        var blizzardDir = 0;
                        for (var d = 0; d < 4; d++)
                        {
                            var pos = GetPositionInBlizzard(x, y, t, d);
                            if (_blizzards[d * _blizzardStride + pos])
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

        public string Run(IReadOnlyList<string> input)
        {
            // 120x24
            // 5x5

            // We exclude the edges. The expedition can always move to the square below it on turn 1 (at least in the example and my input)
            var width = input[0].Length - 2;
            var height = input.Count - 2;

            var world = new World(width, height);

            for (int y = 0; y < height; y++)
            {
                world.ParseLine(y, input[y + 1]);
            }

            for (int t = 0; t < 10; t++)
            {
                Logger.WarningLine($"Minute {t}");
                world.Print(t);
            }

            // They are cyclic
            return string.Empty;
        }
    }
}
