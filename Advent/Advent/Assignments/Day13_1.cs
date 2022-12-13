//#define LOG_PARSE_RESULT
//#define LOG_COMPARISONS

using System.Xml.Linq;

namespace Advent.Assignments
{
    internal class Day13_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var correctSum = 0;
            var parser = new PacketParser();
            for (int l = 0; l < input.Count; l += 3)
            {
                var lineA = input[l + 0];
                var lineB = input[l + 1];

                var listA = parser.ParseList(lineA);
                var listB = parser.ParseList(lineB);
#if LOG_PARSE_RESULT
                Logger.DebugLine(listA.ToString());
                Logger.DebugLine(listB.ToString());
                Logger.Line();
#endif
                var ordered = listA.Compare(listB);
#if LOG_COMPARISONS
                Logger.DebugLine($"Ordered: {ordered}");
#endif
                if (ordered < 0)
                    correctSum += (l / 3) + 1;
            }
            return correctSum.ToString();
        }
    }

    public class PacketParser
    {
        int _index;
        string _data = "";

        public PacketNode ParseList(string data)
        {
            _data = data;
            _index = 0;
            return ParseList();
        }

        public PacketNode ParseList()
        {
            var children = new List<PacketNode>();
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

            return new PacketNode(children);
        }

        public PacketNode ParseNumber()
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
            return new PacketNode(num);
        }
    }

    public class PacketNode
    {
        public List<PacketNode> children;
        public int value = -1;

        public PacketNode(List<PacketNode> children)
        {
            this.children = children ?? throw new ArgumentNullException(nameof(children));
        }

        public PacketNode(int value)
        {
            children = new List<PacketNode>();
            this.value = value;
        }

        public int Compare(PacketNode other)
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
                return new PacketNode(new List<PacketNode> { this }).Compare(other);
            }
            else if (other.value >= 0)
            {
#if LOG_COMPARISONS
                    Logger.DebugLine($"Mixed types; convert right to [{other.value}] and retry comparison");
#endif
                return Compare(new PacketNode(new List<PacketNode> { other }));
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
