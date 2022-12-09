namespace Advent.Assignments
{
    internal class Day09_2 : IAssignment
    {
        public string TestFile => "test-day09-2.txt";

        public string Run(IReadOnlyList<string> input)
        {
            return Day09_1.Run(input, 10);
        }
    }
}
