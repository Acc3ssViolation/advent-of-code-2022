namespace Advent.Assignments
{
    internal class Day09_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var visitedPlaces = new HashSet<Vector2Int>();
            var head = new Vector2Int();
            var tail = new Vector2Int();

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

                for (int i = 0; i < distance; i++)
                {
                    var newHead = head + delta;
                    if (tail.MaxDistance(newHead) > 1)
                        tail = head;
                    head = newHead;
                    visitedPlaces.Add(tail);
                }
            }

            return visitedPlaces.Count.ToString();
        }
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

        public int MaxDistance(Vector2Int other)
        {
            var dx = Math.Abs(other.x - x);
            var dy = Math.Abs(other.y - y);
            return Math.Max(dy, dx);
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
