using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent
{
    internal static class Logger
    {
        private const string White = "\u001b[37m";
        private const string Red = "\u001b[31m";
        private const string Yellow = "\u001b[33m";
        private const string Green = "\u001b[32m";

        public static void Line()
        {
            Console.WriteLine();
        }

        public static void Line(string message)
        {
            Console.WriteLine(message);
            Console.Write(White);
        }

        public static void ErrorLine(string message)
        {
            Console.Write(Red);
            Console.WriteLine(message);
            Console.Write(White);
        }

        public static void WarningLine(string message)
        {
            Console.Write(Yellow);
            Console.WriteLine(message);
            Console.Write(White);
        }

        public static void DebugLine(string message)
        {
            Console.Write(Green);
            Console.WriteLine(message);
            Console.Write(White);
        }
    }
}
