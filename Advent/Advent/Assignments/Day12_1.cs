namespace Advent.Assignments
{
    internal class Day12_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {            
            var width = input[0].Length;
            var height = input.Count;

            var map = new byte[width * height];
            var startNodeIndex = 0;
            var endNodeIndex = 0;

            // Load map
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var chr = input[i][j];
                    if (chr == 'S')
                    {
                        startNodeIndex = i * width + j;
                        chr = 'a';
                    }
                    else if (chr == 'E')
                    {
                        endNodeIndex = i * width + j;
                        chr = 'z';
                    }
                    map[i * width + j] = (byte)(chr - 'a');
                    
                }
            }

            // Not sure if this is the best way of going about it, but it works
            var ring = new RingBuffer(width * height);
            var visited = new IntSet(width* height);
            var parents = new int[width * height];

            // Breath First Search
            var current = startNodeIndex;
            visited.Add(current);
            int shortestDistance = 0;
            while (true)
            {
                if (current == endNodeIndex)
                {
                    // Found it!
                    shortestDistance = 1;
                    while ((current = parents[current]) != startNodeIndex)
                        shortestDistance++;
                    break;
                }

                var x = current % width;
                var y = current / width;
                var maxValue = map[current] + 1;

                //Logger.DebugLine($"[{x},{y}]");

                // Left
                if (x > 0)
                {
                    var left = x - 1 + y * width;
                    var value = map[left];
                    if (value <= maxValue && !visited.Includes(left))
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
                    if (value <= maxValue && !visited.Includes(right))
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
                    if (value <= maxValue && !visited.Includes(up))
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
                    if (value <= maxValue && !visited.Includes(down))
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

        private class RingBuffer
        {
            private int _read;
            private int _write;
            private int[] _buffer;
            
            public RingBuffer(int size)
            {
                _buffer = new int[size];
            }

            public void Enqueue(int value)
            {
                _buffer[_write] = value;
                _write = (_write + 1) % _buffer.Length;
            }

            public int Dequeue()
            {
                var result = _buffer[_read];
                _read = (_read + 1) % _buffer.Length;
                return result;
            }
        }
    }
}
