//#define LOG_TIME_ADVANCES

using System.Diagnostics;

namespace Advent.Assignments
{
    internal class Day19_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var length = input.Count;
            if (length > 3)
                length = 3;

            var tasks = new Task<int>[length];
            for (int i = 0; i < length; i++)
            {
                var line = input[i];
                var task = Task.Run(() =>
                {
                    var blueprint = Blueprint.Parse(line);
                    Logger.Line(blueprint.ToString());
                    var quality = CalculateQualityLevel(blueprint, 32);
                    return quality;
                });
                tasks[i] = task;
            }

            var taskResults = Task.WhenAll(tasks).Result;
            var result = taskResults[0];
            for (var i = 1; i < length; i++)
                result *= taskResults[i];

            return result.ToString();
        }

        private static int CalculateQualityLevel(Blueprint blueprint, int minutesLeft)
        {
            var state = new State
            {
                oreRobots = 1,
                minutesLeft = minutesLeft,
                allowedRobots = Resource.Ore | Resource.Clay,
            };

            var maxChildScore = 0;
            var childScore = DoChoice(blueprint, state, Resource.Clay);
            if (childScore > maxChildScore)
                maxChildScore = childScore;
            childScore = DoChoice(blueprint, state, Resource.Ore);
            if (childScore > maxChildScore)
            {
                maxChildScore = childScore;
            }

            Logger.DebugLine($"Blueprint {blueprint.number} can get at most {maxChildScore} geodes");

            return maxChildScore;
        }

        private static bool UpdateState(Blueprint blueprint, ref State state, Resource build)
        {
            switch (build)
            {
                case Resource.Ore:
                    {
                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.oreRobotOreCost);
                        if (AdvanceTime(ref state, oreTimeout + 1))
                        {
                            return true;
                        }
                        state.ore -= blueprint.oreRobotOreCost;
                        state.oreRobots++;
                        if (state.oreRobots * state.minutesLeft + state.ore >= state.minutesLeft * blueprint.maxOreCost)
                            state.bannedRobots |= Resource.Ore;
                    }
                    break;

                case Resource.Clay:
                    {
                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.clayRobotOreCost);
                        if (AdvanceTime(ref state, oreTimeout + 1))
                        {
                            return true;
                        }
                        state.ore -= blueprint.clayRobotOreCost;
                        state.allowedRobots |= Resource.Obsidian;
                        state.clayRobots++;
                        if (state.clayRobots * state.minutesLeft + state.clay >= state.minutesLeft * blueprint.obsidianRobotClayCost)
                            state.bannedRobots |= Resource.Clay;
                    }
                    break;

                case Resource.Obsidian:
                    {
                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.obsidianRobotOreCost);
                        var clayTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Clay, blueprint.obsidianRobotClayCost);
                        var maxTimeout = oreTimeout > clayTimeout ? oreTimeout : clayTimeout;
                        if (AdvanceTime(ref state, maxTimeout + 1))
                        {
                            return true;
                        }
                        state.ore -= blueprint.obsidianRobotOreCost;
                        state.clay -= blueprint.obsidianRobotClayCost;
                        state.allowedRobots |= Resource.Geode;
                        state.obsidianRobots++;
                        if (state.obsidianRobots * state.minutesLeft + state.obsidian >= state.minutesLeft * blueprint.geodeRobotObsidianCost)
                            state.bannedRobots |= Resource.Obsidian;
                    }
                    break;

                case Resource.Geode:
                    {
                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.geodeRobotOreCost);
                        var obsidianTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Obsidian, blueprint.geodeRobotObsidianCost);
                        var maxTimeout = oreTimeout > obsidianTimeout ? oreTimeout : obsidianTimeout;
                        if (AdvanceTime(ref state, maxTimeout + 1))
                        {
                            return true;
                        }
                        state.ore -= blueprint.geodeRobotOreCost;
                        state.obsidian -= blueprint.geodeRobotObsidianCost;
                        state.geodeRobots++;
                    }
                    break;
            }

            // Not yet finished
            return false;
        }

        private static int DoChoice(Blueprint blueprint, State state, Resource build)//, out PathNode path)
        {
            if (UpdateState(blueprint, ref state, build))
                return state.geodes;

            int childScore;
            var maxChildScore = state.geodes;

            var possibleChoices = state.allowedRobots & (~state.bannedRobots);

            if ((possibleChoices & Resource.Geode) > 0)
            {
                childScore = DoChoice(blueprint, state, Resource.Geode);//, out var childPath);
                if (childScore > maxChildScore)
                {
                    maxChildScore = childScore;
                }
            }

            if ((possibleChoices & Resource.Obsidian) > 0)
            {
                childScore = DoChoice(blueprint, state, Resource.Obsidian);//, out var childPath1);
                if (childScore > maxChildScore)
                {
                    maxChildScore = childScore;
                }
            }

            if ((possibleChoices & Resource.Clay) > 0)
            {
                childScore = DoChoice(blueprint, state, Resource.Clay);//, out var childPath2);
                if (childScore > maxChildScore)
                {
                    maxChildScore = childScore;
                }
            }

            if ((possibleChoices & Resource.Ore) > 0)
            {
                childScore = DoChoice(blueprint, state, Resource.Ore);//, out var childPath3);
                if (childScore > maxChildScore)
                {
                    maxChildScore = childScore;
                }
            }

            return maxChildScore;
        }

        private static int GetMinutesUntilResourceIsAtLevel(ref State state, Resource resource, int level)
        {
            var (rate, current) = resource switch
            {
                Resource.Ore => (state.oreRobots, state.ore),
                Resource.Clay => (state.clayRobots, state.clay),
                Resource.Obsidian => (state.obsidianRobots, state.obsidian),
                Resource.Geode => (state.geodeRobots, state.geodes),
                _ => (0, 0),
            };

            if (level <= current)
                return 0;

            var minutes = (level - current - 1) / rate + 1;
            return minutes >= 0 ? minutes : 0;
        }

        private static bool AdvanceTime(ref State state, int minutes)
        {
            var minutesToAdvance = state.minutesLeft < minutes ? state.minutesLeft : minutes;
            state.minutesLeft -= minutesToAdvance;

            state.ore += state.oreRobots * minutesToAdvance;
            state.clay += state.clayRobots * minutesToAdvance;
            state.obsidian += state.obsidianRobots * minutesToAdvance;
            state.geodes += state.geodeRobots * minutesToAdvance;

#if LOG_TIME_ADVANCES
            Logger.DebugLine($"[{state.minutesLeft}] Over {minutesToAdvance} minutes, {state.oreRobots} ore robots collect {state.oreRobots * minutesToAdvance} ore; you now have {state.ore}");
            Logger.DebugLine($"[{state.minutesLeft}] Over {minutesToAdvance} minutes, {state.clayRobots} clay robots collect {state.clayRobots * minutesToAdvance} clay; you now have {state.clay}");
            Logger.DebugLine($"[{state.minutesLeft}] Over {minutesToAdvance} minutes, {state.obsidianRobots} obsidian robots collect {state.obsidianRobots * minutesToAdvance} obsidian; you now have {state.obsidian}");
            Logger.DebugLine($"[{state.minutesLeft}] Over {minutesToAdvance} minutes, {state.geodeRobots} geode robots collect {state.geodeRobots * minutesToAdvance} geodes; you now have {state.geodes}");
#endif
            return state.minutesLeft == 0;
        }

        private enum Resource
        {
            Ore = 1,
            Clay = 2,
            Obsidian = 4,
            Geode = 8,
        }

        private struct State
        {
            public int minutesLeft;
            public int ore;
            public int clay;
            public int obsidian;
            public int geodes;

            public int oreRobots;
            public int clayRobots;
            public int obsidianRobots;
            public int geodeRobots;

            public Resource allowedRobots;
            public Resource bannedRobots;

            public override string ToString()
            {
                return $"After minute {32 - minutesLeft}: Resources {ore},{clay},{obsidian},{geodes}; robots {oreRobots},{clayRobots},{obsidianRobots},{geodeRobots}";
            }
        }

        private class Blueprint
        {
            public int number;

            public int oreRobotOreCost;

            public int clayRobotOreCost;

            public int obsidianRobotOreCost;
            public int obsidianRobotClayCost;

            public int geodeRobotOreCost;
            public int geodeRobotObsidianCost;

            public int maxOreCost;

            public static Blueprint Parse(string input)
            {
                Blueprint blueprint = new();

                var index = 10;
                blueprint.number = ParseUtils.ParseIntPositive(input, ref index);

                index += 23;
                blueprint.oreRobotOreCost = ParseUtils.ParseIntPositive(input, ref index);

                index += 28;
                blueprint.clayRobotOreCost = ParseUtils.ParseIntPositive(input, ref index);

                index += 32;
                blueprint.obsidianRobotOreCost = ParseUtils.ParseIntPositive(input, ref index);
                index += 9;
                blueprint.obsidianRobotClayCost = ParseUtils.ParseIntPositive(input, ref index);

                index += 30;
                blueprint.geodeRobotOreCost = ParseUtils.ParseIntPositive(input, ref index);
                index += 9;
                blueprint.geodeRobotObsidianCost = ParseUtils.ParseIntPositive(input, ref index);

                blueprint.maxOreCost = blueprint.oreRobotOreCost;
                if (blueprint.clayRobotOreCost > blueprint.maxOreCost)
                    blueprint.maxOreCost = blueprint.clayRobotOreCost;
                if (blueprint.obsidianRobotOreCost > blueprint.maxOreCost)
                    blueprint.maxOreCost = blueprint.obsidianRobotOreCost;
                if (blueprint.geodeRobotOreCost > blueprint.maxOreCost)
                    blueprint.maxOreCost = blueprint.geodeRobotOreCost;

                return blueprint;
            }

            public override string ToString()
            {
                return $"Blueprint {number}: Each ore robot costs {oreRobotOreCost} ore. Each clay robot costs {clayRobotOreCost} ore. Each obsidian robot costs {obsidianRobotOreCost} ore and {obsidianRobotClayCost} clay. Each geode robot costs {geodeRobotOreCost} ore and {geodeRobotObsidianCost} obsidian.";
            }
        }
    }
}
