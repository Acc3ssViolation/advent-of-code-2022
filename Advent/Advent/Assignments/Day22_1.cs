using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day22_1 : IAssignment
    {
        private class World<T>
        {
            public int ChunkSize { get; }
            public int WorldSize { get; }
            public T[][] Chunks { get; } 

            public World(int chunkSize, int worldSizeInChunks)
            {
                ChunkSize = chunkSize;
                WorldSize = worldSizeInChunks;
                Chunks = new T[worldSizeInChunks * worldSizeInChunks][];
            }

            public int GetChunkIndex(int x, int y)
            {
                var chunkX = x / ChunkSize;
                var chunkY = y / ChunkSize;
                return chunkX + chunkY * WorldSize;
            }

            public int GetLocalIndex(int x, int y)
            {
                var localX = x % ChunkSize;
                var localY = y % ChunkSize;
                return localX + localY * ChunkSize;
            }

            public void Set(int x, int y, T value)
            {
                var chunkIndex = GetChunkIndex(x, y);
                var chunk = Chunks[chunkIndex];
                if (chunk == null)
                {
                    chunk = new T[ChunkSize * ChunkSize];
                    Chunks[chunkIndex] = chunk;
                }
                var localIndex = GetLocalIndex(x, y);
                chunk[localIndex] = value;
            }

            public bool TryGet(int x, int y, out T? data)
            {
                var chunkIndex = GetChunkIndex(x, y);
                var chunk = Chunks[chunkIndex];
                if (chunk == null)
                {
                    data = default;
                    return false;
                }

                data = chunk[GetLocalIndex(x, y)];
                return true;
            }

            public string PrintMap()
            {
                var maxCoordinate = WorldSize * ChunkSize;
                // Assume that newlines take 1 char
                var sb = new StringBuilder(maxCoordinate * (maxCoordinate + 1));

                for (var y = 0; y < maxCoordinate; y++)
                {
                    for (var x = 0; x < maxCoordinate; x++)
                    {
                        var chr = ' ';
                        if (TryGet(x, y, out var data))
                        {
                            if (object.Equals(data, default(T)))
                                chr = '.';
                            else
                                chr = '#';
                        }
                        sb.Append(chr);
                    }
                    sb.Append('\n');
                }

                return sb.ToString();
            }
        }

        public string Run(IReadOnlyList<string> input)
        {
            // Example data uses 4x4 chunks, real data uses 50x50
            var chunkSize = input.Count > 20 ? 50 : 4;
            var world = new World<bool>(chunkSize, 4);

            for (var y = 0; y < input.Count - 2; y++)
            {
                var line = input[y];

                for (var x = 0; x < line.Length; x++)
                {
                    var chr = line[x];
                    if (chr == '.')
                        world.Set(x, y, false);
                    else if (chr == '#')
                        world.Set(x, y, true);
                }
            }

            var movement = input[input.Count - 1];

            return "";
        }
    }
}
