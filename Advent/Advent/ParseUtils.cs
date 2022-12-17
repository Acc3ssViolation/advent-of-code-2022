namespace Advent
{
    internal static class ParseUtils
    {
        public static int ParseIntPositive(string str, ref int index)
        {
            var num = 0;
            while (index < str.Length)
            {
                var chr = str[index];
                if (!char.IsNumber(chr))
                {
                    break;
                }
                num *= 10;
                num += chr - '0';
                index++;
            }
            return num;
        }

        public static int ParseInt(string str, ref int index)
        {
            bool negative = false;
            var num = 0;
            if (str[index] == '-')
            {
                negative = true;
                index++;
            }
            while (index < str.Length)
            {
                var chr = str[index];
                if (!char.IsNumber(chr))
                {
                    break;
                }
                num *= 10;
                num += chr - '0';
                index++;
            }
            return negative ? -num : num;
        }
    }
}
