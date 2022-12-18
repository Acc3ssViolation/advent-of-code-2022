namespace Advent.Assignments
{
    internal class Day16_2 : IAssignment
    {
        private record struct State(ulong AvailableValves, int Me, int Elephant, int MeDuration, int ElephantDuration, int TimeLeft);

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

            
            var firstValve = network.GetValve("AA");

            // TODO: How the fuck do we simulate two workers at once?!
            var bestScore = GetBestChildScore(network, new State(availableValves, firstValve.Index, firstValve.Index, 0, 0, 26));
            return bestScore.ToString();
        }

        private static int GetBestChildScore(ValveNetwork network, State state)
        {
            if (state.AvailableValves == 0)
                return 0;

            int bestChildScore = 0;

            var valveCount = network.Count;
            for (int i = 0; i < valveCount; i++)
            {
                if (i == state.Me || i == state.Elephant)
                    continue;

                ulong valveMask = 1UL << i;
                if ((valveMask & state.AvailableValves) == 0)
                    continue;

                // What would be the score of opening this valve?
                {
                    var openDuration = state.TimeLeft - network.GetMinutesToOpenValve(state.Me, i);
                    if (openDuration <= 0)
                        continue;

                    var score = openDuration * network.GetRate(i);
                    score += GetBestChildScore(network, state with
                    {
                        AvailableValves = state.AvailableValves & ~valveMask,
                        Me = i,
                        TimeLeft = openDuration
                    });
                    if (score > bestChildScore)
                        bestChildScore = score;
                }
            }

            return bestChildScore;
        }
    }
}
