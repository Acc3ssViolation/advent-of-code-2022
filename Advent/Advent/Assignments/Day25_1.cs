using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal static class Snafu
    {
        public static long Parse(string snafu)
        {
            var sum = 0L;
            for (var i = 0; i < snafu.Length; i++)
            {
                sum *= 5;

                var chr = snafu[i];
                if (chr == '-')
                    sum--;
                else if (chr == '=')
                    sum -= 2;
                else
                    sum += chr - '0';
            }
            return sum;
        }

        public static string Format(long num)
        {
            Span<char> chars = new char[(int)(Math.Log(num, 5) + 2)];
            var i = chars.Length - 1;
            while (num > 0)
            {
                var digit = num % 5;
                var carry = false;

                if (digit == 4)
                {
                    chars[i--] = '-';
                    carry = true;
                }
                else if (digit == 3)
                {
                    chars[i--] = '=';
                    carry = true;
                }
                else
                {
                    chars[i--] = (char)(digit + '0');
                }

                num /= 5;
                if (carry)
                    num++;
            }

            return new string(chars[(i + 1)..]);
        }
    }
    internal class Day25_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sum = 0L;
            for (var i = 0; i < input.Count;i++) 
            {
                var snafu = input[i];
                var value = Snafu.Parse(snafu);
                //Logger.DebugLine($"{value} = {snafu} = {Snafu.Format(value)}");
                sum += value;
            }
            return Snafu.Format(sum);
        }
    }
}
