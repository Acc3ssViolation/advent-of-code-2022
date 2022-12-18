namespace Advent.Assignments
{
    internal class Day18_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            const int width = 32;
            const int height = 32;
            const int depth = 32;

            const int east = +1;
            const int west = -1;
            const int north = -width;
            const int south = +width;
            const int up = +(width * height);
            const int down = -(width * height);

            var voxels = new bool[width * height * depth];
            var voxelIndexes = new int[input.Count];

            for (int i = 0; i < input.Count; i++)
            {
                var (x, y, z) = ParseLine(input[i]);
                var index = ToIndex(x, y, z);
                voxelIndexes[i] = index;
                voxels[index] = true;
            }

            var sideCount = 0;
            for (int i = 0; i < voxelIndexes.Length; i++)
            {
                var index = voxelIndexes[i];

                // Check neighbours!
                if (!voxels[index + up])
                    sideCount++;
                if (!voxels[index + down])
                    sideCount++;
                if (!voxels[index + north])
                    sideCount++;
                if (!voxels[index + south]) 
                    sideCount++;
                if (!voxels[index + east])
                    sideCount++;
                if (!voxels[index + west]) 
                    sideCount++;
            }

            return sideCount.ToString();
        }

        private static int ToIndex(int x, int y, int z)
        {
            return (z << 10) | (y << 5) | x;
        }

        private static (int, int, int) ParseLine(string line)
        {
            var index = 0;
            var x = ParseUtils.ParseIntPositive(line, ref index);
            index++;
            var y = ParseUtils.ParseIntPositive(line, ref index);
            index++;
            var z = ParseUtils.ParseIntPositive(line, ref index);
            return (x, y, z);
        }
    }
}
