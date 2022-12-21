//#define LOG_MOVES

namespace Advent.Assignments
{
    internal class Day20_1 : IAssignment
    {
        private class Node
        {
            public int value;

            public Node(int value)
            {
                this.value = value;
            }

            public override string ToString()
            {
                return value.ToString();
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            var length = input.Count;
            var original = new Node[length];
            var swap = new Node[length];
            Node? zero = null;

            for (int i = 0; i < length; i++)
            {
                var value = int.Parse(input[i]);
                var node = new Node(value);
                if (value == 0)
                    zero = node;
                original[i] = node;
                swap[i] = node;
            }

#if LOG_MOVES
            Logger.DebugLine("Initial arrangement:");
            Logger.DebugLine(swap.AggregateString());
#endif

            for (int i = 0; i < length; i++)
            {
                var node = original[i];
                var value = node.value;
                if (value == 0)
                {
#if LOG_MOVES
                    Logger.DebugLine($"{value} does not move:");
#endif
                }
                else if (value > 0)
                {
#if LOG_MOVES
                    Logger.DebugLine($"{value} moves:");
#endif
                    var index = Array.IndexOf(swap, node);
                    var crossedEdge = false;
                    for (int n = 0; n < value; n++)
                    {
                        var targetIndex = (index + 1) % length;
                        if (targetIndex == 0)
                            crossedEdge = true;
                        var tmp = swap[targetIndex];
                        swap[targetIndex] = swap[index];
                        swap[index] = tmp;
                        index = targetIndex;
                    }
                    if (crossedEdge)
                    {
                        var last = swap[length - 1];
                        Array.Copy(swap, 0, swap, 1, length - 1);
                        swap[0] = last;
#if LOG_MOVES
                        Logger.WarningLine("Crossed edge");
#endif
                    }
                }
                else
                {
#if LOG_MOVES
                    Logger.DebugLine($"{value} moves:");
#endif
                    var index = Array.IndexOf(swap, node);
                    var crossedEdge = false;
                    for (int n = 0; n < -value; n++)
                    {
                        var targetIndex = (index + length - 1) % length;
                        if (targetIndex == 0)
                            crossedEdge = true;
                        var tmp = swap[targetIndex];
                        swap[targetIndex] = swap[index];
                        swap[index] = tmp;
                        index = targetIndex;
                    }
                    if (crossedEdge)
                    {
                        var first = swap[0];
                        Array.Copy(swap, 1, swap, 0, length - 1);
                        swap[length - 1] = first;
#if LOG_MOVES
                        Logger.WarningLine("Crossed edge");
#endif
                    }
                }
#if LOG_MOVES
                Logger.DebugLine(swap.AggregateString());
                Logger.Line();
#endif
            }


#if LOG_MOVES
            Logger.DebugLine(swap.AggregateString());
#endif
            var zeroIndex = Array.IndexOf(swap, zero!);
            var grove1 = swap[(zeroIndex + 1000) % length].value;
            var grove2 = swap[(zeroIndex + 2000) % length].value;
            var grove3 = swap[(zeroIndex + 3000) % length].value;

            Logger.DebugLine($"{grove1}, {grove2}, {grove3}");

            return (grove1 + grove2 + grove3).ToString();
        }
    }
}
