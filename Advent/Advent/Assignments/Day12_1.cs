namespace Advent.Assignments
{
    internal class Day12_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {            
            var width = input[0].Length;
            var height = input.Count;

            var map = new byte[width * height];
            var distances = new int[width * height];
            var visited = new IntSet(width * height);
            var startNode = new Vector2Int();
            var endNodeIndex = 0;

            // Load map
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    distances[i * width + j] = int.MaxValue;

                    var chr = input[i][j];
                    if (chr == 'S')
                    {
                        startNode.x = j;
                        startNode.y = i;
                        chr = 'a';
                        distances[i * width + j] = 0;
                    }
                    else if (chr == 'E')
                    {
                        endNodeIndex = i * width + j;
                        chr = 'z';
                    }
                    map[i * width + j] = (byte)(chr - 'a');
                    
                }
            }

            // Dijkstraaaaa
            var x = startNode.x;
            var y = startNode.y;
            int shortestDistance;
            while (true)
            {
                // Distance of current node
                var current = x + y * width;
                var maxValue = map[current] + 1;
                var nextDistance = distances[current] + 1;

                // Left
                if (x > 0)
                {
                    var left = x - 1 + y * width;
                    var value = map[left];
                    if (value <= maxValue)
                    {

                        if (!visited.Includes(left))
                        {
                            if (distances[left] > nextDistance)
                                distances[left] = nextDistance;
                        }
                    }
                }

                // Right
                if (x < width - 1)
                {
                    var right = x + 1 + y * width;
                    var value = map[right];
                    if (value <= maxValue)
                    {

                        if (!visited.Includes(right))
                        {
                            if (distances[right] > nextDistance)
                                distances[right] = nextDistance;
                        }
                    }
                }

                // Up
                if (y > 0)
                {
                    var up = x + (y - 1) * width;
                    var value = map[up];
                    if (value <= maxValue)
                    {

                        if (!visited.Includes(up))
                        {
                            if (distances[up] > nextDistance)
                                distances[up] = nextDistance;
                        }
                    }
                }

                // Down
                if (y < height - 1)
                {
                    var down = x + (y + 1) * width;
                    var value = map[down];
                    if (value <= maxValue)
                    {

                        if (!visited.Includes(down))
                        {
                            if (distances[down] > nextDistance)
                                distances[down] = nextDistance;
                        }
                    }
                }

                // Mark current node as visited
                visited.Add(current);

                // Continue with node that has the smallest distance
                // TODO: Optimizeee
                var minValue = int.MaxValue;
                var minIndex = 0;
                for (int i = 0; i < width * height; i++)
                {
                    if (!visited.Includes(i) && distances[i] < minValue)
                    {
                        minValue = distances[i];
                        minIndex = i;
                    }
                }

                if (minValue == int.MaxValue)
                {
                    PrintDistances(distances, map, width, height);
                    ThrowOnFailure();
                }

                if (minIndex == endNodeIndex)
                {
                    shortestDistance = minValue;
                    break;
                }

                x = minIndex % width;
                y = minIndex / width;
            }

            //PrintDistances(distances, map, width, height);

            return shortestDistance.ToString();
        }

        private static void ThrowOnFailure() => throw new Exception("Unable to find path");

        private static void PrintDistances(int[] distances, byte[] map, int width, int height)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var distance = distances[x + y * width];
                    if (distance > 999)
                        distance = 0;
                    Logger.Append(((char)(map[x + y * width] + 'a')).ToString());
                    Logger.Append(" ");
                    Logger.Append(distance.ToString("D03"));
                    Logger.Append("|");
                }
                Logger.Line();
            }
        }
    }
}
