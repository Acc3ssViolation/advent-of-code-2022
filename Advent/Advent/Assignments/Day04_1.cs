using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day04_1 : IAssignment
    {
        private record struct SectionRange(int Start, int End)
        {
            public SectionRange(string Start, string End) : this(int.Parse(Start), int.Parse(End))
            { 
            }

            public bool Overlaps(SectionRange other)
            {
                if (other.Start > End)
                    return false;
                if (End < other.Start) 
                    return false;
                return true;
            }

            public bool Contains(SectionRange other)
            {
                if (other.Start >= Start && other.End <= End)
                    return true;
                return false;
            }
        }

        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            var score = 0;

            foreach (var line in input)
            {
                var splitLine = line.Split(',', '-');
                var first = new SectionRange(splitLine[0], splitLine[1]);
                var second = new SectionRange(splitLine[2], splitLine[3]);

                if (first.Contains(second) || second.Contains(first)) 
                {
                    score++;
                }
            }

            return Task.FromResult(score.ToString());
        }
    }
}
