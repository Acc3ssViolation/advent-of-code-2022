//#define LOG_STEPS
using System.Text;

namespace Advent.Assignments
{
    internal class Day09_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            return Run(input, 2);
        }

        public static string Run(IReadOnlyList<string> input, int ropeLength)
        {
            var visitedPlaces = new HashSet<Vector2Int>();
            var sections = new Vector2Int[ropeLength];

#if LOG_STEPS
            var boardSize = ropeLength == 2 ? 5 : 15;
            Logger.DebugLine($"== Initial State ==");
            DebugPrint(sections, boardSize, boardSize);
#endif

            foreach (var line in input)
            {
                var move = line[0];
                var distance = int.Parse(line[2..]);

                Vector2Int delta = move switch
                {
                    'U' => Vector2Int.Up,
                    'D' => Vector2Int.Down,
                    'L' => Vector2Int.Left,
                    _ => Vector2Int.Right,
                };
#if LOG_STEPS
                Logger.DebugLine($"== {move} {distance} ==");
                Logger.Line();
#endif
                for (int i = 0; i < distance; i++)
                {
                    Move(delta, sections[0] + delta, sections, 0);
                    visitedPlaces.Add(sections[ropeLength - 1]);
#if LOG_STEPS
                    DebugPrint(sections, boardSize, boardSize);
#endif
                }
            }

#if LOG_STEPS
            DebugPrint(visitedPlaces.ToArray(), boardSize, boardSize);
#endif

            return visitedPlaces.Count.ToString();
        }

        private static void Move(Vector2Int delta, Vector2Int newPos, Vector2Int[] sections, int index)
        {
            if (index >= sections.Length - 1)
            {
                sections[index] = newPos;
                return;
            }

            var sd = newPos - sections[index + 1];
            if (sd.x < -1 || sd.x > 1 || sd.y < -1 || sd.y > 1)
            {
                // Clamp and move
                sd.x = Math.Sign(sd.x);
                sd.y = Math.Sign(sd.y);
                Move(delta, sections[index + 1] + sd, sections, index + 1);
            }

            sections[index] = newPos;
        }

#if LOG_STEPS
        private static void DebugPrint(Vector2Int[] sections, int width, int height)
        {
            var sb = new StringBuilder();
            for (int y = height; y >= -height; y--)
            {
                for (int x = -width; x <= width; x++)
                {
                    var chr = '.';
                    for (int i = 0; i < sections.Length; i++)
                    {
                        if (sections[i].x == x && sections[i].y == y)
                        {
                            chr = i.ToString()[0];
                            break;
                        }
                    }
                    sb.Append(chr);
                }
                sb.AppendLine();
            }
            Logger.DebugLine(sb.ToString());
        }
#endif
    }

    internal struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return $"{{{x}, {y}}}";
        }

        public static Vector2Int Left = new (-1, 0);
        public static Vector2Int Right = new (1, 0);
        public static Vector2Int Up = new (0, 1);
        public static Vector2Int Down = new (0, -1);

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new(a.x + b.x, a.y + b.y);
        public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new(a.x - b.x, a.y - b.y);
        public static Vector2Int operator *(Vector2Int a, int b) => new(a.x * b, a.y * b);
    }
}
