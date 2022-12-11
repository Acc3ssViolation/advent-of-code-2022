//#define LOG_MONKEYS
//#define LOG_ROUNDS

namespace Advent.Assignments
{
    internal class Day11_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var monkeys = new List<Monkey2>();
            var builder = new MonkeyBuilder2();

            for (int i = 0; i < input.Count; i++)
            {
                if (input[i].Length > 0 && input[i][0] == 'M')
                {
                    builder.ParseStartingItems(input[i + 1]);
                    builder.ParseOperation(input[i + 2]);
                    // The divisor here is always prime! This means that for the Test to be true, the worry level is NOT prime
                    builder.ParseTest(input[i + 3], input[i + 4], input[i + 5]);
                    monkeys.Add(builder.Build());
                }
            }

            for (int i = 0; i < monkeys.Count; i++)
                monkeys[i].Divisor = builder.Divisor;

            for (int i = 0; i < 10000; i++)
            {
                for (int m = 0; m < monkeys.Count; m++)
                    monkeys[m].Round(monkeys);
#if LOG_ROUNDS
                LogRound(monkeys, i + 1);
#endif
            }

            long max1 = 0;
            long max2 = 0;

            for (int i = 0; i < monkeys.Count; i++)
            {
                var itemCount = monkeys[i].InspectedItems;
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

        private static void LogRound(List<Monkey2> monkeys, int round)
        {
            Logger.DebugLine($"After round {round}, the monkeys are holding items with these worry levels:");
            for (int i = 0; i < monkeys.Count;i++)
            {
                Logger.DebugLine($"Monkey {i}: {monkeys[i].Items.Aggregate("", (s, i) => (s == "" ? i.ToString() : $"{s}, {i}"))}");
            }
        }
    }

    internal class Monkey2
    {
        private readonly List<long> _items = new();
        private bool _addition;
        private int _operationValue;
        private int _testDivisor;
        private int _nextIndexTrue;
        private int _nextIndexFalse;
        private int _inspectedItems;
        private int _divisor;

        public int Divisor { get { return _divisor; } set { _divisor = value; } }
        public IReadOnlyList<long> Items => _items;
        public int InspectedItems => _inspectedItems;

        public Monkey2(List<long> items, bool addition, int operationValue, int testDivisor, int nextIndexTrue, int nextIndexFalse)
        {
            _items = items ?? throw new ArgumentNullException(nameof(items));
            _addition = addition;
            _operationValue = operationValue;
            _testDivisor = testDivisor;
            _nextIndexTrue = nextIndexTrue;
            _nextIndexFalse = nextIndexFalse;
        }

        public void Round(List<Monkey2> monkeys)
        {
#if LOG_MONKEYS
            Logger.DebugLine($"Monkey {monkeys.IndexOf(this)}:");
#endif
            for (int i = 0; i < _items.Count; i++)
            {
                checked
                {
                    _inspectedItems++;
                    var worryLevel = _items[i];
#if LOG_MONKEYS
                Logger.DebugLine($"\tMonkey inspects an item with a worry level of {worryLevel}");
#endif
                    var operand = _operationValue == -1 ? worryLevel : _operationValue;
                    if (_addition)
                    {
                        worryLevel += operand;
#if LOG_MONKEYS
                    Logger.DebugLine($"\t\tWorry level increases by {operand} to {worryLevel}");
#endif
                    }
                    else
                    {
                        worryLevel *= operand;
#if LOG_MONKEYS
                    Logger.DebugLine($"\t\tWorry level is multiplied by {operand} to {worryLevel}");
#endif
                    }
                    worryLevel %= _divisor;
                    // Note that worryLevel is _always_ a multiple of 3 here
                    int target = _nextIndexFalse;
                    if (worryLevel % _testDivisor == 0)
                    {
#if LOG_MONKEYS
                    Logger.DebugLine($"\t\tCurrent worry level is divisible by {_testDivisor}");
#endif
                        target = _nextIndexTrue;
                    }
#if LOG_MONKEYS
                else
                {

                    Logger.DebugLine($"\t\tCurrent worry level is not divisible by {_testDivisor}");
                }
                Logger.DebugLine($"\t\tItem with worry level {worryLevel} is thrown to monkey {target}");
#endif
                    monkeys[target]._items.Add(worryLevel);
                }
            }
            _items.Clear();
        }
    }

    internal class MonkeyBuilder2
    {
        private List<long> _startingItems = new();
        private bool _addition;
        private int _operationValue;
        private int _testDivisor;
        private int _nextIndexTrue;
        private int _nextIndexFalse;
        private int _divisor = 1;

        public int Divisor => _divisor;

        public void ParseStartingItems(string line)
        {
            var split = line[18..].Split(',');
            foreach (var item in split)
                _startingItems.Add(int.Parse(item));
        }

        public void ParseOperation(string line)
        {
            _addition = line[23] == '+';
            var operand = line.AsSpan(25);
            if (operand[0] == 'o')
                _operationValue = -1;
            else
                _operationValue = int.Parse(operand);
        }

        public void ParseTest(string line, string trueLine, string falseLine)
        {
            _testDivisor = int.Parse(line.AsSpan(21));
            _nextIndexTrue = int.Parse(trueLine.AsSpan(29));
            _nextIndexFalse = int.Parse(falseLine.AsSpan(30));

            _divisor *= _testDivisor;
        }

        public Monkey2 Build()
        {
            var monkey = new Monkey2(_startingItems, _addition, _operationValue, _testDivisor, _nextIndexTrue, _nextIndexFalse);
            _startingItems = new List<long>();
            _addition = false;
            _operationValue = 0;
            _testDivisor = 0;
            _nextIndexTrue = 0;
            _nextIndexFalse = 0;
            return monkey;
        }
    }
}
