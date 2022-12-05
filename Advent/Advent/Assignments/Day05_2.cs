using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day05_2 : IAssignment
    {

        private static readonly Regex MoveRegex = new(@"move (\d+) from (\d+) to (\d+)");

        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            // First parse the stack layout
            var reversedStacks = new List<Stack<char>>();

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
                        inCrate = false;
                    }

                    x++;
                }
            }

            // Flip the stacks upside down
            var stacks = new List<List<char>>();
            foreach (var outStack in reversedStacks)
            {
                var inStack = new List<char>();
                stacks.Add(inStack);
                while (outStack.Count > 0)
                    inStack.Add(outStack.Pop());
            }

            // Then process the moves
            for (; lineIndex < input.Count; lineIndex++)
            {
                var line = input[lineIndex];
                var match = MoveRegex.Match(line);
                var count = int.Parse(match.Groups[1].Value);
                var from = int.Parse(match.Groups[2].Value) - 1;
                var to = int.Parse(match.Groups[3].Value) - 1;

                var fromStack = stacks[from];
                var toStack = stacks[to];

                var range = fromStack.GetRange(fromStack.Count - count, count);
                fromStack.RemoveRange(fromStack.Count - count, count);
                toStack.AddRange(range);
            }

            // Check which containers are on top
            var sb = new StringBuilder();
            foreach (var stack in stacks)
            {
                sb.Append(stack[stack.Count - 1]);
            }

            return Task.FromResult(sb.ToString());
        }
    }
}
