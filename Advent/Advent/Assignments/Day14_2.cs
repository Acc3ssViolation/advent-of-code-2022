//#define PRINT_MAP

namespace Advent.Assignments
{
    internal class Day14_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var (width, height, offsetX) = input.Count < 10 ? (32, 16, 485) : (512, 180, 250);

            var sand = new SandCave(width, height);
            var parser = new SandParser(sand, offsetX);
            for (int i = 0; i < input.Count; i++)
            {
                parser.ParseLine(input[i]);
            }

            // Add the floor
            sand.DrawLine(new Vector2Int(0, parser.maxY + 2), new Vector2Int(width - 1, parser.maxY + 2));

#if PRINT_MAP
            Logger.Line("Start situation");
            sand.Print();
#endif

            var sandCount = 1;
            int sandSpawnX = 500 - offsetX;
            while (sand.SpawnSand(sandSpawnX))
            {
                sandCount++;
            }

#if PRINT_MAP
            Logger.Line("End situation");
            sand.Print();
#endif
            return sandCount.ToString();
        }
    }
}
