using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var quality = CalculateQualityLevel(blueprint, 24);
                qualitySum += quality;
            }

            return qualitySum.ToString();
        }

        private int CalculateQualityLevel(Blueprint blueprint, int minutesLeft)
        {
            return blueprint.number * minutesLeft;
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
        }
    }
}
