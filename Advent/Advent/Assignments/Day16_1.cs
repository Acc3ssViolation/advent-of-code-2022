namespace Advent.Assignments
{
    internal class Day16_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var data = "Valve AA has flow rate=0; tunnels lead to valves DD, II, BB";
            var span = data.AsSpan(data.IndexOf('=') + 1);
            var num = int.Parse(span);
            return num.ToString();
        }
    }

    internal class ValveNetwork
    {
        private readonly Dictionary<string, Valve> _valves = new();

        public void Parse(string input)
        {

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
    }
}
