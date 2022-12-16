//#define LOG_DEBUG

namespace Advent.Assignments
{
    internal class Day15_1 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sensors = new Sensor[input.Count];
            var segments = new Segment[input.Count];

            for (int i = 0; i < sensors.Length; i++)
            {
                sensors[i] = Sensor.Parse(input[i]);
            }

            var y = sensors.Length > 20 ? 2000000 : 10;
            segments[0].active = false;
            var segmentCount = 0;
            var beacons = new int[segments.Length];
            var beaconsInLine = 0;

            for (int i = 0; i < sensors.Length; i++)
            {
                ref var sensor = ref sensors[i];
                var yDistance = Math.Abs(sensor.y - y);
                var xDistanceHalf = sensor.range - yDistance;
                if (xDistanceHalf < 0)
                    continue;

                if (sensor.by == y)
                {
                    var foundBeacon = false;
                    for (int b = 0; b < beaconsInLine; b++)
                    {
                        if (beacons[b] == sensor.bx)
                        {
                            foundBeacon = true;
                            break;
                        }
                    }
                    if (!foundBeacon)
                    {
                        beacons[beaconsInLine++] = sensor.bx;
                    }
                }

                var start = sensor.x - xDistanceHalf;
                var end = sensor.x + xDistanceHalf;

                segments[segmentCount].active = true;
                segments[segmentCount].start = start;
                segments[segmentCount].end = end;
                segmentCount++;
            }

            // Merge segments
            bool changes;
            do
            {
                changes = false;
                for (int s = 0; s < segmentCount; s++)
                {
                    ref var segment = ref segments[s];

                    if (segment.active)
                    {
                        for (int o = 0; o < segmentCount; o++)
                        {
                            if (s == o)
                                continue;

                            ref var other = ref segments[o];
                            if (!other.active || other.end < segment.start || other.start > segment.end)
                                continue;
#if LOG_DEBUG
                            Logger.DebugLine($"Merged [{o}][{other.start}, {other.end}] into [{s}][{segment.start}, {segment.end}]");
#endif
                            if (other.start < segment.start)
                                segment.start = other.start;
                            if (other.end > segment.end)
                                segment.end = other.end;
                            other.active = false;
                            changes = true;
#if LOG_DEBUG
                            Logger.DebugLine($"[{s}][{segment.start}, {segment.end}]");
#endif
                        }
                    }
                }
            } while (changes);

            // Count covered area
            var covered = -beaconsInLine;
            for (int s = 0; s < segmentCount; s++)
            {
                ref var segment = ref segments[s];
                if (!segment.active)
                    continue;
                var length = segment.end - segment.start + 1;
#if LOG_DEBUG
                Logger.DebugLine($"[{s}][{segment.start}, {segment.end}] = {length}");
#endif
                covered += length;
            }

            return covered.ToString();
        }
    }

    internal struct Segment
    {
        public bool active;
        public int start;
        public int end;

        public Segment(int start, int end)
        {
            active = false;
            this.start = start;
            this.end = end;
        }
    }

    internal struct Sensor
    {
        public int x;
        public int y;
        public int bx;
        public int by;
        public int range;

        public Sensor(int x, int y, int bx, int by, int range)
        {
            this.x = x;
            this.y = y;
            this.bx = bx;
            this.by = by;
            this.range = range;
        }

        public static Sensor Parse(string input)
        {
            // Sensor at x=
            var index = 12;
            // {num}
            var x = ParseInt(input, ref index);
            // , y=
            index += 4;
            // {num}
            var y = ParseInt(input, ref index);
            // : closest beacon is at x=
            index += 25;
            // {num}
            var bx = ParseInt(input, ref index);
            // , y=
            index += 4;
            // {num}
            var by = ParseInt(input, ref index);

            var range = Math.Abs(x - bx) + Math.Abs(y - by);
#if LOG_DEBUG
            Logger.DebugLine($"{input} = {x},{y} -> {bx},{by}: {range}");
#endif

            return new Sensor(x, y, bx, by, range);
        }

        private static int ParseInt(string str, ref int index)
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
