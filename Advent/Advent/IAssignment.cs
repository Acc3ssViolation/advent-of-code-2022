namespace Advent
{
    internal interface IAssignment
    {
        virtual string Name => GetType().Name;

        virtual string InputFile 
        {
            get
            {
                var name = GetType().Name;
                var dayEnd = name.IndexOf("_");
                if (dayEnd == -1)
                    return name + ".txt";
                return name.Substring(0, dayEnd) + ".txt";
            }
        }

        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default);
    }
}
