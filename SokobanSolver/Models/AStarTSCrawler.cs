using System.Collections.Generic;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The A* Tree Search Crawler
    /// </summary>
    public class AStarTSCrawler : GreBeFTSCrawler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AStarTSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        /// <param name="crates">The starting crate positions</param>
        /// <param name="targets">The starting target positions</param>
        public AStarTSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {
        }
    }
}
