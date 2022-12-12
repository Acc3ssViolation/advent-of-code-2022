//#define LOG_EVERY_TREE
//#define LOG_TALL_TREES
using System.Diagnostics;

namespace Advent.Assignments
{
    internal class Day08_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            // Construct the map
            var width = input[0].Trim().Length;
            var height = input.Count;

            Logger.DebugLine($"Width: {width}, Height: {height}");

            var trees = new int[width* height];

            for (int y = 0; y < height; y++)
            {
                var line = input[y];
                for (int x = 0; x < width; x++)
                {
                    trees[x + y * width] = line[x] - '0';
                }
            }

            // Do the thing!
            var seenTrees = new IntSet(width * height);

            // Horizontal visibility
            for (int y = 1; y < height - 1; y++)
            {
                var treeHeight = trees[y * width + 0];
                var tallestTreeX = 0;

                // Left to right
                for (int x = 1; x < width - 1; x++)
                {
                    var tree = trees[y * width + x];
#if LOG_EVERY_TREE
                    Logger.DebugLine($"L [{y},{x}]: {tree}");
#endif
                    if (tree > treeHeight)
                    {
#if LOG_TALL_TREES
                        Logger.DebugLine($"L [{y},{x}]: {tree} TALL");
#endif
                        treeHeight = tree;
                        tallestTreeX = x;
                        seenTrees.TryAdd(y * width + x);
                    }
                }

                // Right to left
                treeHeight = trees[y * width + (width - 1)];
                for (int x = width - 2; x > tallestTreeX; x--)
                {
                    var tree = trees[y * width + x];
#if LOG_EVERY_TREE
                    Logger.DebugLine($"R [{y},{x}]: {tree}");
#endif
                    if (tree > treeHeight)
                    {
#if LOG_TALL_TREES
                        Logger.DebugLine($"R [{y},{x}]: {tree} TALL");
#endif
                        treeHeight = tree;
                        seenTrees.TryAdd(y * width + x);
                    }
                }
            }

            // Vertical visibility
            Logger.DebugLine("Vertical");

            for (int x = 1; x < width - 1; x++)
            {
                var treeHeight = trees[x];
                var tallestTreeY = 0;

                // Top to bottom
                for (int y = 1; y < height - 1; y++)
                {
                    var tree = trees[y * width + x];
#if LOG_EVERY_TREE
                    Logger.DebugLine($"T [{y},{x}]: {tree}");
#endif
                    if (tree > treeHeight)
                    {
#if LOG_TALL_TREES
                        Logger.DebugLine($"T [{y},{x}]: {tree} TALL");
#endif
                        treeHeight = tree;
                        tallestTreeY = y;
                        seenTrees.TryAdd(y * width + x);
                    }
                }

                // Bottom to top
                treeHeight = trees[(height - 1) * width + x];
                for (int y = height - 2; y > tallestTreeY; y--)
                {
                    var tree = trees[y * width + x];
#if LOG_EVERY_TREE
                    Logger.DebugLine($"B [{y},{x}]: {tree}");
#endif
                    if (tree > treeHeight)
                    {
#if LOG_TALL_TREES
                        Logger.DebugLine($"B [{y},{x}]: {tree} TALL");
#endif
                        treeHeight = tree;
                        seenTrees.TryAdd(y * width + x);
                    }
                }
            }

            // Edge trees are always visible
            var visibleTrees = seenTrees.Count + (width - 1) * 2 + (height - 1) * 2;
            return visibleTrees.ToString();
        }
    }

    internal class IntSet
    {
        private uint[] _flags;
        private int _count;

        public int Count => _count;

        public IntSet(int size)
        {
            _flags = new uint[(size - 1) / 32 + 1];
        }

        public bool TryAdd(int value)
        {
            var index = value / 32;
            var mask = 1U << (value & 0x1F);
            if ((_flags[index] & mask ) > 0)
                return false;

            _count++;
            _flags[index] |= mask;
            return true;
        }

        public bool Includes(int value)
        {
            var index = value / 32;
            var mask = 1U << (value & 0x1F);
            return (_flags[index] & mask) > 0;
        }

        public void Add(int value)
        {
            var index = value / 32;
            var mask = 1U << (value & 0x1F);
            _flags[index] |= mask;
        }
    }
}
