using System.Diagnostics;
using System.Drawing;
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

            private class Side
            {
                public int ChunkIndex { get; set; }
                public CubeSide Id { get; set; }
                /// <summary>
                /// These are clockwise from the top left when looking at them on the 'normal' unwrapping (see CubeSide)
                /// </summary>
                public int[] Vertexes { get; }

                public Side(int chunkIndex, CubeSide id)
                {
                    ChunkIndex = chunkIndex;
                    Id = id;
                    Vertexes = new int[4];

                    // These are tl, tr, bl, br
                    //LogSideCode(0, 1, 3, 2, CubeSide.A);
                    //LogSideCode(4, 5, 0, 1, CubeSide.D);
                    //LogSideCode(1, 5, 2, 6, CubeSide.C);
                    //LogSideCode(3, 2, 7, 6, CubeSide.E);
                    //LogSideCode(4, 0, 7, 3, CubeSide.B);
                    //LogSideCode(5, 4, 6, 7, CubeSide.F);
                }

                public static (int Left, int Right) GetVertexesBelow(int left, int right)
                {
                    return (left, right) switch
                    {
                        (0, 1) => (3, 2),
                        (4, 5) => (0, 1),
                        (1, 5) => (2, 6),
                        (3, 2) => (7, 6),
                        (4, 0) => (7, 3),
                        (5, 4) => (6, 7),

                        (3, 0) => (2, 1),
                        (0, 4) => (1, 5),
                        (2, 1) => (6, 5),
                        (7, 3) => (6, 2),
                        (7, 4) => (3, 0),
                        (6, 5) => (7, 4),

                        (2, 3) => (1, 0),
                        (1, 0) => (5, 4),
                        (6, 2) => (5, 1),
                        (6, 7) => (2, 3),
                        (3, 7) => (0, 4),
                        (7, 6) => (4, 5),

                        (1, 2) => (0, 3),
                        (5, 1) => (4, 0),
                        (5, 6) => (1, 2),
                        (2, 6) => (3, 7),
                        (0, 3) => (4, 7),
                        (4, 7) => (5, 6),

                        _ => ThrowInvalidValue((left, right)),
                    };
                }

                private static void LogSideCode(int a, int b, int c, int d, CubeSide side)
                {
                    Logger.DebugLine($"{side} = {GetSideCode(a, b, c, d)}");
                }

                private static int GetSideCode(int a, int b, int c, int d)
                {
                    Span<int> list = stackalloc int[4];
                    list[0] = a;
                    list[1] = b;
                    list[2] = c;
                    list[3] = d;
                    list.Sort();

                    var code = list[0] + list[1] * 8 + list[2] * 16 + list[3] * 24;
                    return code;
                }

                public static CubeSide GetCubeSide(int a, int b, int c, int d)
                {
                    var code = GetSideCode(a, b, c, d);
                    return code switch
                    {
                        112 => CubeSide.A,
                        192 => CubeSide.D,
                        241 => CubeSide.C,
                        290 => CubeSide.E,
                        256 => CubeSide.B,
                        308 => CubeSide.F,
                        _ => (CubeSide)ThrowInvalidValue(code),
                    };
                }

                private static K ThrowInvalidValue<K>(K value) => throw new ArgumentException($"{value} is invalid.");

                public override string ToString()
                {
                    return $"{this.Id} = {this.ChunkIndex}: {this.Vertexes.AggregateString()}";
                }
            }

            private class WrapMapper
            {
                private World<T> _world;
                private Queue<Side> _queue = new();
                private HashSet<int> _checked = new();

                public WrapMapper(World<T> world)
                {
                    _world = world ?? throw new ArgumentNullException(nameof(world));
                }

                public Side[] ComputeWrapping()
                {
                    _queue.Clear();

                    var sides = new List<Side>(6);

                    for (var index = 0; index < _world.Chunks.Length; index++)
                    {
                        if (_world.Chunks[index] == null)
                            continue;

                        var side = new Side(index, CubeSide.A);
                        side.Vertexes[0] = 0;
                        side.Vertexes[1] = 1;
                        side.Vertexes[2] = 2;
                        side.Vertexes[3] = 3;
                        _queue.Enqueue(side);
                        _checked.Add(index);
                        break;
                    }

                    while (_queue.Count > 0)
                    {
                        var side = _queue.Dequeue();

                        // Check below
                        CheckChunk(side.ChunkIndex + _world.WorldSize, side.Vertexes[3], side.Vertexes[2], 0);

                        // Check above
                        CheckChunk(side.ChunkIndex - _world.WorldSize, side.Vertexes[1], side.Vertexes[0], 2);

                        // Check left
                        CheckChunk(side.ChunkIndex - 1, side.Vertexes[0], side.Vertexes[3], 1);

                        // Check right
                        CheckChunk(side.ChunkIndex + 1, side.Vertexes[2], side.Vertexes[1], 3);

                        sides.Add(side);
                    }

                    return sides.ToArray();
                }

                private void CheckChunk(int chunkIndex, int topLeft, int topRight, int vertexOffset)
                {
                    if (chunkIndex < 0 || chunkIndex >= _world.Chunks.Length)
                        return;
                    if (_world.Chunks[chunkIndex] == null)
                        return;
                    if (_checked.Contains(chunkIndex)) 
                        return;

                    var (bottomLeft, bottomRight) = Side.GetVertexesBelow(topLeft, topRight);
                    var side = new Side(chunkIndex, Side.GetCubeSide(topLeft, topRight, bottomLeft, bottomRight));
                    // Note: the topLeft, topRight, etc. are rotated when viewed from the map, which is why we apply vertexOffset before storing them
                    side.Vertexes[(vertexOffset + 0) % 4] = topLeft;
                    side.Vertexes[(vertexOffset + 1) % 4] = topRight;
                    side.Vertexes[(vertexOffset + 2) % 4] = bottomRight;
                    side.Vertexes[(vertexOffset + 3) % 4] = bottomLeft;
                    _checked.Add(chunkIndex);
                    _queue.Enqueue(side);
                }
            }

            public void ComputeWrappingTable()
            {
                _wrapEdges.Clear();

                var wrapper = new WrapMapper(this);
                var sides = wrapper.ComputeWrapping();
                var rotationTable = new RelativeRotation[]
                {
                    RelativeRotation.Right,
                    RelativeRotation.None,
                    RelativeRotation.Left,
                    RelativeRotation.Half,
                };

                for (int s = 0; s < sides.Length; s++)
                {
                    var side = sides[s];

                    for (var edge = 0; edge < 4; edge++)
                    {
                        var startIndex = (edge + 0) % 4;
                        var sv = side.Vertexes[startIndex];
                        var ev = side.Vertexes[(edge + 1) % 4];

                        // Find the matching edge. Note that our start must match their end and vice versa
                        for (int o = 0; o < sides.Length; o++)
                        {
                            if (o == s)
                                continue;

                            var other = sides[o];
                            // Note: these indexes are the location of _our_ start and end vertexes in the other side's buffer
                            var otherStartIndex = Array.IndexOf(other.Vertexes, sv);
                            var otherEndIndex = Array.IndexOf(other.Vertexes, ev);
                            if (otherStartIndex < 0 || otherEndIndex < 0)
                                continue;

                            // Assert on the property described above
                            Debug.Assert((otherEndIndex + 1) % 4 == otherStartIndex);

                            // So, we found our matching edge. Figure out the rotation and we're done
                            var rotationIndex = (startIndex - otherStartIndex + 4) % 4;

                            var rotation = rotationTable[rotationIndex];
                            var direction = (Direction)((startIndex + 3) % 4);
                            _wrapEdges.Add((side.ChunkIndex, direction), (other.ChunkIndex, rotation));
                        }
                    }
                }
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
            public Direction Direction { get; set; }

            private World<bool> _world;
            private List<(Vector2Int, Direction)> _path = new();

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

                //Logger.DebugLine($"Starting at {Position.x},{Position.y}");

                _path.Add((Position, Direction));
            }

            public void Move(int distance)
            {
                for (; distance > 0; distance--)
                {
                    var delta = Direction switch
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
                        var wrapped = _world.Wrap(Position.x, Position.y, newPos.x, newPos.y, Direction);
                        _world.TryGet(wrapped.X, wrapped.Y, out occupied);
                        if (occupied)
                            return;

                        //Logger.DebugLine($"Wrapped from {Position.x}, {Position.y} to {wrapped.X}, {wrapped.Y}");

                        Direction = wrapped.Rotation;
                        Position = new Vector2Int(wrapped.X, wrapped.Y);
                    }

                    _path.Add((Position, Direction));
                }
            }

            public void RotateLeft()
            {
                if (Direction == Direction.Right)
                    Direction = Direction.Up;
                else
                    Direction--;

                _path.Add((Position, Direction));
            }

            public void RotateRight()
            {
                if (Direction == Direction.Up)
                    Direction = Direction.Right;
                else
                    Direction++;

                _path.Add((Position, Direction));
            }

            public string PrintMap()
            {
                var str = _world.PrintMap();
                var strData = str.ToArray();

                foreach (var point in _path)
                {
                    var chr = point.Item2 switch
                    {
                        Direction.Right => '>',
                        Direction.Down => 'v',
                        Direction.Left => '<',
                        _ => '^',
                    };
                    strData[point.Item1.x + point.Item1.y * (_world.WorldSizeInTiles + 1)] = chr;
                }

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

            //Logger.Line(walker.PrintMap());

            var row = walker.Position.y + 1;
            var col = walker.Position.x + 1;
            var password = 1000 * row + 4 * col + walker.Direction;

            return password.ToString();
        }
    }
}
