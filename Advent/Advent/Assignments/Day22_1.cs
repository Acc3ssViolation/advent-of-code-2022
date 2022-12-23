using System.Text;

namespace Advent.Assignments
{
    internal class Day22_1 : IAssignment
    {
        private class World<T>
        {
            public int ChunkSize { get; }
            public int WorldSize { get; }
            public int WorldSizeInTiles { get; }
            public T[][] Chunks { get; } 

            public World(int chunkSize, int worldSizeInChunks)
            {
                ChunkSize = chunkSize;
                WorldSize = worldSizeInChunks;
                WorldSizeInTiles = worldSizeInChunks * chunkSize;
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
                data = default;

                if (y < 0 || y >= WorldSizeInTiles) 
                    return false;

                if (x < 0 || x >= WorldSizeInTiles)
                    return false;

                var chunkIndex = GetChunkIndex(x, y);
                var chunk = Chunks[chunkIndex];
                if (chunk == null)
                    return false;

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

        private enum Rotation
        {
            Right,
            Down,
            Left,
            Up,
        }

        private class Walker
        {
            public int X { get; set; }
            public int Y { get; set; }
            public Rotation Rotation { get; set; }

            private World<bool> _world;

            public Walker(World<bool> world)
            {
                _world = world ?? throw new ArgumentNullException(nameof(world));
            }

            public void MoveToStartPosition()
            {
                // The map MUST contain an available tile on the top row, so no need to bounds check
                for (;; X += _world.ChunkSize)
                {
                    if (_world.TryGet(X, Y, out var tile))
                    {
                        // This will fail if there is no open tile within this chunk, but neither the test nor the real input have that
                        while (tile)
                        {
                            X++;
                            _world.TryGet(X, Y, out tile);
                        }
                        break;
                    }
                }

                Logger.DebugLine($"Starting at {X},{Y}");
            }

            public void Move(int distance)
            {
                switch (Rotation)
                {
                    case Rotation.Left:
                        {
                            for (; distance > 0; distance--)
                            {
                                if (_world.TryGet(X - 1, Y, out var occupied))
                                {
                                    if (occupied)
                                        return;
                                    X--;
                                }
                                else
                                {
                                    // Wrapping time!
                                    var checkX = _world.WorldSizeInTiles - 1;
                                    while (!_world.TryGet(checkX, Y, out occupied))
                                        checkX -= _world.ChunkSize;
                                    if (occupied)
                                        return;

                                    X = checkX;
                                }
                            }
                        }
                        break;
                    case Rotation.Right:
                        {
                            for (; distance > 0; distance--)
                            {
                                if (_world.TryGet(X + 1, Y, out var occupied))
                                {
                                    if (occupied)
                                        return;
                                    X++;
                                }
                                else
                                {
                                    // Wrapping time!
                                    var checkX = 0;
                                    while (!_world.TryGet(checkX, Y, out occupied))
                                        checkX += _world.ChunkSize;
                                    if (occupied)
                                        return;

                                    X = checkX;
                                }
                            }
                        }
                        break;
                    case Rotation.Up:
                        {
                            for (; distance > 0; distance--)
                            {
                                if (_world.TryGet(X, Y - 1, out var occupied))
                                {
                                    if (occupied)
                                        return;
                                    Y--;
                                }
                                else
                                {
                                    // Wrapping time!
                                    var checkY = _world.WorldSizeInTiles - 1;
                                    while (!_world.TryGet(X, checkY, out occupied))
                                        checkY -= _world.ChunkSize;
                                    if (occupied)
                                        return;

                                    Y = checkY;
                                }
                            }
                        }
                        break;
                    case Rotation.Down:
                        {
                            for (; distance > 0; distance--)
                            {
                                if (_world.TryGet(X, Y + 1, out var occupied))
                                {
                                    if (occupied)
                                        return;
                                    Y++;
                                }
                                else
                                {
                                    // Wrapping time!
                                    var checkY = 0;
                                    while (!_world.TryGet(X, checkY, out occupied))
                                        checkY += _world.ChunkSize;
                                    if (occupied)
                                        return;

                                    Y = checkY;
                                }
                            }
                        }
                        break;
                }
            }

            public void RotateLeft()
            {
                if (Rotation == Rotation.Right)
                    Rotation = Rotation.Up;
                else
                    Rotation--;
            }

            public void RotateRight()
            {
                if (Rotation == Rotation.Up)
                    Rotation = Rotation.Right;
                else
                    Rotation++;
            }

            public string PrintMap()
            {
                var str = _world.PrintMap();
                var strData = str.ToArray();
                strData[X + Y * (_world.WorldSizeInTiles + 1)] = 'W';
                return new string(strData);
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

            var walker = new Walker(world);
            walker.MoveToStartPosition();

            var m = 0;
            while (m < movement.Length)
            {
                var chr = movement[m];
                if (chr == 'R')
                {
                    walker.RotateRight();
                    m++;
                }
                else if (chr == 'L')
                {
                    walker.RotateLeft();
                    m++;
                }
                else
                {
                    var distance = ParseUtils.ParseIntPositive(movement, ref m);
                    walker.Move(distance);

                    //Logger.DebugLine($"Move {distance} {walker.Rotation}");
                    //Logger.DebugLine($"Moved to {walker.X}, {walker.Y}");

                    //Logger.Line(walker.PrintMap());

                    //Logger.Line();
                }
            }

            var row = walker.Y + 1;
            var col = walker.X + 1;
            var password = 1000 * row + 4 * col + walker.Rotation;

            return password.ToString();
        }
    }
}
