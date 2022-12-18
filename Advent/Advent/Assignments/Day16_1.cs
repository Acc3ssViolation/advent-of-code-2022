using System.Xml.Linq;

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

            // We only care about nodes that are not yet opened
            // We need to know the distances between all of the nodes that can be opened
            var valve = network.GetValve("AA");
            var path = new PathNode(network, valve);

            var score = DoThing(valve, path, 30);

            return score.ToString();
        }

        private static int DoThing(Valve valve, PathNode path, int minutesLeft)
        {
            if (minutesLeft <= 0)
                return path.Score;

            var bestChildScore = 0;

            foreach (var nextValve in path.GetUnopenedValves())
            {
                if (nextValve == valve)
                    continue;

                //Logger.DebugLine($"Checking path from {valve.Name} to unopened valve {nextValve.Name}");

                var distance = valve.RouteLengthTo(nextValve);
                var timeLeftOnceArrived = minutesLeft - distance;

                // Visit a path where we do not open the valve
                var score = DoThing(nextValve, new PathNode(path, timeLeftOnceArrived), timeLeftOnceArrived);
                if (score > bestChildScore)
                    bestChildScore = score;

                // Visit a path where we open the valve now
                score = DoThing(nextValve, new PathNode(path, nextValve, timeLeftOnceArrived - 1), timeLeftOnceArrived - 1);
                if (score > bestChildScore)
                    bestChildScore = score;
            }

            return path.Score + bestChildScore;
        }
    }

    internal class PathNode
    {
        public PathNode? Parent { get; }
        public Valve? OpenedValve { get; }
        public int MinutesLeft { get; }
        public int Score => MinutesLeft * OpenedValve?.Rate ?? 0;

        private readonly ValveNetwork _network;

        public PathNode(ValveNetwork network, Valve valve)
        {
            OpenedValve = valve;
            _network = network;
        }

        public PathNode(PathNode parent, int minutesLeft)
        {
            Parent = parent;
            _network = parent._network;
            MinutesLeft = minutesLeft;
        }

        public PathNode(PathNode parent, Valve? openedValve, int minutesLeft)
        {
            Parent = parent;
            _network = parent._network;
            OpenedValve = openedValve;
            MinutesLeft = minutesLeft;
        }

        public bool IsValveOpened(Valve valve)
        {
            if (valve.Rate == 0)
                return true;

            if (OpenedValve == valve)
                return true;
            return Parent?.IsValveOpened(valve) ?? false;
        }

        public IEnumerable<Valve> GetUnopenedValves()
        {
            return _network.Valves.Where(v => !IsValveOpened(v));
        }
    }

    internal class ValveNetwork
    {
        private readonly Dictionary<string, Valve> _valves = new();

        public void Parse(string input)
        {
            var name = new string(input.AsSpan(6, 2));
            // Valve AA has flow rate=
            var index = 23;
            //
            var rate = ParseUtils.ParseIntPositive(input, ref index);
            Logger.DebugLine($"{input[index]}");
            index += 25;
            Logger.DebugLine($"{input[index]}");
            var valve = CreateOrGetValve(name);

            valve.SetRate(rate);

            while ((index + 1) < input.Length)
            {
                var link = new string(input.AsSpan(index, 2));
                index += 4;
                var linkedValve = CreateOrGetValve(link);
                Logger.DebugLine(link);
                valve.Link(linkedValve);
            }
        }

        public Valve GetValve(string name) => _valves[name];

        public IEnumerable<Valve> Valves => _valves.Values;

        private Valve CreateOrGetValve(string name)
        {
            if (!_valves.TryGetValue(name, out var valve))
            {
                valve = new Valve(name);
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

        public Valve(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
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
