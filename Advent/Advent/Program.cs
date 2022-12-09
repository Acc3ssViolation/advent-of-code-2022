using Advent;

var runner = new Runner();

Logger.SetLevel(LogLevel.Info);

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

runner.LogTimingToFile = true;
runner.Prepare();

await runner.RunTestsAsync(default);

Console.WriteLine("Press any key to continue...");
try { Console.Read(); } catch { return; }

await runner.RunAsync(default);