using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day03_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sum = 0;

            var addedItems = new HashSet<char>();

            foreach (var line in input)
            {
                var firstCompartment = line[..(line.Length / 2)];
                var secondCompartment = line[(line.Length / 2)..];
                
                foreach (var item in secondCompartment)
                {
                    if (!addedItems.Contains(item) && firstCompartment.Contains(item))
                    {
                        sum += GetPriority(item);
                        addedItems.Add(item);
                    }
                }

                addedItems.Clear();
            }

            return sum.ToString();
        }

        private static int GetPriority(char item)
        {
            if (item >= 'a' && item <= 'z')
                return item - 'a' + 1;
            else if (item >= 'A' && item <= 'Z')
                return item - 'A' + 27;
            throw new ArgumentException(null, nameof(item));
        }
    }
}
