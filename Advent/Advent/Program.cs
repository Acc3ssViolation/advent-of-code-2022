//#define AUTO_LOAD_ASSIGNMENTS
using Advent;
using Advent.Assignments;

var runner = new Runner();

const int Iterations = 1;
//Logger.SetLevel(LogLevel.Info);

#if AUTO_LOAD_ASSIGNMENTS
var interfaceType = typeof(IAssignment);
    var all = AppDomain.CurrentDomain.GetAssemblies()
      .SelectMany(x => x.GetTypes())
      .Where(x => interfaceType.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
      .Select(x => Activator.CreateInstance(x));

    foreach (var item in all)
    {
        if (item is IAssignment assignment)
            runner.Add(assignment);
    }
#else
//runner.Add(new Day01_1());
//runner.Add(new Day01_2());
//runner.Add(new Day02_1());
//runner.Add(new Day02_2());
//runner.Add(new Day03_1());
//runner.Add(new Day03_2());
//runner.Add(new Day04_1());
//runner.Add(new Day04_2());
//runner.Add(new Day05_1());
//runner.Add(new Day05_2());
//runner.Add(new Day06_1());
//runner.Add(new Day06_2());
//runner.Add(new Day07_1());
//runner.Add(new Day07_2());
//runner.Add(new Day08_1());
//runner.Add(new Day08_2());
//runner.Add(new Day09_1());
//runner.Add(new Day09_2());
//runner.Add(new Day10_1());
//runner.Add(new Day10_2());
//runner.Add(new Day11_1());
//runner.Add(new Day11_2());
//runner.Add(new Day12_1());
//runner.Add(new Day12_2());
//runner.Add(new Day13_1());
//runner.Add(new Day13_2());
//runner.Add(new Day14_1());
//runner.Add(new Day14_2());
//runner.Add(new Day15_1());
//runner.Add(new Day15_2());
//runner.Add(new Day16_1());
//runner.Add(new Day16_2());
//runner.Add(new Day17_1());
//runner.Add(new Day17_2());
//runner.Add(new Day18_1());
//runner.Add(new Day18_2());
//runner.Add(new Day19_1());
//runner.Add(new Day19_2());
runner.Add(new Day20_1());
#endif

runner.LogTimingToFile = true;
runner.Prepare();

await runner.RunTestsAsync(default);

Console.WriteLine("Press any key to continue...");
try { Console.Read(); } catch { return; }

for (var i = 0; i < Iterations; i++)
    await runner.RunAsync(default);