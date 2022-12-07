using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day05_1 : IAssignment
    {
        private static readonly Regex MoveRegex = new(@"move (\d+) from (\d+) to (\d+)");

        private struct CrateStack
        {
            public char[] data;
            public int depth;
        }

        public string Run(IReadOnlyList<string> input)
        {
            // First parse the stack layout
            var reversedStacks = new List<Stack<char>>();
            var totalCrates = 0;
            int lineIndex;
            for (lineIndex = 0; lineIndex < input.Count; lineIndex++)
            {
                var line = input[lineIndex];
                if (string.IsNullOrWhiteSpace(line))
                { 
                    lineIndex++;
                    break;
                }

                var x = 0;
                var inCrate = false;
                foreach (var chr in line)
                {
                    if (chr == '[')
                    {
                        inCrate = true;
                    }
                    else if (inCrate)
                    {
                        var stackIndex = x / 4;
                        while (reversedStacks.Count <= stackIndex)
                            reversedStacks.Add(new Stack<char>());

                        var stack = reversedStacks[stackIndex];
                        stack.Push(chr);
                        totalCrates++;
                        inCrate = false;
                    }

                    x++;
                }
            }

            Logger.DebugLine("Input parsed");

            // Flip the stacks upside down
            var stacks = new CrateStack[reversedStacks.Count];
            {
                var stackIndex = 0;
                foreach (var outStack in reversedStacks)
                {
                    var inStack = new CrateStack
                    {
                        data = new char[totalCrates],
                        depth = outStack.Count,
                    };
                    stacks[stackIndex++] = inStack;
                    for (var i = 0; i < inStack.depth; i++)
                    {
                        inStack.data[i] = outStack.Pop();
                    }
                }
            }
            Logger.DebugLine("Stacks flipped");

            // Then process the moves
            for (; lineIndex < input.Count; lineIndex++)
            {
                var line = input[lineIndex];
                var match = MoveRegex.Match(line);
                var count = int.Parse(match.Groups[1].Value);
                var from = int.Parse(match.Groups[2].Value) - 1;
                var to = int.Parse(match.Groups[3].Value) - 1;

                ref var fromStack = ref stacks[from];
                ref var toStack = ref stacks[to];

                Array.Copy(fromStack.data, fromStack.depth - count, toStack.data, toStack.depth, count);
                Array.Reverse(toStack.data, toStack.depth, count);

                toStack.depth += count;
                fromStack.depth -= count;
            }

            // Check which containers are on top
            var sb = new StringBuilder();
            foreach (var stack in stacks)
            {
                sb.Append(stack.data[stack.depth - 1]);
            }

            return sb.ToString();
        }
    }
}
