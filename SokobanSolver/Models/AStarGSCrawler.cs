using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SokobanSolver.Models
{
    public class AStarGSCrawler : AStarTSCrawler
    {
        // The Explored or "Closed" set
        public List<BaseNode> ExploredSet = new List<BaseNode>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AStarGSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        /// <param name="crates">The starting crate positions</param>
        /// <param name="targets">The starting target positions</param>
        public AStarGSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {
        }

        /// <summary>
        /// Adds the child to queue.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AddNodeToQueue(BaseNode node)
        {
            // Check our "Explored" queue and ensure that we haven't been in this state before. If we have, don't add this to the Frontier
            if (this.ExploredSet.Any(exploredNode => exploredNode.CrateLocations.TrueForAll(e => node.CrateLocations.Any(cl => cl[0] == e[0] && cl[1] == e[1])) && exploredNode.Key == node.Key))
            {
                return;
            }

            // Add our node to the Explored Queue to ensure we don't re-do this evaluation
            this.ExploredSet.Add(node);

            // Add node to the queue per usual
            base.AddNodeToQueue(node);
        }
    }
}
