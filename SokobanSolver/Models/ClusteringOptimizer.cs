using System;
using System.Collections.Generic;
using System.Linq;

namespace SokobanSolver.Models
{
    public class ClusteringOptimizer
    {
        private bool optimize = true;
        private Dictionary<int, Dictionary<int, string>> crateHoles = new Dictionary<int, Dictionary<int, string>>();

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ClusteringOptimizer"/> is optimize.
        /// </summary>
        /// <value>
        ///   <c>true</c> if optimize; otherwise, <c>false</c>.
        /// </value>
        public bool Optimize
        {
            get
            {
                return this.optimize;
            }

            set
            {
                this.optimize = value;
            }
        }

        /// <summary>
        /// Pres the process.
        /// </summary>
        /// <param name="gameMap">The game map.</param>
        /// <param name="crates">The crates.</param>
        /// <param name="targets">The targets.</param>
        public void PreProcess(char[,] gameMap, List<int[]> crates, List<int[]> targets)
        {
            try
            {
                if (!this.optimize)
                {
                    return;
                }

                int width = gameMap.GetLength(0);
                int height = gameMap.GetLength(1);

                char[,] gradeMap = new char[width, height];

                int[] colGrateStartArray = new int[width];

                for (int i = 0; i < width; i++)
                {
                    colGrateStartArray[i] = -1;
                }

                // Walk through every row
                for (int y = 0; y < height; y++)
                {
                    int rowGrateStart = -1;

                    // Walk through every column
                    for (int x = 0; x < width; x++)
                    {
                        // If this is a wall, then we should have already closed the Grate above ot to its side; reset the grates.
                        // If this is a target, we don't count it as a Corner or a Grate; reset the grate view.
                        if (gameMap[x,y] == 'w' || targets.Any(t => t[0] == x && t[1] == y))
                        {
                            rowGrateStart = -1;
                            colGrateStartArray[x] = -1;
                            continue;
                        }

                        int lookUp = -1;
                        int lookDown = 1;
                        int lookLeft = -1;
                        int lookRight = 1;

                        if (lookUp + y < 0 || gameMap[x,lookUp + y] == 'w' || lookDown + y >= height || gameMap[x, lookDown + y] == 'w')
                        {
                            if (lookLeft + x < 0 || gameMap[lookLeft + x, y] == 'w' || lookRight + x >= width || gameMap[lookRight + x, y] == 'w')
                            {
                                if (rowGrateStart > -1)
                                {
                                    this.AddCrateHoles(y, rowGrateStart, x - rowGrateStart);
                                    rowGrateStart = -1;
                                }
                                else
                                {
                                    rowGrateStart = x;
                                }

                                if (colGrateStartArray[x] > -1)
                                {
                                    this.AddCrateHoles(x, colGrateStartArray[x], y - colGrateStartArray[x], false);
                                    colGrateStartArray[x] = -1;
                                }
                                else
                                {
                                    colGrateStartArray[x] = y;
                                }

                                this.AddCrateHole(x, y);
                            }
                        }
                        else
                        {
                            colGrateStartArray[x] = -1;
                            rowGrateStart = -1;
                        }

                        //targets.Any(t => t[0] == x && t[1] == y)
                    }
                }

                // Logging code to write out the Optimized Map.
                //for (int y = 0; y < height; y++)
                //{
                //    string test = "";
                //    for (int x = 0; x < width; x++)
                //    {
                //        test += this.IsCrateMoveSubOptimal(x,y) ? "X" : gameMap[x,y].ToString();
                //    }

                //    Console.WriteLine(test + "\n");
                //}
            }
            catch(Exception ex)
            {

            }
        }

        /// <summary>
        /// Determines whether [is crate move sub optimal] [the specified x].
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        ///   <c>true</c> if [is crate move sub optimal] [the specified x]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsCrateMoveSubOptimal(int x, int y)
        {
            if (this.optimize && this.crateHoles?.ContainsKey(x) == true)
            {
                return this.crateHoles[x].ContainsKey(y);
            }

            return false;
        }

        /// <summary>
        /// Adds the crate hole.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        protected void AddCrateHole(int x, int y)
        {
            if (!this.crateHoles.ContainsKey(x))
            {
                this.crateHoles.Add(x, new Dictionary<int, string>() { { y, "-" } });
            }
            else
            {
                if (!this.crateHoles[x].ContainsKey(y))
                {
                    this.crateHoles[x].Add(y, "-");
                }
            }
        }

        /// <summary>
        /// Adds the crate holes.
        /// </summary>
        /// <param name="rowOrCol">The row or col.</param>
        /// <param name="start">The start.</param>
        /// <param name="length">The length.</param>
        /// <param name="horizontal">if set to <c>true</c> [horizontal].</param>
        protected void AddCrateHoles(int rowOrCol, int start, int length, bool horizontal = true)
        {
            for (int i = start; i <= start + length; i++)
            {
                int x = horizontal ? i : rowOrCol;
                int y = horizontal ? rowOrCol : i;

                this.AddCrateHole(x, y);
            }
        }

        private char[,] Rotate90Degrees(char[,] node)
        {


            return node;
        }
    }
}
