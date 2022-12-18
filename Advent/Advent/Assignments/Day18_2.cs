using System;

namespace Advent.Assignments
{
    internal class Day18_2 : IAssignment
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

            var visitedVoxels = new bool[width * height * depth];
            var queue = new BoundedRingBuffer(width * height * depth);
            queue.Enqueue(0);
            var sideCount = 0;

            while (queue.count > 0)
            {
                var index = queue.Dequeue();

                void ProcessPosition(int checkIndex)
                {
                    if (checkIndex < 0 || checkIndex >= voxels.Length)
                        return;

                    if (!visitedVoxels[checkIndex])
                    {
                        if (voxels[checkIndex])
                        {
                            // We can see this voxel's side
                            sideCount++;
                        }
                        else
                        {
                            // Empty space, add it to the queue
                            queue.Enqueue(checkIndex);
                            visitedVoxels[checkIndex] = true;
                        }
                    }
                }

                ProcessPosition(index + up);
                ProcessPosition(index + down);
                ProcessPosition(index + north);
                ProcessPosition(index + south);
                ProcessPosition(index + east);
                ProcessPosition(index + west);
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

    internal class BoundedRingBuffer
    {
        public int count;

        private int _read;
        private int _write;
        private int[] _buffer;

        public BoundedRingBuffer(int size)
        {
            _buffer = new int[size];
        }

        public void Enqueue(int value)
        {
            if (count < _buffer.Length)
            {
                _buffer[_write] = value;
                _write = (_write + 1) % _buffer.Length;
                count++;
            }
            else
            {
                ThrowFull();
            }
        }

        public int Dequeue()
        {
            var result = _buffer[_read];
            _read = (_read + 1) % _buffer.Length;
            count--;
            return result;
        }

        private static void ThrowFull() => throw new Exception("Queue is empty");
    }
}
