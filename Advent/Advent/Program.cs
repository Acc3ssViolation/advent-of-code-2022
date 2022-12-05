using Advent;

var runner = new Runner();

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

await runner.RunAsync(default);