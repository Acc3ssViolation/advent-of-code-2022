namespace Advent.Assignments
{
    internal class Day16_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var network = new ValveNetwork();
            foreach (var line in input)
            {
                network.Parse(line);
            }

            // Some prep work, cache all paths from any valve to any other valve
            network.CalculateAllPaths();

            ulong availableValves = 0UL;
            foreach (var valve in network.Valves)
            {
                if (valve.Rate == 0)
                    continue;
                availableValves |= 1UL << valve.Index;
            }

            // We only care about nodes that are not yet opened
            // We need to know the distances between all of the nodes that can be opened
            var firstValve = network.GetValve("AA");

            var bestScore = GetBestChildScore(network, availableValves, firstValve.Index, 30);
            return bestScore.ToString();
        }

        private static int GetBestChildScore(ValveNetwork network, ulong availableValves, int currentValve, int timeLeft)
        {
            if (availableValves == 0)
                return 0;

            int bestChildScore = 0;

            var valveCount = network.Count;
            for (int i = 0; i < valveCount; i++)
            {
                if (i == currentValve)
                    continue;

                ulong valveMask = 1UL << i;
                if ((valveMask & availableValves) == 0)
                    continue;

                // What would be the score of opening this valve?
                var openDuration = timeLeft - network.GetMinutesToOpenValve(currentValve, i);
                if (openDuration <= 0)
                    continue;

                var score = openDuration * network.GetRate(i);
                score += GetBestChildScore(network, availableValves & ~valveMask, i, openDuration);
                if (score > bestChildScore)
                    bestChildScore = score;
            }

            return bestChildScore;
        }
    }

    internal class ValveNetwork
    {
        private readonly Dictionary<string, Valve> _valves = new();

        private readonly Dictionary<int, int> _pathLengths = new();

        private int[]? _rates;

        public int Count => _valves.Count;

        public void Parse(string input)
        {
            var name = new string(input.AsSpan(6, 2));
            // Valve AA has flow rate=
            var index = 23;
            //
            var rate = ParseUtils.ParseIntPositive(input, ref index);
            index += 25;
            var valve = CreateOrGetValve(name);

            valve.SetRate(rate);

            while ((index + 1) < input.Length)
            {
                var link = new string(input.AsSpan(index, 2));
                index += 4;
                var linkedValve = CreateOrGetValve(link);
                valve.Link(linkedValve);
            }
        }

        public int GetMinutesToOpenValve(int currentValve, int targetValve)
        {
            return _pathLengths[GetPathIndex(currentValve, targetValve)];
        }

        private static int GetPathIndex(int valveA, int valveB)
        {
            // We only store paths once, with the lowest index as 'from' and the highest as 'to'
            var (from, to) = valveA < valveB ? (valveA, valveB) : (valveB, valveA);
            return from | (to << 16);
        }

        public void CalculateAllPaths()
        {
            _rates = new int[_valves.Count];

            foreach (var valve in _valves.Values)
            {
                _rates[valve.Index] = valve.Rate;

                // AA is special since it's the starting valve. We need to know the paths from it even though it cannot be opened.
                if (valve.Rate == 0 && valve.Name != "AA")
                    continue;

                foreach (var connection in _valves.Values)
                {
                    if (connection == valve)
                        continue;
                    if (connection.Rate == 0)
                        continue;

                    var pathIndex = GetPathIndex(valve.Index, connection.Index);
                    if (!_pathLengths.ContainsKey(pathIndex))
                    {
                        // Calculate this path!
                        var pathLength = valve.RouteLengthTo(connection) + 1;
                        _pathLengths[pathIndex] = pathLength;
                    }
                }
            }
        }

        public int GetRate(int index) => _rates![index];

        public Valve GetValve(string name) => _valves[name];

        public IEnumerable<Valve> Valves => _valves.Values;

        private Valve CreateOrGetValve(string name)
        {
            if (!_valves.TryGetValue(name, out var valve))
            {
                valve = new Valve(name, _valves.Count);
                _valves[name] = valve;
            }
            return valve;
        }
    }

    internal class Valve
    {
        private readonly List<Valve> _tunnels = new();
        private int _rate;

        public string Name { get; }
        public int Rate => _rate;
        public IReadOnlyList<Valve> Tunnels => _tunnels;
        public int Index { get; }

        public Valve(string name, int index)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Index = index;
        }

        public void SetRate(int rate) => _rate = rate;

        public void Link(Valve other)
        {
            if (!_tunnels.Contains(other))
                _tunnels.Add(other);
            if (!other._tunnels.Contains(this))
                other._tunnels.Add(this);
        }

        public int RouteLengthTo(Valve other)
        {
            var visited = new HashSet<Valve>();
            var toVisit = new Queue<Valve>();
            toVisit.Enqueue(this);
            var parents = new Dictionary<Valve, Valve>();
            var found = false;
            while (!found)
            {
                var current = toVisit.Dequeue();

                if (current == other)
                    break;

                foreach (var neighbour in current._tunnels)
                {
                    if (!visited.Contains(neighbour))
                    {
                        visited.Add(neighbour);
                        toVisit.Enqueue(neighbour);
                        parents.Add(neighbour, current);
                        if (neighbour == other)
                        {
                            found = true;
                            break;
                        }
                    }
                }
            }

            var length = 0;
            var node = other;
            while (node != this)
            {
                node = parents[node];
                length++;
            }

            //Logger.DebugLine($"Distance from {Name} to {other.Name} is {length}");

            return length;
        }

        public override string ToString()
        {
            return $"Valve {Name} has flow rate={Rate}; tunnels lead to valves {_tunnels.AggregateString(v => v.Name)}";
        }
    }
}
