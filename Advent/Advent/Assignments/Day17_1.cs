//#define LOG_BLOCKS

using System.Text;

namespace Advent.Assignments
{
    internal class Day17_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var rocks = new RockShape[]
            {
                new RockShape(
                    new Vector2Int[]{ new(0, 0) },
                    new Vector2Int[]{ new(3, 0) },
                    new Vector2Int[]{ new(0, 0), new(1, 0), new(2, 0), new(3, 0) },
                    new Vector2Int[]{ new(0, 0), new(1, 0), new(2, 0), new(3, 0) }
                ),

                new RockShape(
                    new Vector2Int[]{ new(1, 0), new(0, 1), new(1, 2) },
                    new Vector2Int[]{ new(1, 0), new(2, 1), new(1, 2) },
                    new Vector2Int[]{ new(0, 1), new(1, 0), new(2, 1) },
                    new Vector2Int[]{ new(1, 1), new(0, 1), new(1, 2), new(2, 1), new(1, 0) }
                ),

                new RockShape(
                    new Vector2Int[]{ new(0, 0), new(2, 1), new(2, 2) },
                    new Vector2Int[]{ new(2, 0), new(2, 1), new(2, 2) },
                    new Vector2Int[]{ new(0, 0), new(1, 0), new(2, 0) },
                    new Vector2Int[]{ new(0, 0), new(1, 0), new(2, 0), new(2, 1), new(2, 2) }
                ),

                new RockShape(
                    new Vector2Int[]{ new(0, 0), new(0, 1), new(0, 2), new(0, 3) },
                    new Vector2Int[]{ new(0, 0), new(0, 1), new(0, 2), new(0, 3) },
                    new Vector2Int[]{ new(0, 0) },
                    new Vector2Int[]{ new(0, 0), new(0, 1), new(0, 2), new(0, 3) }
                ),

                new RockShape(
                    new Vector2Int[]{ new(0, 0), new (0, 1) },
                    new Vector2Int[]{ new(1, 0), new (1, 1) },
                    new Vector2Int[]{ new(0, 0), new (1, 0) },
                    new Vector2Int[]{ new(0, 0), new(1, 0), new(0, 1), new(1, 1) }
                ),
            };

            var width = 7;
            var height = 10000;
            var world = new bool[width * height];

            var rockIndex = 0;
            var rockCount = 0;
            var floorHeight = -1;
            var commandIndex = 0;

            while (rockCount < 2022)
            {
                var rock = rocks[rockIndex];
                rockIndex = (rockIndex + 1) % rocks.Length;

                var x = 2;
                var y = floorHeight + 4;

                while (true)
                {
                    // Gas
                    if (input[0][commandIndex] == '>')
                    {
                        // Right
                        var pos = new Vector2Int(x + 1, y);

                        var hit = false;
                        for (int i = 0; i < rock.RightCollisions.Length; i++)
                        {
                            var checkPos = rock.RightCollisions[i] + pos;
                            if (checkPos.x >= width)
                            {
                                hit = true;
                                break;
                            }

                            if (world[checkPos.x + checkPos.y * width])
                            {
                                hit = true;
                                break;
                            }
                        }

                        if (!hit)
                            x++;
                    }
                    else
                    {
                        // Left
                        var pos = new Vector2Int(x - 1, y);

                        var hit = false;
                        for (int i = 0; i < rock.LeftCollisions.Length; i++)
                        {
                            var checkPos = rock.LeftCollisions[i] + pos;
                            if (checkPos.x < 0)
                            {
                                hit = true;
                                break;
                            }

                            if (world[checkPos.x + checkPos.y * width])
                            {
                                hit = true;
                                break;
                            }
                        }

                        if (!hit)
                            x--;
                    }
                    commandIndex = (commandIndex + 1) % input[0].Length;

#if LOG_BLOCKS
                    Logger.DebugLine("Move left or right");
                    Print(width, 10, world, rock, new Vector2Int(x, y));
#endif
                    // Fall
                    var hitFloor = false;
                    var fallPos = new Vector2Int(x, y - 1);
                    for (int i = 0; i < rock.FallCollisions.Length; i++)
                    {
                        var checkPos = rock.FallCollisions[i] + fallPos;
                        if (checkPos.y < 0)
                        {
                            hitFloor = true;
                            break;
                        }

                        if (world[checkPos.x + checkPos.y * width])
                        {
                            hitFloor = true;
                            break;
                        }
                    }

                    if (hitFloor)
                    {
                        var pos = new Vector2Int(x, y);
                        for (int i = 0; i < rock.Shape.Length; i++)
                        {
                            var shapePos = rock.Shape[i] + pos;
                            world[shapePos.x + shapePos.y * width] = true;
                            if (shapePos.y > floorHeight)
                                floorHeight = shapePos.y;
                        }
                        rockCount++;
#if LOG_BLOCKS
                        Print(width, 10, world);
#endif
                        break;
                    }
                    else
                    {
                        y--;
#if LOG_BLOCKS
                        Logger.DebugLine("Fall");
                        Print(width, 10, world, rock, new Vector2Int(x, y));
#endif
                    }
                }
            }

            return (floorHeight + 1).ToString();
        }

        private static void Print(int width, int height, ReadOnlySpan<bool> world, RockShape rock, Vector2Int rockPos)
        {
            for (int y = height; y >= 0; y--)
            {
                Console.Out.Write('|');

                for (int x = 0; x < width; x++)
                {
                    var chr = world[x + y * width] ? '#' : '.';
                    var pos = new Vector2Int(x, y);
                    for (int i = 0; i < rock.Shape.Length; i++)
                    {
                        if (rock.Shape[i] + rockPos == pos)
                        {
                            chr = '@';
                            break;
                        }
                    }
                    Console.Out.Write(chr);
                }

                Console.Out.Write('|');
                Console.Out.WriteLine();
            }

            Console.Out.Write('+');
            Console.Out.Write(new string('-', width));
            Console.Out.Write('+');
            Console.Out.WriteLine();
        }

        private static void Print(int width, int height, ReadOnlySpan<bool> world)
        {
            for (int y = height; y >= 0; y--)
            {
                Console.Out.Write('|');

                for (int x = 0; x < width; x++)
                {
                    Console.Out.Write(world[x + y * width] ? '#' : '.');
                }

                Console.Out.Write('|');
                Console.Out.WriteLine();
            }

            Console.Out.Write('+');
            Console.Out.Write(new string('-', width));
            Console.Out.Write('+');
            Console.Out.WriteLine();
        }

        private class RockShape
        {
            public Vector2Int[] LeftCollisions;
            public Vector2Int[] RightCollisions;
            public Vector2Int[] FallCollisions;

            public Vector2Int[] Shape;

            public RockShape(Vector2Int[] leftCollisions, Vector2Int[] rightCollisions, Vector2Int[] fallCollisions, Vector2Int[] shape)
            {
                LeftCollisions = leftCollisions ?? throw new ArgumentNullException(nameof(leftCollisions));
                RightCollisions = rightCollisions ?? throw new ArgumentNullException(nameof(rightCollisions));
                FallCollisions = fallCollisions ?? throw new ArgumentNullException(nameof(fallCollisions));
                Shape = shape ?? throw new ArgumentNullException(nameof(shape));
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                for (int y = 3; y >= 0; y--)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (Shape.Contains(new(x, y)))
                            sb.Append('#');
                        else
                            sb.Append('.');
                    }
                    sb.AppendLine();
                }
                return sb.ToString();
            }
        }
    }
}
