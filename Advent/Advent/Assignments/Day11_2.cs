//#define LOG_ROUNDS

namespace Advent.Assignments
{
    internal class Day11_2 : IAssignment
    {
        enum MonkeyOp
        {
            Add,
            Multiply,
            Square,
        }

        struct Monkey
        {
            public MonkeyOp op;
            public int opValue;
            public int test;
            public int nextIndexTrue;
            public int nextIndexFalse;
            public int items;
            public int inspected;
        }

        public string Run(IReadOnlyList<string> input)
        {
            const int MaxMonkeys = 8;
            const int MaxItems = 64;
            var normalizer = 1;

            var items = new int[MaxMonkeys * MaxItems];
            var monkeys = new Monkey[MaxMonkeys];
            var monkeyCount = 0;

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].Length > 0 && input[i][0] == 'M')
                {
                    ref var monkey = ref monkeys[monkeyCount];

                    // Load the starting items
                    var split = input[i + 1][18..].Split(',');
                    for (int k = 0; k < split.Length; k++)
                        items[monkeyCount * MaxItems + k] = int.Parse(split[k]);
                    monkey.items = split.Length;

                    // Load the operation
                    monkey.op = input[i + 2][23] == '+' ? MonkeyOp.Add : MonkeyOp.Multiply;
                    var operand = input[i + 2].AsSpan(25);
                    if (operand[0] == 'o')
                        monkey.op = MonkeyOp.Square;
                    else
                        monkey.opValue = int.Parse(operand);

                    // Load the test
                    monkey.test = int.Parse(input[i + 3].AsSpan(21));
                    monkey.nextIndexTrue = int.Parse(input[i + 4].AsSpan(29));
                    monkey.nextIndexFalse = int.Parse(input[i + 5].AsSpan(30));

                    // The normalizer must be a common multiple of all test divisors. Since they are all prime this multiplication will give us the Least Common Multiple.
                    normalizer *= monkey.test;

                    i += 6;
                    monkeyCount++;
                }
            }

            for (int r = 0; r < 10000; r++)
            {
                for (int m = 0; m < monkeyCount; m++)
                {
                    // Monkey businessssss
                    ref var monkey = ref monkeys[m];
                    monkey.inspected += monkey.items;

                    var itemSlice = items.AsSpan(m * MaxItems, MaxItems);
                    for (int i = 0; i < monkey.items; i++)
                    {
                        long worry = itemSlice[i];
                        if (monkey.op == MonkeyOp.Add)
                            worry += monkey.opValue;
                        else if (monkey.op == MonkeyOp.Multiply)
                            worry *= monkey.opValue;
                        else
                            worry *= worry;

                        // Todo: use bitmask if possible?
                        worry %= normalizer;
                        var target = monkey.nextIndexFalse;
                        if (worry % monkey.test == 0)
                            target = monkey.nextIndexTrue;
                        // Move the item to the other monkey
                        items[target * MaxItems + monkeys[target].items] = (int)worry;
                        monkeys[target].items++;
                    }
                    monkey.items = 0;
                }

#if LOG_ROUNDS
                // PRINT IT
                if (r == 0 || r == 19 || (r % 1000 == 999))
                {
                    Logger.DebugLine($"== After round {r + 1} ==");
                    for (int m = 0; m < monkeyCount; m++)
                        Logger.DebugLine($"Monkey {m} inspected items {monkeys[m].inspected} times.");
                    Logger.Line();
                }
#endif
            }

            long max1 = 0;
            long max2 = 0;

            for (int i = 0; i < monkeyCount; i++)
            {
                var itemCount = monkeys[i].inspected;
                if (itemCount > max1)
                {
                    max2 = max1;
                    max1 = itemCount;
                }
                else if (itemCount > max2)
                {
                    max2 = itemCount;
                }
            }

            var business = max1 * max2;

            return business.ToString();
        }
    }
}
