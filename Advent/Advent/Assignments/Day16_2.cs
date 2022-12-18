namespace Advent.Assignments
{
    internal class Day16_2 : IAssignment
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

            
            var firstValve = network.GetValve("AA");

            // TODO: How the fuck do we simulate two workers at once?!
            var bestScore = GetBestChildScore(network, availableValves, firstValve.Index, 26);
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
}
