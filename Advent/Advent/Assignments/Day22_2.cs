using System.Text;

namespace Advent.Assignments
{
    internal class Day22_2 : IAssignment
    {
        private enum RelativeRotation
        {
            None,
            Right,
            Half,
            Left,
        }

        private class World<T>
        {
            public int ChunkSize { get; }
            public int WorldSize { get; }
            public int WorldSizeInTiles { get; }
            public T[][] Chunks { get; }

            private Dictionary<(int chunk, Rotation edge), (int chunk, RelativeRotation relative)> _wrapEdges = new();

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

            public void ComputeWrappingTable()
            {
                _wrapEdges.Clear();

                for (var y = 0; y < WorldSize; y++)
                {
                    for (var x = 0; x < WorldSize; x++)
                    {
                        var chunk = Chunks[GetChunkIndex(x, y)];
                        if (chunk == null) 
                            continue;

                        // Found a chunk
                    }
                }



                const int a = 2;
                const int b = 6;
                const int c = 10;
                const int d = 5;
                const int e = 11;
                const int f = 4;

                _wrapEdges.Add((a, Rotation.Right), (e, RelativeRotation.Half));
                _wrapEdges.Add((a, Rotation.Up), (f, RelativeRotation.Half));
                _wrapEdges.Add((a, Rotation.Left), (d, RelativeRotation.Left));

                _wrapEdges.Add((b, Rotation.Right), (e, RelativeRotation.Right));

                _wrapEdges.Add((c, Rotation.Down), (f, RelativeRotation.Half));
                _wrapEdges.Add((c, Rotation.Left), (d, RelativeRotation.Right));

                _wrapEdges.Add((d, Rotation.Up), (a, RelativeRotation.Right));
                _wrapEdges.Add((d, Rotation.Down), (c, RelativeRotation.Left));

                _wrapEdges.Add((e, Rotation.Up), (b, RelativeRotation.Left));
                _wrapEdges.Add((e, Rotation.Right), (a, RelativeRotation.Half));
                _wrapEdges.Add((e, Rotation.Down), (f, RelativeRotation.Left));

                _wrapEdges.Add((f, Rotation.Up), (a, RelativeRotation.Half));
                _wrapEdges.Add((f, Rotation.Down), (c, RelativeRotation.Half));
                _wrapEdges.Add((f, Rotation.Left), (e, RelativeRotation.Right));
            }

            private (int X, int Y) GetChunkOrigin(int chunkIndex)
            {
                return ((chunkIndex % WorldSize) * ChunkSize, (chunkIndex / WorldSize) * ChunkSize);
            }

            public (int X, int Y, Rotation Rotation) Wrap(int x, int y, int newX, int newY, Rotation rotation)
            {
                var chunkIndex = GetChunkIndex(x, y);
                var (newChunkIndex, relativeRotation) = _wrapEdges[(chunkIndex, rotation)];

                // Make these relative to the new chunk
                var (chunkX, chunkY) = GetChunkOrigin(chunkIndex);
                newX -= chunkX;
                newY -= chunkY;
                if (newX < 0)
                    newX += ChunkSize;
                else if (newX >= ChunkSize)
                    newX -= ChunkSize;
                if (newY < 0)
                    newY += ChunkSize;
                else if (newY >= ChunkSize)
                    newY -= ChunkSize;

                var newRotation = RotateRotation(rotation, relativeRotation);

                if (relativeRotation == RelativeRotation.None)
                {
                    // No rotation, we're done
                }
                else if (relativeRotation == RelativeRotation.Right)
                {
                    var tmp = newX;
                    newX = ChunkSize - 1 - newY;
                    newY = tmp;
                }
                else if (relativeRotation == RelativeRotation.Left)
                {
                    var tmp = newY;
                    newY = ChunkSize - 1 - newX;
                    newX = tmp;
                }
                else
                {
                    // 180 degree flip
                    newX = ChunkSize - 1 - newX;
                    newY = ChunkSize - 1 - newY;
                }

                var (newChunkX, newChunkY) = GetChunkOrigin(newChunkIndex);
                newX += newChunkX;
                newY += newChunkY;
                return (newX, newY, newRotation);
            }

            public Rotation RotateRotation(Rotation rotation, RelativeRotation relativeRotation)
            {
                var angle = (int)rotation * 90;
                var offset = (int)relativeRotation * 90;

                var result = (Rotation)(((angle + offset) % 360) / 90);

                //Logger.DebugLine($"Rotating {rotation} {relativeRotation} gives {result}");

                return result;
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
            public Vector2Int Position { get; set; }
            public Rotation Rotation { get; set; }

            private World<bool> _world;

            public Walker(World<bool> world)
            {
                _world = world ?? throw new ArgumentNullException(nameof(world));
            }

            public void MoveToStartPosition()
            {
                // The map MUST contain an available tile on the top row, so no need to bounds check
                int x = 0;
                for (; ; x += _world.ChunkSize)
                {
                    if (_world.TryGet(x, 0, out var tile))
                    {
                        // This will fail if there is no open tile within this chunk, but neither the test nor the real input have that
                        while (tile)
                        {
                            x++;
                            _world.TryGet(x, 0, out tile);
                        }
                        break;
                    }
                }

                Position = new Vector2Int(x, 0);

                Logger.DebugLine($"Starting at {Position.x},{Position.y}");
            }

            public void Move(int distance)
            {
                for (; distance > 0; distance--)
                {
                    var delta = Rotation switch
                    {
                        Rotation.Left => new Vector2Int(-1, 0),
                        Rotation.Right => new Vector2Int(1, 0),
                        Rotation.Up => new Vector2Int(0, -1),
                        _ => new Vector2Int(0, 1),
                    };

                    var newPos = Position + delta;

                    if (_world.TryGet(newPos.x, newPos.y, out var occupied))
                    {
                        if (occupied)
                            return;
                        Position = newPos;
                    }
                    else
                    {
                        // Wrapping time!
                        var wrapped = _world.Wrap(Position.x, Position.y, newPos.x, newPos.y, Rotation);
                        _world.TryGet(wrapped.X, wrapped.Y, out occupied);
                        if (occupied)
                            return;

                        //Logger.DebugLine($"Wrapped from {Position.x}, {Position.y} to {wrapped.X}, {wrapped.Y}");

                        Rotation = wrapped.Rotation;
                        Position = new Vector2Int(wrapped.X, wrapped.Y);

                        //Logger.DebugLine(PrintMap());
                    }
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
                strData[Position.x + Position.y * (_world.WorldSizeInTiles + 1)] = 'W';
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

            world.ComputeWrappingTable();

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
                    //Logger.DebugLine($"Moved to {walker.Position.x}, {walker.Position.y}");
                    //Logger.Line(walker.PrintMap());
                    //Logger.Line();
                }
            }

            var row = walker.Position.y + 1;
            var col = walker.Position.x + 1;
            var password = 1000 * row + 4 * col + walker.Rotation;

            return password.ToString();
        }
    }
}
