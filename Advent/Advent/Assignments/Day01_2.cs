namespace Advent.Assignments
{
    internal class Day01_2 : IAssignment
    {
        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            var calories = new List<int>();
            var sum = 0;

            foreach (var item in input)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    calories.Add(sum);
                    sum = 0;
                }
                else
                {
                    sum += int.Parse(item);
                }
            }

            // Add last entry
            calories.Add(sum);

            calories.Sort();

            var lastIndex = calories.Count - 1;
            var total = calories[lastIndex] + calories[lastIndex - 1] + calories[lastIndex - 2];

            return Task.FromResult(total.ToString());
        }
    }
}
