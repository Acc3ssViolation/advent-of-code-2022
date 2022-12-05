using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal static class Logger
    {
        public static void Line()
        {
            Console.WriteLine();
        }

        public static void Line(string message)
        {
            Console.WriteLine(message);
        }

        public static void ErrorLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void WarningLine(string message)
        {
            Console.WriteLine(message);
        }

        public static void DebugLine(string message)
        {
            Console.WriteLine(message);
        }
    }
}
