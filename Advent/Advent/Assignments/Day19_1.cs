//#define LOG_TIME_ADVANCES

namespace Advent.Assignments
{
    internal class Day19_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var qualitySum = 0;

            for (int i = 0; i < input.Count; i++)
            {
                var blueprint = Blueprint.Parse(input[i]);
                Logger.Line(blueprint.ToString());
                var quality = CalculateQualityLevel(blueprint, 24);
                qualitySum += quality;
            }

            return qualitySum.ToString();
        }

        private static int CalculateQualityLevel(Blueprint blueprint, int minutesLeft)
        {
            var state = new State
            {
                oreRobots = 1,
                minutesLeft = minutesLeft,
            };

            var maxChildScore = 0;
            var childScore = DoChoice(blueprint, state, Resource.Clay, out var childPath);
            if (childScore > maxChildScore)
                maxChildScore = childScore;
            childScore = DoChoice(blueprint, state, Resource.Ore, out var childPath1);
            if (childScore > maxChildScore)
            {
                maxChildScore = childScore;
                childPath = childPath1;
            }

            Logger.DebugLine($"Blueprint {blueprint.number} can get at most {maxChildScore} geodes");

            do
            {
                Logger.DebugLine(childPath.startState.ToString());
                Logger.WarningLine($"Build {childPath.buildCommand} robot");
                Logger.DebugLine(childPath.finalState.ToString());
            } while ((childPath = childPath.child) != null);

            return blueprint.number * maxChildScore;
        }

        private class PathNode
        {
            public PathNode? child;
            public Resource buildCommand;
            public State startState;
            public State finalState;
        }

        private static int DoChoice(Blueprint blueprint, State state, Resource build, out PathNode path)
        {
            path = new PathNode { buildCommand = build, startState = state};
            switch (build)
            {
                case Resource.Ore:
                    {
                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.oreRobotOreCost);
                        if (AdvanceTime(ref state, oreTimeout + 1))
                        {
                            path.finalState = state;
                            return state.geodes;
                        }
                        state.ore -= blueprint.oreRobotOreCost;
                        state.oreRobots++;
                    }
                    break;

                case Resource.Clay:
                    {
                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.clayRobotOreCost);
                        if (AdvanceTime(ref state, oreTimeout + 1))
                        {
                            path.finalState = state;
                            return state.geodes;
                        }
                        state.ore -= blueprint.clayRobotOreCost;
                        state.clayRobots++;
                    }
                    break;

                case Resource.Obisian:
                    {
                        if (state.clayRobots == 0)
                        {
                            path.finalState = state;
                            return state.geodes;
                        }

                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.obsidianRobotOreCost);
                        var clayTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Clay, blueprint.obsidianRobotClayCost);
                        var maxTimeout = oreTimeout > clayTimeout ? oreTimeout : clayTimeout;
                        if (AdvanceTime(ref state, maxTimeout + 1))
                        {
                            path.finalState = state;
                            return state.geodes;
                        }
                        state.ore -= blueprint.obsidianRobotOreCost;
                        state.clay -= blueprint.obsidianRobotClayCost;
                        state.obsidianRobots++;
                    }
                    break;

                case Resource.Geode:
                    {
                        if (state.obsidianRobots == 0)
                        {
                            path.finalState = state;
                            return state.geodes;
                        }

                        var oreTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Ore, blueprint.geodeRobotOreCost);
                        var obsidianTimeout = GetMinutesUntilResourceIsAtLevel(ref state, Resource.Obisian, blueprint.geodeRobotObsidianCost);
                        var maxTimeout = oreTimeout > obsidianTimeout ? oreTimeout : obsidianTimeout;
                        if (AdvanceTime(ref state, maxTimeout + 1))
                        {
                            path.finalState = state;
                            return state.geodes;
                        }
                        state.ore -= blueprint.geodeRobotOreCost;
                        state.obsidian -= blueprint.geodeRobotObsidianCost;
                        state.geodeRobots++;
                    }
                    break;
            }

            var childScore = DoChoice(blueprint, state, Resource.Geode, out var childPath);
            var maxChildScore = childScore;

            childScore = DoChoice(blueprint, state, Resource.Obisian, out var childPath1);
            if (childScore > maxChildScore)
            {
                maxChildScore = childScore;
                childPath = childPath1;
            }
            childScore = DoChoice(blueprint, state, Resource.Clay, out var childPath2);
            if (childScore > maxChildScore)
            {
                maxChildScore = childScore;
                childPath = childPath2;
            }
            childScore = DoChoice(blueprint, state, Resource.Ore, out var childPath3);
            if (childScore > maxChildScore)
            {
                maxChildScore = childScore;
                childPath = childPath3;
            }

            path.child = childPath;
            path.finalState = state;
            return maxChildScore;
        }

        private static int GetMinutesUntilResourceIsAtLevel(ref State state, Resource resource, int level)
        {
            var (rate, current) = resource switch
            {
                Resource.Ore => (state.oreRobots, state.ore),
                Resource.Clay => (state.clayRobots, state.clay),
                Resource.Obisian => (state.obsidianRobots, state.obsidian),
                Resource.Geode => (state.geodeRobots, state.geodes),
                _ => (0, 0),
            };
            var minutes = (level - 1 - current) / rate + 1;
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
            Ore = 0,
            Clay = 1,
            Obisian = 2,
            Geode = 3,
            None = 4,
        }

        private struct State
        {
            // TODO: Use bytes if size is a concern
            public int minutesLeft;
            public int ore;
            public int clay;
            public int obsidian;
            public int geodes;

            public int oreRobots;
            public int clayRobots;
            public int obsidianRobots;
            public int geodeRobots;

            public override string ToString()
            {
                return $"[{25 - minutesLeft}] ore {ore}+{oreRobots}, clay {clay}+{clayRobots}, obsidian {obsidian}+{obsidianRobots}, geodes {geodes}+{geodeRobots}";
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

                return blueprint;
            }

            public override string ToString()
            {
                return $"Blueprint {number}: Each ore robot costs {oreRobotOreCost} ore. Each clay robot costs {clayRobotOreCost} ore. Each obsidian robot costs {obsidianRobotOreCost} ore and {obsidianRobotClayCost} clay. Each geode robot costs {geodeRobotOreCost} ore and {geodeRobotObsidianCost} obsidian.";
            }
        }
    }
}
