using System;
using System.Collections.Generic;
using System.Linq;

namespace SokobanSolver.Helpers
{
    public class MathHelpers
    {
        /// <summary>
        /// Gets the straight line distance.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>The Straight Line Distance</returns>
        public static double GetStraightLineDistance(int x2, int y2, int x1, int y1)
        {
            return GetStraightLineDistance(Convert.ToDouble(x1), Convert.ToDouble(y1), Convert.ToDouble(x2), Convert.ToDouble(y2));
        }

        /// <summary>
        /// Gets the straight line distance.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>The Straight Line Distance</returns>
        public static double GetStraightLineDistance(double x2, double y2, double x1, double y1)
        {
            return Math.Floor(Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2)));
        }



        /// <summary>
        /// Gets the manhattan distance.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>The Manhattan Distance</returns>
        public static double GetManhattanDistance(int x2, int y2, int x1, int y1)
        {
            return GetManhattanDistance(Convert.ToDouble(x1), Convert.ToDouble(y1), Convert.ToDouble(x2), Convert.ToDouble(y2));
        }


        /// <summary>
        /// Gets the manhattan distance.
        /// </summary>
        /// <param name="x1">The x1.</param>
        /// <param name="y1">The y1.</param>
        /// <param name="x2">The x2.</param>
        /// <param name="y2">The y2.</param>
        /// <returns>The Manhattan Distance</returns>
        public static double GetManhattanDistance(double x2, double y2, double x1, double y1)
        {
            double deltaX = Math.Abs(x1 - x2);
            double deltaY = Math.Abs(y1 - y2);

            return deltaX + deltaY;
        }

        public static int[,] GetManhattanDistanceMap(char[,] map, int[] mapDims, int[] endToStartWith)
        {
            
            return StepEvaluateAndContinue(map, mapDims, endToStartWith);
        }

        private static int[,] StepEvaluateAndContinue(char[,] map, int[] mapDims, int[] location)
        {
            int[,] distanceMap = new int[mapDims[0], mapDims[1]];

            try
            {
                List<Tuple<int[], int>> queue = new List<Tuple<int[], int>>() { new Tuple<int[], int>(location, 0) };

                while (queue.FirstOrDefault() is Tuple<int[], int> step)
                {
                    queue.Remove(step);

                    bool outOfBounds = step.Item1[0] < 0 || step.Item1[1] < 0 || step.Item1[0] >= mapDims[0] || step.Item1[1] >= mapDims[1] || map[step.Item1[0], step.Item1[1]] == 'w';

                    if (outOfBounds || distanceMap[step.Item1[0], step.Item1[1]] != 0 || (step.Item2 != 0 && map[step.Item1[0], step.Item1[1]] == 't'))
                    {
                        continue;
                    }
                    else
                    {
                        distanceMap[step.Item1[0], step.Item1[1]] = step.Item2;

                        int[] left = { step.Item1[0] - 1, step.Item1[1] };
                        int[] up = { step.Item1[0], step.Item1[1] - 1 };
                        int[] right = { step.Item1[0] + 1, step.Item1[1] };
                        int[] down = { step.Item1[0], step.Item1[1] + 1 };

                        queue.Add(new Tuple<int[], int>(left, step.Item2 + 1));
                        queue.Add(new Tuple<int[], int>(up, step.Item2 + 1));
                        queue.Add(new Tuple<int[], int>(right, step.Item2 + 1));
                        queue.Add(new Tuple<int[], int>(down, step.Item2 + 1));
                    }
                }

                return distanceMap;
            }
            catch (Exception ex)
            {
                return distanceMap;
            }
        }
    }
}
