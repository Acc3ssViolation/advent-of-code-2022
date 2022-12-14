//#define ANIMATED_OUTPUT
//#define PRINT_MAP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day14_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var (width, height, offsetX) = input.Count < 10 ? (16, 16, 490) : (128, 180, 420);            

            var sand = new SandCave(width, height);
            var parser = new SandParser(sand, offsetX);
            for (int i = 0; i < input.Count; i++)
            {
                parser.ParseLine(input[i]);
            }

#if PRINT_MAP
            Logger.Line("Start situation");
            sand.Print();
#endif

            var sandCount = 0;
            int sandSpawnX = 500 - offsetX;
            while (sand.SpawnSand(sandSpawnX))
            {
                sandCount++;
            }

#if PRINT_MAP
            Logger.Line("End situation");
            sand.Print();
#endif
            return sandCount.ToString();
        }
    }

    internal class SandParser
    {
        private int _index;
        private string _str;
        private SandCave _cave;
        private int _xOffset;

        public int maxY;

        public SandParser(SandCave cave, int xOffset)
        {
            _str = string.Empty;
            _cave = cave ?? throw new ArgumentNullException(nameof(cave));
            _xOffset = xOffset;
        }

        public void ParseLine(string line)
        {
            _str = line;
            _index = 0;

            var previousPoint = ParseVector();

            while (ParseArrow())
            {
                var point = ParseVector();
                _cave.DrawLine(previousPoint, point);
                previousPoint = point;
            }
        }

        private Vector2Int ParseVector()
        {
            var x = ParseInt() - _xOffset;
            _index++;
            var y = ParseInt();
            if (y > maxY)
                maxY = y;
            return new Vector2Int(x, y);
        }

        private int ParseInt()
        {
            var num = 0;
            while (_index < _str.Length)
            {
                var chr = _str[_index];
                if (!char.IsNumber(chr))
                {
                    break;
                }
                num *= 10;
                num += chr - '0';
                _index++;
            }
            return num;
        }

        private bool ParseArrow()
        {
            if (_index < _str.Length)
            {
                _index += 4;
                return true;
            }
            return false;
        }
    }

    internal class SandCave
    {
        private char[] _map;
        private int _widthShift;
        private int _height;
        private int _width;

        public SandCave(int width, int height)
        {
            if (!BitOperations.IsPow2(width))
                throw new ArgumentException(null, nameof(width));

            _height = height;
            _width = width;
            _widthShift = BitOperations.Log2((uint)width);
            _map = new char[width * height];
            for (var i = 0; i < _width * _height; i++)
                _map[i] = '.';
        }

        public void DrawLine(Vector2Int from, Vector2Int to)
        {
            // Assume values are within range
            if (from.x == to.x)
            {
                // Vertical line
                var (minY, maxY) = (from.y < to.y) ? (from.y, to.y) : (to.y, from.y);
                var index = from.x + (minY << _widthShift);

                for (int y = minY; y <= maxY; y++)
                {
                    _map[index] = '#';
                    index += _width;
                }
            }
            else
            {
                // Horizontal line
                var (minX, maxX) = (from.x < to.x) ? (from.x, to.x) : (to.x, from.x);

                var index = minX + (from.y << _widthShift);
                for (int x = minX; x <= maxX; x++)
                {
                    _map[index++] = '#';
                }
            }
        }

        public bool SpawnSand(int x)
        {
            var index = x;
            var spawnIndex = x;

            while (true)
            {
#if ANIMATED_OUTPUT
                PrintWithSand(index);
#endif
                if (_map[index + _width] == '.')
                {
                    // Free space below, fall down
                    index += _width;
                }
                else
                {
                    // Try to fall down and to the left
                    var downLeft = index + _width - 1;
                    if (_map[downLeft] == '.')
                    {
                        index = downLeft;
                    }
                    else
                    {
                        // Try to fall down and to the right
                        var downRight = index + _width + 1;
                        if (_map[downRight] == '.')
                        {
                            index = downRight;
                        }
                        else
                        {
                            // End of the line
                            _map[index] = 'o';
                            break;
                        }
                    }
                }

                if ((index >> _widthShift) + 1 >= _height)
                    return false;
            }

            if (index == spawnIndex)
                return false;

            return true;
        }

        private void PrintWithSand(int sandIndex)
        {
            Console.CursorTop -= _height;
            _map[sandIndex] = 'x';
            Print();
            _map[sandIndex] = '.';
            Thread.Sleep(10);
        }

        public void Print()
        {
            var index = 0;
            for (int y = 0; y < _height; y++)
            {
                Console.Out.Write(_map.AsSpan(index, _width));
                Console.Out.Write('\n');
                index += _width;
            }
        }
    }
}
