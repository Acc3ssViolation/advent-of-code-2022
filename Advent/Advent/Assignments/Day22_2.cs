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

        /// <summary>
        /// Cube sides in this pattern
        ///     D
        /// F B A C
        ///     E
        /// Opposite sides are AF, BC and DE
        /// </summary>
        private enum CubeSide
        {
            A,
            B,
            C,
            D,
            E,
            F
        }

        private struct Cube
        {
            public CubeSide Face { get; private set; }
            public CubeSide Back { get; private set; }
            public CubeSide Up { get; private set; }
            public CubeSide Down { get; private set; }
            public CubeSide Left { get; private set; }
            public CubeSide Right { get; private set; }

            public CubeSide this[Direction dir] => dir switch
            {
                Direction.Right => Right,
                Direction.Down => Down,
                Direction.Left => Left,
                _ => Up,
            };

            public Cube()
            {
                Face = CubeSide.A;
                Back = CubeSide.F;
                Up = CubeSide.D;
                Down = CubeSide.E;
                Left = CubeSide.B;
                Right = CubeSide.C;
            }

            public static CubeSide Expected(CubeSide side, Direction dir) => dir switch
            {
                Direction.Right => ExpectedRight(side),
                Direction.Down => ExpectedDown(side),
                Direction.Left => ExpectedLeft(side),
                _ => ExpectedUp(side),
            };

            public static CubeSide ExpectedUp(CubeSide side) => side switch
            {
                CubeSide.A => CubeSide.D,
                CubeSide.B => CubeSide.D,
                CubeSide.C => CubeSide.D,
                CubeSide.F => CubeSide.D,
                CubeSide.E => CubeSide.A,
                _ => CubeSide.F,
            };

            public static CubeSide ExpectedDown(CubeSide side) => side switch
            {
                CubeSide.A => CubeSide.E,
                CubeSide.B => CubeSide.E,
                CubeSide.C => CubeSide.E,
                CubeSide.F => CubeSide.E,
                CubeSide.E => CubeSide.F,
                _ => CubeSide.A,
            };

            public static CubeSide ExpectedRight(CubeSide side) => side switch
            {
                CubeSide.A => CubeSide.C,
                CubeSide.B => CubeSide.A,
                CubeSide.C => CubeSide.F,
                CubeSide.F => CubeSide.B,
                CubeSide.E => CubeSide.C,
                _ => CubeSide.C,
            };

            public static CubeSide ExpectedLeft(CubeSide side) => side switch
            {
                CubeSide.A => CubeSide.B,
                CubeSide.B => CubeSide.F,
                CubeSide.C => CubeSide.A,
                CubeSide.F => CubeSide.C,
                CubeSide.E => CubeSide.B,
                _ => CubeSide.B,
            };

            public static CubeSide Opposite(CubeSide side) => side switch
            {
                CubeSide.A => CubeSide.F,
                CubeSide.B => CubeSide.C,
                CubeSide.C => CubeSide.B,
                CubeSide.F => CubeSide.A,
                CubeSide.E => CubeSide.D,
                _ => CubeSide.E,
            }; 

            public Cube RotateUp()
            {
                return new Cube
                {
                    Face = Up,
                    Up = Back,
                    Back = Down,
                    Down = Face,
                    Right = Right,
                    Left = Left,
                };
            }

            public Cube RotateDown()
            {
                return new Cube
                {
                    Face = Down,
                    Up = Face,
                    Back = Up,
                    Down = Back,
                    Right = Right,
                    Left = Left,
                };
            }

            public Cube RotateRight()
            {
                return new Cube
                {
                    Face = Right,
                    Right = Back,
                    Back = Left,
                    Left = Face,
                    Up = Up,
                    Down = Down,
                };
            }

            public Cube RotateLeft()
            {
                return new Cube
                {
                    Face = Left,
                    Right = Face,
                    Back = Right,
                    Left = Back,
                    Up = Up,
                    Down = Down,
                };
            }

            public override string ToString()
            {
                return Face.ToString();
            }
        }

        private class World<T>
        {
            

            public int ChunkSize { get; }
            public int WorldSize { get; }
            public int WorldSizeInTiles { get; }
            public T[][] Chunks { get; }

            private Dictionary<(int chunk, Direction edge), (int chunk, RelativeRotation relative)> _wrapEdges = new();

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

            private int ChunkUp(int index) => index - WorldSize;

            private int ChunkDown(int index) => index + WorldSize;

            private int ChunkLeft(int index) => index - 1;

            private int ChunkRight(int index) => index + 1;

            public void ComputeWrappingTable()
            {
                _wrapEdges.Clear();

                var sidesToCubes = new Cube[6];
                var sidesToIndexes = new int[6] { -1, -1, -1, -1, -1, -1 };
                var sidesLeftToFind = 6;
                var queue = new Queue<(Cube Cube, int Index)>();

                for (var index = 0; index < Chunks.Length; index++)
                {
                    if (Chunks[index] == null)
                        continue;
                    queue.Enqueue((new Cube(), index));
                    break;
                }

                while (true)
                {
                    var current = queue.Dequeue();
                    Logger.DebugLine($"Mapping side {current.Cube} to index {current.Index}");

                    sidesToIndexes[(int)current.Cube.Face] = current.Index;
                    sidesToCubes[(int)current.Cube.Face] = current.Cube;
                    sidesLeftToFind--;
                    if (sidesLeftToFind == 0)
                        break;

                    Cube side;
                    side = current.Cube.RotateLeft();
                    if (sidesToIndexes[(int)side.Face] < 0)
                    {
                        var index = ChunkLeft(current.Index);
                        if (index >= 0 && index < Chunks.Length && Chunks[index] != null)
                        {
                            Logger.DebugLine($"{side} is left of {current.Cube}");
                            queue.Enqueue((side, index));
                        }
                    }

                    side = current.Cube.RotateRight();
                    if (sidesToIndexes[(int)side.Face] < 0)
                    {
                        var index = ChunkRight(current.Index);
                        if (index >= 0 && index < Chunks.Length && Chunks[index] != null)
                        {
                            Logger.DebugLine($"{side} is right of {current.Cube}");
                            queue.Enqueue((side, index));
                        }
                    }

                    side = current.Cube.RotateUp();
                    if (sidesToIndexes[(int)side.Face] < 0)
                    {
                        var index = ChunkUp(current.Index);
                        if (index >= 0 && index < Chunks.Length && Chunks[index] != null)
                        {
                            Logger.DebugLine($"{side} is up of {current.Cube}");
                            queue.Enqueue((side, index));
                        }
                    }

                    side = current.Cube.RotateDown();
                    if (sidesToIndexes[(int)side.Face] < 0)
                    {
                        var index = ChunkDown(current.Index);
                        if (index >= 0 && index < Chunks.Length && Chunks[index] != null)
                        {
                            Logger.DebugLine($"{side} is down of {current.Cube}");
                            queue.Enqueue((side, index));
                        }
                    }
                }

                int a = sidesToIndexes[(int)CubeSide.A];
                int e = sidesToIndexes[(int)CubeSide.B];
                int f = sidesToIndexes[(int)CubeSide.C];
                int b = sidesToIndexes[(int)CubeSide.D];
                int c = sidesToIndexes[(int)CubeSide.E];
                int d = sidesToIndexes[(int)CubeSide.F];

                // The 'normalized' rotations
                // Look up as (side * 4 + direction)
                var normalEdges = new int[] {
                    // A (order is right, down, left, up)
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.None,

                    // B
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Left,
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Right,

                    // C
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Right,
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Left,

                    // D
                    (int) RelativeRotation.Right,
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Left,
                    (int) RelativeRotation.Half,

                    // E
                    (int) RelativeRotation.Left,
                    (int) RelativeRotation.Half,
                    (int) RelativeRotation.Right,
                    (int) RelativeRotation.None,

                    // F
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Half,
                    (int) RelativeRotation.None,
                    (int) RelativeRotation.Half,
                };

                var rotations = new int[6];

                for (var side = 0; side < 6; side++)
                {
                    var actual = sidesToCubes[side][Direction.Up];
                    var expected = Cube.ExpectedUp((CubeSide)side);

                    if (expected == actual)
                    {
                        // Nothing
                        rotations[side] = 0;
                    }
                    else if (expected == Cube.Opposite(actual))
                    {
                        rotations[side] = 2;
                    }
                    else
                    {
                        if (sidesToCubes[side][Direction.Left] == expected)
                        {
                            rotations[side] = 1;
                        }
                        else
                        {
                            rotations[side] = 3;
                        }
                    }
                }

                for (var side = 0; side < 6; side++)
                {
                    for (var dir = 0; dir < 4; dir++)
                    {
                        //var attachedSide = (int)sidesToCubes[side][(Direction)dir];
                        var normalizedDir = (rotations[side] + dir) % 4;
                        
                        var attachedSide = (int)Cube.Expected((CubeSide)side, (Direction)normalizedDir);
                        var normalRotation = normalEdges[side * 4 + normalizedDir];

                        Logger.DebugLine($"Normal rotation from {(CubeSide)side} to {(CubeSide)attachedSide} is {(RelativeRotation)normalRotation}");

                        var finalRotation = (normalRotation + rotations[attachedSide] + rotations[side]) % 4;

                        Logger.DebugLine($"Final rotation from {(CubeSide)side} to {(CubeSide)attachedSide} is {(RelativeRotation)finalRotation}");
                        Logger.Line();

                        _wrapEdges.Add((sidesToIndexes[side], (Direction)dir), (sidesToIndexes[attachedSide], (RelativeRotation)finalRotation));
                    }
                }

                Logger.Line();

                //_wrapEdges.Add((a, Direction.Right), (c, RelativeRotation.Half));
                //_wrapEdges.Add((a, Direction.Up), (d, RelativeRotation.Half));
                //_wrapEdges.Add((a, Direction.Left), (b, RelativeRotation.Left));      Right

                //_wrapEdges.Add((e, Direction.Right), (c, RelativeRotation.Right));    Right

                //_wrapEdges.Add((f, Direction.Down), (d, RelativeRotation.Half));
                //_wrapEdges.Add((f, Direction.Left), (b, RelativeRotation.Right));     Left

                //_wrapEdges.Add((b, Direction.Up), (a, RelativeRotation.Right));       Half
                //_wrapEdges.Add((b, Direction.Down), (f, RelativeRotation.Left));      Half

                //_wrapEdges.Add((c, Direction.Up), (e, RelativeRotation.Left));
                //_wrapEdges.Add((c, Direction.Right), (a, RelativeRotation.Half));
                //_wrapEdges.Add((c, Direction.Down), (d, RelativeRotation.Left));

                //_wrapEdges.Add((d, Direction.Up), (a, RelativeRotation.Half));
                //_wrapEdges.Add((d, Direction.Down), (f, RelativeRotation.Half));
                //_wrapEdges.Add((d, Direction.Left), (c, RelativeRotation.Right));
            }

            private (int X, int Y) GetChunkOrigin(int chunkIndex)
            {
                return ((chunkIndex % WorldSize) * ChunkSize, (chunkIndex / WorldSize) * ChunkSize);
            }

            public (int X, int Y, Direction Rotation) Wrap(int x, int y, int newX, int newY, Direction rotation)
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

                var newRotation = RotateDirection(rotation, relativeRotation);

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

            public Direction RotateDirection(Direction rotation, RelativeRotation relativeRotation)
            {
                var angle = (int)rotation;
                var offset = (int)relativeRotation;

                var result = (Direction)((angle + offset) % 4);

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

        private enum Direction
        {
            Right,
            Down,
            Left,
            Up,
        }

        private class Walker
        {
            public Vector2Int Position { get; set; }
            public Direction Rotation { get; set; }

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
                        Direction.Left => new Vector2Int(-1, 0),
                        Direction.Right => new Vector2Int(1, 0),
                        Direction.Up => new Vector2Int(0, -1),
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
                if (Rotation == Direction.Right)
                    Rotation = Direction.Up;
                else
                    Rotation--;
            }

            public void RotateRight()
            {
                if (Rotation == Direction.Up)
                    Rotation = Direction.Right;
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
