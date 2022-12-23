//#define SHOW_STEPS

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

        private static OpCode Opposite(OpCode op) => op switch
        {
            OpCode.Add => OpCode.Subtract,
            OpCode.Subtract => OpCode.Add,
            OpCode.Multiply => OpCode.Divide,
            _ => OpCode.Multiply,
        };

        private abstract class AstNode
        {
        }

        private class BinaryNode : AstNode
        {
            private static char[] OpCodeCharTable = new char[] { '+', '-', '*', '/', };

            public AstNode Left { get; set; }
            public AstNode Right { get; set; }
            public OpCode OpCode { get; set; }

            public BinaryNode(AstNode left, AstNode right, OpCode opCode)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
                OpCode = opCode;
            }

            public override string ToString()
            {
                return $"({Left} {OpCodeCharTable[(int)OpCode]} {Right})";
            }
        }

        private class NumberNode : AstNode
        {
            public long Value { get; }

            public NumberNode(long value)
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
            public UnknownNumberNode()
            {
            }

            public override string ToString()
            {
                return "x";
            }
        }

        private class EqualsNode : AstNode
        {
            public AstNode Left { get; set; }
            public AstNode Right { get; set; }

            public EqualsNode(AstNode left, AstNode right)
            {
                Left = left ?? throw new ArgumentNullException(nameof(left));
                Right = right ?? throw new ArgumentNullException(nameof(right));
            }

            public override string ToString()
            {
                return $"{Left} = {Right}";
            }
        }

        private record class BinaryOp(string Left, string Right, OpCode OpCode);

        public string Run(IReadOnlyList<string> input)
        {
            var binOps = new Dictionary<string, BinaryOp>();
            var resolved = new Dictionary<string, long>();
            var rootLeftKey = string.Empty;
            var rootRightKey = string.Empty;

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

                    if (key == "root")
                    {
                        rootLeftKey = left;
                        rootRightKey = right;
                    }
                }
            }

            var rootNode = (EqualsNode)ResolveNode("root", binOps, resolved);

            var unknownIsInLeftBranch = ContainsUnknownChild(rootNode.Left);

            rootNode = (EqualsNode)SimplifyNode(rootNode);

            Normalize(rootNode);

            var answer = ((NumberNode)rootNode.Right).Value;

#if SHOW_STEPS
            resolved["humn"] = (long)answer;
            var leftAnswer = Resolve(rootLeftKey, binOps, resolved);
            var rightAnswer = Resolve(rootRightKey, binOps, resolved);

            Logger.DebugLine($"Branch containing unknown: {(unknownIsInLeftBranch ? "left" : "right")}");
            Logger.DebugLine($"{leftAnswer} = {rightAnswer}");
            if (leftAnswer != rightAnswer)
            {
                Logger.ErrorLine($"{leftAnswer} - {rightAnswer} = {leftAnswer - rightAnswer}");
            }
#endif

            return answer.ToString();
        }

        private static void Normalize(EqualsNode root)
        {
            var (unknownTree, constantTree) = ContainsUnknownChild(root.Left) ? (root.Left, root.Right) : (root.Right, root.Left);
#if SHOW_STEPS
            var step = 1;
#endif
            while (true)
            {
                if (unknownTree is UnknownNumberNode)
                    break;

                var binaryToInvert = (BinaryNode)unknownTree;
                var unknownToTheLeft = ContainsUnknownChild(binaryToInvert.Left);

                if (unknownToTheLeft)
                {
                    var left = constantTree;
                    var right = binaryToInvert.Right;
                    constantTree = new BinaryNode(left, right, Opposite(binaryToInvert.OpCode));
                }
                else
                {
                    if (binaryToInvert.OpCode == OpCode.Divide || binaryToInvert.OpCode == OpCode.Subtract)
                    {
                        var left = binaryToInvert.Left;
                        var right = constantTree;
                        constantTree = new BinaryNode(left, right, binaryToInvert.OpCode);
                    }
                    else
                    {
                        var left = constantTree;
                        var right = binaryToInvert.Left;
                        constantTree = new BinaryNode(left, right, Opposite(binaryToInvert.OpCode));
                    }
                }

                unknownTree = unknownToTheLeft ? binaryToInvert.Left : binaryToInvert.Right;
#if SHOW_STEPS
                var tmp = new EqualsNode(unknownTree, constantTree);
                Logger.DebugLine($"[{step}] {tmp}");
                step++;
#endif
            }

            root.Left = unknownTree;
            root.Right = SimplifyNode(constantTree);
        }

        private static bool ContainsUnknownChild(AstNode node)
        {
            if (node is UnknownNumberNode)
                return true;

            if (node is BinaryNode binNode)
            {
                if (ContainsUnknownChild(binNode.Left))
                    return true;
                return ContainsUnknownChild(binNode.Right);
            }
            return false;
        }

        private static AstNode SimplifyNode(AstNode node)
        {
            if (node is EqualsNode equals)
            {
                equals.Left = SimplifyNode(equals.Left);
                equals.Right = SimplifyNode(equals.Right);
            }
            if (node is BinaryNode binary)
            {
                var left = SimplifyNode(binary.Left);
                var right = SimplifyNode(binary.Right);
                if (left is NumberNode numLeft && right is NumberNode numRight)
                {
                    checked
                    {
                        var value = binary.OpCode switch
                        {
                            OpCode.Add => numLeft.Value + numRight.Value,
                            OpCode.Subtract => numLeft.Value - numRight.Value,
                            OpCode.Multiply => numLeft.Value * numRight.Value,
                            _ => numLeft.Value / numRight.Value,
                        };

                        return new NumberNode(value);
                    }
                }
                binary.Left = left;
                binary.Right = right;
            }
            return node;
        }

        private static AstNode ResolveNode(string name, Dictionary<string, BinaryOp> binOps, Dictionary<string, long> resolved)
        {
            if (name == "humn")
                return new UnknownNumberNode();

            if (resolved.TryGetValue(name, out var value))
                return new NumberNode(value);

            var binOp = binOps[name];
            var leftNode = ResolveNode(binOp.Left, binOps, resolved);
            var rightNode = ResolveNode(binOp.Right, binOps, resolved);

            if (name == "root")
                return new EqualsNode(leftNode, rightNode);

            return new BinaryNode(leftNode, rightNode, binOp.OpCode);
        }

        private static long Resolve(string key, Dictionary<string, BinaryOp> binOps, Dictionary<string, long> resolved)
        {
            if (!resolved.TryGetValue(key, out var value))
            {
                var op = binOps[key];
                var left = Resolve(op.Left, binOps, resolved);
                var right = Resolve(op.Right, binOps, resolved);
                checked
                {
                    value = op.OpCode switch
                    {
                        OpCode.Add => left + right,
                        OpCode.Subtract => left - right,
                        OpCode.Multiply => left * right,
                        _ => left / right,
                    };
                    resolved[key] = value;
                }
            }

            return value;
        }
    }
}
