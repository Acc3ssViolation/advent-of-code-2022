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

            for (int i = 0; i < length; i++)
            {
                var node = original[i];
                var value = node.value;
                if (value == 0)
                {
                }
                else if (value > 0)
                {
                    var index = Array.IndexOf(swap, node);
                    for (int n = 0; n < value; n++)
                    {
                        var targetIndex = (index + 1) % length;
                        var tmp = swap[targetIndex];
                        swap[targetIndex] = swap[index];
                        swap[index] = tmp;
                        index = targetIndex;
                    }
                }
                else
                {
                    var index = Array.IndexOf(swap, node);
                    for (int n = 0; n < -value; n++)
                    {
                        var targetIndex = (index + length - 1) % length;
                        var tmp = swap[targetIndex];
                        swap[targetIndex] = swap[index];
                        swap[index] = tmp;
                        index = targetIndex;
                    }
                }
            }

            var zeroIndex = Array.IndexOf(swap, zero!);
            var grove1 = swap[(zeroIndex + 1000) % length].value;
            var grove2 = swap[(zeroIndex + 2000) % length].value;
            var grove3 = swap[(zeroIndex + 3000) % length].value;

            return (grove1 + grove2 + grove3).ToString();
        }
    }
}
