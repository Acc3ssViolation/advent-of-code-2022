using Advent;
using Advent.Assignments;

var runner = new Runner();
runner.Add(new Day01_1());
runner.Add(new Day01_2());

await runner.RunAsync(default);