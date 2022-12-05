namespace Advent.Assignments
{
    internal class Day02_1 : IAssignment
    {
        enum Move
        {
            Rock = 1,
            Paper = 2,
            Scissors = 3,
        }

        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            var score = 0;

            foreach (var round in input)
            {
                var moves = round.Trim().Split(' ');
                var opponent = GetOpponentMove(moves[0][0]);
                var self = GetSelfMove(moves[1][0]);

                var result = (int)self;
                if ((self == Move.Rock && opponent == Move.Scissors) || (self > opponent && !(opponent == Move.Rock && self == Move.Scissors)))
                {
                    // I win
                    result += 6;
                }
                else if (self < opponent || (opponent == Move.Rock && self == Move.Scissors))
                {
                    // Opponent wins
                    result += 0;
                }
                else
                {
                    // Draw
                    result += 3;
                }

                score += result;
            }

            return Task.FromResult(score.ToString());
        }

        private Move GetSelfMove(char move) => move switch
        {
            'X' => Move.Rock,
            'Y' => Move.Paper,
            'Z' => Move.Scissors,
            _ => throw new ArgumentException(null, nameof(move)),
        };

        private Move GetOpponentMove(char move) => move switch
        {
            'A' => Move.Rock,
            'B' => Move.Paper,
            'C' => Move.Scissors,
            _ => throw new ArgumentException(null, nameof(move)),
        };
    }
}
