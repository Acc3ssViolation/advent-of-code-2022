﻿namespace Advent.Assignments
{
    internal class Day01_1 : IAssignment
    {
        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            var highestSum = 0;
            var sum = 0;

            foreach (var item in input)
            {
                if (string.IsNullOrWhiteSpace(item))
                {
                    if (sum > highestSum)
                    {
                        highestSum = sum;
                    }
                    sum = 0;
                }
                else
                {
                    sum += int.Parse(item);
                }
            }

            return Task.FromResult(highestSum.ToString());
        }
    }
}
