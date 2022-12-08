//#define LOG_SCORE_ADDITIONS

namespace Advent.Assignments
{
    internal class Day08_2 : IAssignment
    {
        public string Run(IReadOnlyList<string> input)
        {
            // Construct the map
            var width = input[0].Trim().Length;
            var height = input.Count;

            Logger.DebugLine($"Width: {width}, Height: {height}");

            var trees = new int[width* height];

            for (int y = 0; y < height; y++)
            {
                var line = input[y];
                for (int x = 0; x < width; x++)
                {
                    trees[x + y * width] = line[x] - '0';
                }
            }

            var scores = new ScenicScores(width, height);

            // Scan each row for tree viewing distances
            for (int y = 1; y < height - 1; y++)
            {
                var tallest = -1;
                var previousTreeX = 0;
                for (int x = 1; x < width - 1; x++)
                {
                    var index = x + y * width;
                    var tree = trees[index];
                    if (tree > tallest)
                    {
                        // This tree is taller than all the previous ones. Its view distance to the left is the X coordinate.
                        scores.AddView(index, x);
                        // The view distance of the previous tree to the right is x - previousTreeX
                        scores.AddView(previousTreeX + y * width, x - previousTreeX);
                        tallest = tree;
                        previousTreeX = x;
                    }
                    else if (tree == tallest)
                    {
                        // This tree is not taller than one of the previous ones. Its view distance to the left is x - previousTreeX.
                        scores.AddView(index, x - previousTreeX);
                        // The view distance of the previous tree to the right is also x - previousTreeX
                        scores.AddView(previousTreeX + y * width, x - previousTreeX);
                        previousTreeX = x;
                    }
                }

                // Close off the last one
                // The view distance of the last tree to the right is width - 1 - previousTreeX
                scores.AddView(previousTreeX + y * width, width - 1 - previousTreeX);
            }

            // Scan each column for tree viewing distances
            for (int x = 1; x < width - 1; x++)
            {
                var tallest = -1;
                var previousTreeY = 0;
                for (int y = 1; y < height - 1; y++)
                {
                    var index = x + y * width;
                    var tree = trees[index];
                    if (tree > tallest)
                    {
                        // This tree is taller than all the previous ones.
                        if (scores[index] == 0)
                        {
                            // This tree needs to have its horizontal scores calculated as well
#if LOG_SCORE_ADDITIONS
                            Logger.WarningLine($"[{y},{x}] missing horizontal scores");
#endif
                            scores.AddView(index, CalculateHorizontalScores(x, y, tree, trees, width));
                        }
                        // Its view distance to the top is the Y coordinate.
                        scores.AddView(index, y);
                        // The view distance of the previous tree to the bottom is y - previousTreeY
                        scores.AddView(x + previousTreeY * width, y - previousTreeY);
                        tallest = tree;
                        previousTreeY = y;
                    }
                    else if (tree == tallest)
                    {
                        // This tree is not taller than one of the previous ones.
                        if (scores[index] == 0)
                        {
                            // This tree needs to have its horizontal scores calculated as well
#if LOG_SCORE_ADDITIONS
                            Logger.WarningLine($"[{y},{x}] missing horizontal scores");
#endif
                            scores.AddView(index, CalculateHorizontalScores(x, y, tree, trees, width));
                        }
                        // Its view distance to the top is y - previousTreeY.
                        scores.AddView(index, y - previousTreeY);
                        // The view distance of the previous tree to the bottom is also y - previousTreeY
                        scores.AddView(x + previousTreeY * width, y - previousTreeY);
                        previousTreeY = y;
                    }
                }

                // Close off the last one
                // The view distance of the last tree to the bottom is height - 1 - previousTreeY
                scores.AddView(x + previousTreeY * width, height - 1 - previousTreeY);
            }


            return scores.HighestScore.ToString();
        }

        private static int CalculateHorizontalScores(int treeX, int treeY, int tree, int[] trees, int width)
        {
            var left = 0;
            for (int x = treeX - 1; x >= 0; x--)
            {
                left++;
                if (trees[x + treeY * width] >= tree)
                    break;
            }
#if LOG_SCORE_ADDITIONS
            Logger.DebugLine($"[{treeY},{treeX}] L {left}");
#endif
            var right = 0;
            for (int x = treeX + 1; x < width; x++)
            {
                right++;
                if (trees[x + treeY * width] >= tree)
                    break;
            }
#if LOG_SCORE_ADDITIONS
            Logger.DebugLine($"[{treeY},{treeX}] R {right}");
#endif
            return left * right;
        }

        private class ScenicScores
        {
            private int[] _buffer;
            private int _highest;
#if LOG_SCORE_ADDITIONS
            private int _width;
#endif

            public int this[int index] => _buffer[index];

            public int HighestScore => _highest;

            public ScenicScores(int width, int height)
            {
#if LOG_SCORE_ADDITIONS
                _width = width;
#endif
                _buffer = new int[width * height];
                _highest = 0;
            }

            public void AddView(int index, int viewValue)
            {
#if LOG_SCORE_ADDITIONS
                var x = index % _width;
                var y = index / _width;
                Logger.DebugLine($"[{y},{x}] + {viewValue}");
#endif
                var totalView = _buffer[index];
                if (totalView == 0)
                    totalView = viewValue;
                else
                    totalView *= viewValue;
                _buffer[index] = totalView;
                if (totalView > _highest)
                    _highest = totalView;
            }
        }
    }
}
