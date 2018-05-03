namespace SokobanSolver.Models
{
    using SokobanSolver.Constants;
    using System.Collections.Generic;
    using static Constants.AlgorithmConstants;

    /// <summary>
    /// The Depth First Tree Search Crawler
    /// </summary>
    /// <seealso cref="SokobanSolver.Models.BaseSokobanCrawler" />
    public class DFTSCrawler : BaseSokobanCrawler
    {
        private readonly Move[] possibleMoves = { Move.L, Move.U, Move.R, Move.D };

        /// <summary>
        /// Initializes a new instance of the <see cref="DFTSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public DFTSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {
        }

        /// <summary>
        /// Gets or sets the possible moves.
        /// </summary>
        /// <value>
        /// The possible moves.
        /// </value>
        public override Move[] PossibleMoves
        {
            get
            {
               return this.possibleMoves; 
            }
        }

        /// <summary>
        /// Adds the child to queue.
        /// </summary>
        /// <param name="node">The node.</param>
        protected override void AddNodeToQueue(BaseNode node)
        {
            // Implements a LIFO queue
            this.queue.Insert(0, node);
        }
    }
}
