//#define LOG_DEBUG

namespace Advent.Assignments
{
    internal class Day15_2: IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            var sensors = new Sensor[input.Count];
            var segments = new Segment[input.Count];

            for (int i = 0; i < sensors.Length; i++)
            {
                sensors[i] = Sensor.Parse(input[i]);
            }

            var range = sensors.Length > 20 ? 4000000 : 20;

            for (var y = 0; y < range; y++)
            {
                var segmentCount = 0;

                for (int i = 0; i < sensors.Length; i++)
                {
                    ref var sensor = ref sensors[i];
                    var yDistance = Math.Abs(sensor.y - y);
                    var xDistanceHalf = sensor.range - yDistance;
                    if (xDistanceHalf < 0)
                        continue;

                    var start = sensor.x - xDistanceHalf;
                    var end = sensor.x + xDistanceHalf;

                    if (start < 0)
                        start = 0;
                    if (end > range)
                        end = range;

                    segments[segmentCount].active = true;
                    segments[segmentCount].start = start;
                    segments[segmentCount].end = end;
                    segmentCount++;
                }

                // Exit when we have more than 1 segment. This means there is a gap
                if (MergeSegments(segments, segmentCount, out int x) > 1)
                {
                    return (x * 4000000L + y).ToString();
                }
            }

            return "";
        }

        private static int MergeSegments(Segment[] segments, int segmentCount, out int gap)
        {
            // Merge segments
            int usedSegments;
            bool changes;
            gap = 0;
            do
            {
                changes = false;
                usedSegments = 0;
                for (int s = 0; s < segmentCount; s++)
                {
                    ref var segment = ref segments[s];

                    if (segment.active)
                    {
                        usedSegments++;
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

                        if (segment.start > 0)
                            gap = segment.start - 1;
                    }
                }
            } while (changes);

            return usedSegments;
        }
    }
}
