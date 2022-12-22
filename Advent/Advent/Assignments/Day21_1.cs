namespace Advent.Assignments
{
    internal class Day21_1 : IAssignment
    {
        private enum OpCode
        {
            Add,
            Subtract, 
            Multiply, 
            Divide,
        }
        private record struct BinaryOp(string Left, string Right, OpCode OpCode);

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

            return Resolve("root", binOps, resolved).ToString();
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
