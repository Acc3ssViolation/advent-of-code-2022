using System.Text.Encodings.Web;

namespace Advent.Assignments
{
    internal class Day21_2 : IAssignment
    {
        private enum OpCode
        {
            Add,
            Subtract,
            Multiply,
            Divide,
        }

        private abstract class AstNode
        {
            public string Name { get; }

            protected AstNode(string name)
            {
                Name = name ?? throw new ArgumentNullException(nameof(name));
            }
        }

        private class BinaryNode : AstNode
        {
            private static char[] OpCodeCharTable = new char[] { '+', '-', '*', '/', };

            public AstNode Left { get; }
            public AstNode Right { get; }
            public OpCode OpCode { get; }

            public BinaryNode(string name, AstNode left, AstNode right, OpCode opCode) : base(name)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
                OpCode = opCode;
            }

            public override string ToString()
            {
                return $"({Left}) {OpCodeCharTable[(int)OpCode]} ({Right})";
            }
        }

        private class NumberNode : AstNode
        {
            public long Value { get; }

            public NumberNode(string name, long value) : base(name)
            {
                Value = value;
            }

            public override string ToString()
            {
                return Value.ToString();
            }
        }

        private class UnknownNumberNode : AstNode
        {
            public UnknownNumberNode(string name) : base(name)
            {
            }

            public override string ToString()
            {
                return "x";
            }
        }

        private class EqualsNode : AstNode
        {
            public AstNode Left { get; }
            public AstNode Right { get; }

            public EqualsNode(string name, AstNode left, AstNode right) : base(name)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public override string ToString()
            {
                return $"({Left}) = ({Right})";
            }
        }

        private record class BinaryOp(string Left, string Right, OpCode OpCode);

        public string Run(IReadOnlyList<string> input)
        {
            var binOps = new Dictionary<string, BinaryOp>();
            var resolved = new Dictionary<string, long>();

            for (var i = 0; i < input.Count; i++)
            {
                var line = input[i];
                var key = line[..4];
                if (line.Length < 17)
                {
                    resolved.Add(key, long.Parse(line[6..]));
                }
                else
                {
                    var left = line[6..10];
                    var right = line[13..17];
                    var op = line[11] switch
                    {
                        '+' => OpCode.Add,
                        '-' => OpCode.Subtract,
                        '*' => OpCode.Multiply,
                        _ => OpCode.Divide,
                    };
                    binOps.Add(key, new BinaryOp(left, right, op));
                }
            }

            var rootNode = (EqualsNode)ResolveNode("root", binOps, resolved);

            var leftNode = rootNode.Left;
            var rightNode = rootNode.Right;

            var nodeWithHuman = ContainsAsChild(leftNode, "humn") ? leftNode : rightNode;
            Logger.DebugLine(rootNode.ToString());
            const string AppId = "???";

            var httpClient = new HttpClient();
            var question = UrlEncoder.Default.Encode(rootNode.ToString());
            var url = $"https://api.wolframalpha.com/v1/result?appid={AppId}&i=solve+{question}";
            var result = httpClient.GetStringAsync(url).Result;

            return result[9..];
        }

        private static bool ContainsAsChild(AstNode node, string name)
        {
            if (node.Name == name)
                return true;

            if (node is BinaryNode binNode)
            {
                if (ContainsAsChild(binNode.Left, name))
                    return true;
                return ContainsAsChild(binNode.Right, name);
            }
            return false;
        }

        private static AstNode ResolveNode(string name, Dictionary<string, BinaryOp> binOps, Dictionary<string, long> resolved)
        {
            if (name == "humn")
                return new UnknownNumberNode(name);

            if (resolved.TryGetValue(name, out var value))
                return new NumberNode(name, value);

            var binOp = binOps[name];
            var leftNode = ResolveNode(binOp.Left, binOps, resolved);
            var rightNode = ResolveNode(binOp.Right, binOps, resolved);

            if (name == "root")
                return new EqualsNode(name, leftNode, rightNode);

            return new BinaryNode(name, leftNode, rightNode, binOp.OpCode);
        }

        private static long Resolve(string key, Dictionary<string, BinaryOp> binOps, Dictionary<string, long> resolved)
        {
            if (!resolved.TryGetValue(key, out var value))
            {
                var op = binOps[key];
                var left = Resolve(op.Left, binOps, resolved);
                var right = Resolve(op.Right, binOps, resolved);
                value = op.OpCode switch
                {
                    OpCode.Add => left + right,
                    OpCode.Subtract => left - right,
                    OpCode.Multiply => left * right,
                    _ => left / right,
                };
                resolved[key] = value;
            }

            return value;
        }
    }
}
