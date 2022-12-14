namespace Advent.Assignments
{
    internal class Day12_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var width = input[0].Length;
            var height = input.Count;

            var map = new byte[width * height];
            var startNodeIndex = 0;

            // Load map
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var chr = input[i][j];
                    if (chr == 'S')
                    {
                        chr = 'a';
                    }
                    else if (chr == 'E')
                    {
                        startNodeIndex = i * width + j;
                        chr = 'z';
                    }
                    map[i * width + j] = (byte)(chr - 'a');

                }
            }

            // Not sure if this is the best way of going about it, but it works
            var ring = new RingBuffer(width * height);
            var visited = new IntSet(width * height);
            var parents = new int[width * height];

            // Breath First Search
            var current = startNodeIndex;
            visited.Add(current);
            int shortestDistance = 0;
            while (true)
            {
                if (map[current] == 0)
                {
                    // Found it!
                    shortestDistance = 1;
                    while ((current = parents[current]) != startNodeIndex)
                        shortestDistance++;
                    break;
                }

                var x = current % width;
                var y = current / width;
                var minValue = map[current] - 1;

                //Logger.DebugLine($"[{x},{y}]");

                // Left
                if (x > 0)
                {
                    var left = x - 1 + y * width;
                    var value = map[left];
                    if (value >= minValue && !visited.Includes(left))
                    {
                        parents[left] = current;
                        visited.Add(left);
                        ring.Enqueue(left);
                    }
                }

                // Right
                if (x < width - 1)
                {
                    var right = x + 1 + y * width;
                    var value = map[right];
                    if (value >= minValue && !visited.Includes(right))
                    {
                        parents[right] = current;
                        visited.Add(right);
                        ring.Enqueue(right);
                    }
                }

                // Up
                if (y > 0)
                {
                    var up = x + (y - 1) * width;
                    var value = map[up];
                    if (value >= minValue && !visited.Includes(up))
                    {
                        parents[up] = current;
                        visited.Add(up);
                        ring.Enqueue(up);
                    }
                }

                // Down
                if (y < height - 1)
                {
                    var down = x + (y + 1) * width;
                    var value = map[down];
                    if (value >= minValue && !visited.Includes(down))
                    {
                        parents[down] = current;
                        visited.Add(down);
                        ring.Enqueue(down);
                    }
                }

                shortestDistance++;

                current = ring.Dequeue();
            }

            return shortestDistance.ToString();
        }
    }
}
