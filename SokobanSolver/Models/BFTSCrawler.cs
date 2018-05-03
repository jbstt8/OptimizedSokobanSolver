using SokobanSolver.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Breadth-First Tree Search Crawler
    /// </summary>
    /// <seealso cref="SokobanSolver.Models.BaseSokobanCrawler" />
    /// <remarks>All implementation of this Crawler is inherited from the Base. This class is just so people can find BFTS.</remarks>
    public class BFTSCrawler : BaseSokobanCrawler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BFTSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public BFTSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims,startingCoord, crates, targets)
        {

        }
    }
}
