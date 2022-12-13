﻿//#define LOG_PARSE_RESULT
//#define LOG_COMPARISONS

namespace Advent.Assignments
{
    internal class Day13_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var parser = new Parser();
            var divA = new Node(new List<Node> { new Node(new List<Node> { new Node(2) }) });
            var divB = new Node(new List<Node> { new Node(new List<Node> { new Node(6) }) });
            var nodes = new List<Node> { divA, divB, };
            for (int l = 0; l < input.Count; l += 3)
            {
                var lineA = input[l + 0];
                var lineB = input[l + 1];

                nodes.Add(parser.ParseList(lineA));
                nodes.Add(parser.ParseList(lineB));
            }

            var indexA = 1;
            for (var i = 0; i < nodes.Count; i++)
            {
                if (divA.Compare(nodes[i]) > 0)
                    indexA++;
            }
            var indexB = 1;
            for (var i = 0; i < nodes.Count; i++)
            {
                if (divB.Compare(nodes[i]) > 0)
                    indexB++;
            }

            return (indexA * indexB).ToString();
        }

        private class Parser
        {
            int _index;
            string _data = "";

            public Node ParseList(string data)
            {
                _data = data;
                _index = 0;
                return ParseList();
            }

            public Node ParseList()
            {
                var children = new List<Node>();
                // Opening [
                _index++;

                while (true)
                {
                    var chr = _data[_index];
                    if (chr == ']')
                    {
                        _index++;
                        break;
                    }
                    else if (chr == ',')
                    {
                        // Next child
                        _index++;
                    }
                    else if (chr == '[')
                    {
                        var childList = ParseList();
                        children.Add(childList);
                    }
                    else
                    {
                        var child = ParseNumber();
                        children.Add(child);
                    }
                }

                return new Node(children);
            }

            public Node ParseNumber()
            {
                var num = 0;
                while (true)
                {
                    var chr = _data[_index];
                    if (!char.IsNumber(chr))
                    {
                        break;
                    }
                    num *= 10;
                    num += chr - '0';
                    _index++;
                }
                return new Node(num);
            }
        }

        private class Node : IComparable
        {
            public List<Node> children;
            public int value = -1;

            public Node(List<Node> children)
            {
                this.children = children ?? throw new ArgumentNullException(nameof(children));
            }

            public Node(int value)
            {
                children = new List<Node>();
                this.value = value;
            }

            public int Compare(Node other)
            {
                if (value >= 0 && other.value >= 0)
                {
                    // Both are numbers
#if LOG_COMPARISONS
                    Logger.DebugLine($"Compare {value} vs {other.value}");
#endif
                    return Math.Sign(value - other.value);
                }
                else if (value >= 0)
                {
#if LOG_COMPARISONS
                    Logger.DebugLine($"Mixed types; convert left to [{value}] and retry comparison");
#endif
                    return new Node(new List<Node> { this }).Compare(other);
                }
                else if (other.value >= 0)
                {
#if LOG_COMPARISONS
                    Logger.DebugLine($"Mixed types; convert right to [{other.value}] and retry comparison");
#endif
                    return Compare(new Node(new List<Node> { other }));
                }
#if LOG_COMPARISONS
                Logger.DebugLine($"Compare {this} vs {other}");
#endif
                var maxIndex = Math.Min(children.Count, other.children.Count);
                for (int i = 0; i < maxIndex; i++)
                {
                    var result = children[i].Compare(other.children[i]);
                    if (result != 0)
                        return result;
                }
                return Math.Sign(children.Count - other.children.Count);
            }

            public int CompareTo(object? obj)
            {
                return Compare((Node)obj);
            }

            public override string ToString()
            {
                if (value >= 0)
                {
                    return value.ToString();
                }
                return "[" + children.Aggregate("", (a, b) => a.Length > 0 ? a + ", " + b : b.ToString()) + "]";
            }
        }
    }
}
