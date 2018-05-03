namespace SokobanSolver.Models
{
    using SokobanSolver.Constants;
    using System.Collections.Generic;
    using static Constants.AlgorithmConstants;

    /// <summary>
    /// Iterative Deepening - Depth-First Tree Search Crawler
    /// </summary>
    /// <seealso cref="SokobanSolver.Models.DFTSCrawler" />
    /// <remarks>This Class inherits the fundamentals of the Depth-First Tree Search Crawler</remarks>
    public class IDDFTSCrawler : DFTSCrawler
    {
        private int depth = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="IDDFTSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public IDDFTSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {
        }

        /// <summary>
        /// Crawls the specified solution.
        /// </summary>
        /// <returns>
        /// A solution if found
        /// </returns>
        public override BaseNode Crawl()
        {
            // Attempt a single Crawl and see if we find a solution
            BaseNode solution =  base.Crawl();

            if (solution == null)
            {
                // If we didn't find a solution, increase the depth-limit, put our starting node back in the queue
                // and recursively call Crawl until we find a solution.
                this.depth++;
                this.AddNodeToQueue(new BaseNode(this.startCoord[0], this.startCoord[1], this.crateStarts, 0, 0));
                solution = this.Crawl();
            }

            return solution;
        }

        /// <summary>
        /// Expands the node.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="testMap">The test map.</param>
        public override void ExpandNode(BaseNode currentNode)
        {
            // For the IDDFTS, we need to stop expanding nodes when we hit our "Depth-Limit"
            if (currentNode.PathCost >= this.depth)
            {
                return;
            }

            // If we haven't hit our depth limit, keep expanding the deepest nodes
            base.ExpandNode(currentNode);
        }
    }
}
