//#define LOG_PARSE_RESULT
//#define LOG_COMPARISONS

namespace Advent.Assignments
{
    internal class Day13_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var parser = new PacketParser();
            var divA = new PacketNode(new List<PacketNode> { new PacketNode(new List<PacketNode> { new PacketNode(2) }) });
            var divB = new PacketNode(new List<PacketNode> { new PacketNode(new List<PacketNode> { new PacketNode(6) }) });
            var nodes = new List<PacketNode> { divA, divB, };
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
    }
}
