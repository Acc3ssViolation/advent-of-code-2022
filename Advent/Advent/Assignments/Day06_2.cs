using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Advent.Assignments
{
    internal class Day06_2 : IAssignment
    {
        private class Fifo
        {
            private readonly uint[] _buffer;
            private int _index;

            public Fifo(int depth)
            {
                _buffer = new uint[depth];
                _index = 0;
            }

            public int Push(uint value)
            {
                _buffer[_index] = value;
                _index = (_index+ 1) % _buffer.Length;
                uint combined = 0;
                for (int i = 0; i < _buffer.Length; i++)
                    combined |= _buffer[i];
                return BitOperations.PopCount(combined);
            }
        }

        public Task<string> RunAsync(IReadOnlyList<string> input, CancellationToken cancellationToken = default)
        {
            int counter = 0;
            var fifo = new Fifo(14);
            var stream = input[0];

            for (int i = 0; i < stream.Length; i++)
            {
                if (fifo.Push(1u << (stream[i] - 'a')) == 14)
                {
                    counter = i + 1;
                    break;
                }
            }

            return Task.FromResult(counter.ToString());
        }
    }
}
