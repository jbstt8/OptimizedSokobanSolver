using System.Collections.Generic;
using System.Linq;
using SokobanSolver.Constants;
using SokobanSolver.Helpers;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Greedy Best-First Tree Search Crawler
    /// </summary>
    /// <seealso cref="SokobanSolver.Models.BaseSokobanCrawler" />
    public class GreBeFTSCrawler : BaseSokobanCrawler
    {
        private readonly Dictionary<string, int[,]> distanceCache = new Dictionary<string, int[,]>();

        /// <summary>
        /// Initializes a new instance of the <see cref="GreBeFTSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public GreBeFTSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {

        }

        /// <summary>
        /// Expands the node.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="currentNode">The current node.</param>
        /// <param name="testMap">The test map.</param>
        public override void ExpandNode(BaseNode currentNode)
        {
            base.ExpandNode(currentNode);

            // After expanding the node and adding all of its possible children, we prioritize the queue.
            // This is done here to avoid incurring it everytime we do an add (i.e. add them all then prioritize).
            this.SortQueue();
        }

        /// <summary>
        /// Sorts the queue.
        /// </summary>
        public virtual void SortQueue()
        {
            this.queue = this.queue.OrderBy(n => n.PathCost + n.HeuristicValue).ToList();
        }

        /// <summary>
        /// Calculates the node cost. This is a heuristic or cost function.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="action">The action.</param>
        /// <returns>
        /// The Cost of the Child Node
        /// </returns>
        protected override double CalculateHeuristicValue(BaseNode parent, AlgorithmConstants.Move? action, List<int[]> childState)
        {
            double distance = this.CalcTotalDistances(childState, this.goalTargets);
            int goalsUnAttained = this.goalTargets.Count - this.NumberOfGoalsAchieved(childState);

            return distance + (goalsUnAttained * 10);
        }

        /// <summary>
        /// Calculates the total distances from goals with bonus.
        /// </summary>
        /// <param name="starts">The starts.</param>
        /// <param name="ends">The ends.</param>
        /// <param name="bonus">The bonus.</param>
        /// <returns>The combined distance from each start to each end </returns>
        protected virtual double CalcTotalDistances(List<int[]> starts, List<int[]> ends)
        {
            double totalDistance = 0;

            foreach (int[] end in ends)
            {
                int[,] distanceMap;

                if (!this.distanceCache.ContainsKey(end[0].ToString() + end[1].ToString()))
                {
                    distanceMap = MathHelpers.GetManhattanDistanceMap(this.map, this.mapDims, end);
                    this.distanceCache.Add(end[0].ToString() + end[1].ToString(), distanceMap);
                }
                else
                {
                    distanceMap = this.distanceCache[end[0].ToString() + end[1].ToString()];
                }

                starts.ForEach(start => totalDistance += distanceMap[start[0], start[1]]);
            }
            
            return totalDistance;
        }
    }
}
