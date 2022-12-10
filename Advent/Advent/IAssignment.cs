using System;

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
                return string.Concat(name.AsSpan(0, dayEnd), ".txt");
            }
        }

        virtual string TestFile => "test-" + InputFile;

        virtual int Part
        {
            get
            {
                var name = GetType().Name;
                var dayEnd = name.IndexOf("_");
                if (dayEnd == -1)
                    return 1;
                return int.Parse(name.AsSpan(dayEnd + 1));
            }
        }

        public string Run(IReadOnlyList<string> input);
    }
}
