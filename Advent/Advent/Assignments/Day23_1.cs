//#define LOG_MOVES
//#define LOG_ROUND_END

namespace Advent.Assignments
{
    internal class Day23_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var mapWidth = 5000;
            var mapHeight = 5000;
            var offset = 4000;
            var elfPositionSet = new IntSet(mapWidth * mapHeight);
            var elfTargetPositionSet = new IntSet(mapWidth * mapHeight);
            var elfDuplicatePositionSet = new IntSet(mapWidth * mapHeight);
            var elfPositionList = new List<int>(input[0].Length * input[0].Length);
            
            for (int y = 0; y < input.Count; y++)
            {
                var line = input[y];
                for (int x = 0; x < line.Length; x++)
                {
                    if (line[x] == '#')
                    {
                        var px = x + offset;
                        var py = y + offset;
                        elfPositionSet.Add(px + py * mapWidth);
                        elfPositionList.Add(px + py * mapWidth);
                    }
                }
            }

            var elfPositions = elfPositionList.ToArray();
            var elfCount = elfPositions.Length;
            var elfTargetPositions = new int[elfCount];

            // N, NE, E, SE, S, SW, W, NW
            var adjacentOffsets = new int[]
            { 
                -mapWidth,
                -mapWidth + 1,
                1,
                mapWidth + 1,
                mapWidth,
                mapWidth - 1,
                -1,
                -mapWidth - 1,
            };

            // N, S, W, E
            var checkMasks = new int[] 
            {
                0x83,
                0x38,
                0xE0,
                0x0E,
            };

            var checkOffsets = new int[]
            {
                -mapWidth,
                mapWidth,
                -1,
                +1,
            };

            var checkMaskNames = new string[]
            {
                "N",
                "S",
                "W",
                "E",
            };

            // Start out at N
            var moveCheckStart = 0;

            for (var round = 0; round < 10; round++)
            {
                // First half
                for (var e = 0; e < elfCount; e++)
                {
                    // 1. Check if we have any neighbours
#if LOG_MOVES
                    Logger.DebugLine($"Elf {e} is at {elfPositions[e].x},{elfPositions[e].y}");
#endif
                    var elfPosIndex = elfPositions[e];
                    var neighbourMask = 0;
                    for (var o = 0; o < adjacentOffsets.Length; o++)
                    {
                        var checkIndex = elfPosIndex + adjacentOffsets[o];
                        if (elfPositionSet.Includes(checkIndex))
                        {
                            neighbourMask |= 1 << o;
                        }
                    }

                    if (neighbourMask == 0)
                    {
                        elfTargetPositions[e] = elfPosIndex;
#if LOG_MOVES
                        Logger.DebugLine($"Elf {e} has no neighbours, stays in place");
#endif
                        continue;
                    }

                    // 2. We have a neighbour, find a move proposal
                    for (var m = 0; m < 4; m++)
                    {
                        var checkIndex = (m + moveCheckStart) & 0x3;

                        if ((neighbourMask & checkMasks[checkIndex]) != 0)
                        {
                            // This move is not available
#if LOG_MOVES
                            Logger.DebugLine($"Elf {e} cannot move {checkMaskNames[checkIndex]}");
#endif
                            if (m == 3)
                            {
                                // Ran out of options
#if LOG_MOVES
                                Logger.DebugLine($"Elf {e} cannot move at all");
#endif
                                elfTargetPositions[e] = elfPosIndex;
                            }
                            continue;
                        }

                        var p1 = elfPosIndex + checkOffsets[checkIndex];
                        // p1 is our proposed move, it is the 'center' of our 3 sided check
                        if (elfTargetPositionSet.Includes(p1))
                            elfDuplicatePositionSet.Add(p1);
                        elfTargetPositionSet.Add(p1);
                        elfTargetPositions[e] = p1;
#if LOG_MOVES
                        Logger.DebugLine($"Elf {e} wants to move {checkMaskNames[checkIndex]} to {elfTargetPositions[e].x},{elfTargetPositions[e].y}");
#endif
                        break;
                    }
                }

                // Clear old data
                elfPositionSet.Clear();

                // Part 2
                for (var e = 0; e < elfCount; e++)
                {
                    var targetPos = elfTargetPositions[e];
                    if (elfDuplicatePositionSet.Includes(targetPos))
                    {
                        // Do not move
#if LOG_MOVES
                        Logger.DebugLine($"Elf {e} can not move to proposed location, stays at {elfPositions[e].x},{elfPositions[e].y}");
#endif
                        elfPositionSet.Add(elfPositions[e]);
                        continue;
                    }

                    // Space is available, move the elf
                    elfPositions[e] = targetPos;
                    elfPositionSet.Add(targetPos);
#if LOG_MOVES
                    Logger.DebugLine($"Elf {e} moves to proposed location {elfPositions[e].x},{elfPositions[e].y}");
#endif
                }

                // Use a different starting point for the direction checks
                moveCheckStart = (moveCheckStart + 1) & 0x3;

                // Clear old round data
                elfTargetPositionSet.Clear();
                elfDuplicatePositionSet.Clear();


                // Print
#if LOG_ROUND_END
                Console.Out.Write($"== End of Round {round + 1} ==\n");
                PrintMap(3998, 3998, 11, 11, mapWidth, elfPositions);
#endif
            }

            var minX = mapWidth;
            var minY = mapHeight;
            var maxX = 0;
            var maxY = 0;
            for (var e = 0; e < elfCount; e++)
            {
                var x = elfPositions[e] % mapWidth;
                var y = elfPositions[e] / mapWidth;
                if (x < minX)
                    minX = x;
                else if (x > maxX)
                    maxX = x;

                if (y < minY)
                    minY = y;
                else if (y > maxY)
                    maxY = y;
            }
            
            var width = maxX - minX + 1;
            var height = maxY - minY + 1;

            //Logger.DebugLine($"Rect is {width}x{height}={width * height}");

            return ((width * height) - elfCount).ToString();
        }

        private static void PrintMap(int minX, int minY, int width, int height, int mapWidth, ReadOnlySpan<Vector2Int> elfPositions)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var wx = x + minX;
                    var wy = y + minY;
                    var chr = '.';
                    for (int e = 0; e < elfPositions.Length; e++)
                    {
                        if (elfPositions[e].x == wx && elfPositions[e].y == wy)
                        {
                            chr = e.ToString()[0];
                            break;
                        }
                    }
                    Console.Out.Write(chr);
                }
                Console.Out.Write('\n');
            }
            Console.Out.Write('\n');
        }
    }
}
