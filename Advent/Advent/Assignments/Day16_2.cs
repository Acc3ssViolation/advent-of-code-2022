namespace Advent.Assignments
{
    internal class Day16_2 : IAssignment
    {
        private record struct State(ulong AvailableValves, int Me, int Elephant, int MeTimeLeft, int ElephantTimeLeft);

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
            var bestScore = GetBestChildScore(network, new State(availableValves, firstValve.Index, firstValve.Index, 26, 26));
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
                ulong valveMaskMe = 1UL << i;
                if ((valveMaskMe & state.AvailableValves) == 0)
                    continue;

                // Check if opening this would make sense
                var openDurationMe = state.MeTimeLeft - network.GetMinutesToOpenValve(state.Me, i);
                if (openDurationMe <= 0)
                {
                    // In this case we should still try with just the elephant
                    var openDurationElephant = state.ElephantTimeLeft - network.GetMinutesToOpenValve(state.Elephant, i);
                    if (openDurationElephant <= 0)
                        continue;

                    var score = openDurationElephant * network.GetRate(i);
                    score += GetBestChildScore(network, state with
                    {
                        AvailableValves = state.AvailableValves & ~valveMaskMe,
                        Elephant = i,
                        ElephantTimeLeft = openDurationElephant,
                    });
                    if (score > bestChildScore)
                        bestChildScore = score;
                }
                else
                {
                    // Find a job for the elephant as well
                    var elephantDidAny = false;
                    for (int k = 0; k < valveCount; k++)
                    {
                        // We can't both go to the same valve
                        if (k == i)
                            continue;

                        ulong valveMaskElephant = 1UL << k;
                        if ((valveMaskElephant & state.AvailableValves) == 0)
                            continue;

                        // Check if opening this would make sense
                        var openDurationElephant = state.ElephantTimeLeft - network.GetMinutesToOpenValve(state.Elephant, k);
                        if (openDurationElephant <= 0)
                            continue;

                        elephantDidAny = true;

                        // i is for me, k is for the elephant
                        var score = openDurationMe * network.GetRate(i);
                        score += openDurationElephant * network.GetRate(k);

                        score += GetBestChildScore(network, state with
                        {
                            AvailableValves = state.AvailableValves & ~(valveMaskMe | valveMaskElephant),
                            Me = i,
                            MeTimeLeft = openDurationMe,
                            Elephant = k,
                            ElephantTimeLeft = openDurationElephant,
                        });
                        if (score > bestChildScore)
                            bestChildScore = score;
                    }

                    if (!elephantDidAny)
                    {
                        // Try with just me
                        var score = openDurationMe * network.GetRate(i);
                        score += GetBestChildScore(network, state with
                        {
                            AvailableValves = state.AvailableValves & ~valveMaskMe,
                            Me = i,
                            MeTimeLeft = openDurationMe,
                        });
                        if (score > bestChildScore)
                            bestChildScore = score;
                    }
                }
            }

            return bestChildScore;
        }
    }
}
