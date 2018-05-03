using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SokobanSolver.Constants;
using SokobanSolver.Helpers;
using static SokobanSolver.Constants.AlgorithmConstants;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Bi-Directional Greedy Best-First Graph Search Crawler
    /// </summary>
    public class BiDirectGreBeFGSCrawler : GreBeFGSCrawler
    {
        private BiDirectGreBeFGSCrawler reverseCrawler;
        private bool reversed;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="BiDirectGreBeFGSCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        /// <param name="crates">The crates.</param>
        /// <param name="targets">The targets.</param>
        /// <param name="reverse">if set to <c>true</c> [reverse].</param>
        public BiDirectGreBeFGSCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets, bool reverse = false) : base(map, mapDims, startingCoord, crates, targets)
        {
            // If not the reverse, then create and store a reverse
            if (!reverse)
            {
                // The reverse crawler has crate locations as targets and target locations as crates; also, its startCoords are the location of the last goal
                this.reverseCrawler = new BiDirectGreBeFGSCrawler(map, mapDims, null, targets, crates, true);
            }
            this.reversed = reverse;
        }

        /// <summary>
        /// Gets the queue.
        /// </summary>
        /// <value>
        /// The queue.
        /// </value>
        /// <remarks>This allows us to access the Reverse Crawler's queue externally</remarks>
        public List<BaseNode> Queue
        {
            get
            {
                return this.queue;
            }
        }

        /// <summary>
        /// Crunches this instance.
        /// </summary>
        /// <returns>
        /// Statistics from the Algorithms Success
        /// </returns>
        /// <remarks>
        /// I wanted Crunch seperate from Crawl. Crunch does the generic book keeping. Crawl does the actual algorithm.
        /// </remarks>
        public override string[] Crunch()
        {
            // The reverse Crawler start points are any valid points on the sides of the final crate position. So expand the target so we can get
            // all possible places the player could be on the side of it to have pushed it to where it is; put those in this queue.
            foreach (int[] crate in this.crateStarts)
            {
                this.reverseCrawler.ExpandNode(new BaseNode(crate[0], crate[1], this.reverseCrawler.crateStarts.ToList(), -1, 0));
            }

            return base.Crunch();
        }

        /// <summary>
        /// Crawls this instance.
        /// </summary>
        /// <returns>The solution if found</returns>
        /// <remarks>We have to override this so we can synchonize the forward and reverse crawlers</remarks>
        public override BaseNode Crawl()
        {
            if (!this.reversed)
            {
                BaseNode solution = null;

                while ((!this.IsQueueEmpty() || !reverseCrawler.IsQueueEmpty()) && solution == null)
                {
                    this.EvaluateAndExpandNode(this.PopQueue());
                    reverseCrawler.Crawl();

                    // If there exists a node that both crawlers have found where, combined, a solution is found; that is our winner.
                    List<BaseNode> similarNodes = this.ExploredSet.Where(en => reverseCrawler.ExploredSet.Any(rcEn => rcEn.Key == en.Key)).ToList();
                    foreach(BaseNode similarNode in similarNodes)
                    {
                        foreach(BaseNode possibleSolution in this.reverseCrawler.ExploredSet.Where(en => en.Key == similarNode.Key))
                        {
                            List<Move> movesToGoal = TreeHelpers.ActionTracker(possibleSolution);
                            movesToGoal.Reverse();
                            movesToGoal = TreeHelpers.ReflectMoves(movesToGoal);

                            int x = similarNode.X;
                            int y = similarNode.Y;
                            List<int[]> newChildState = new List<int[]>();
                            similarNode.CrateLocations.ForEach(state => newChildState.Add(new int[] { state[0], state[1] }));

                            this.MoveToCurrentNode(movesToGoal, ref x, ref y, ref newChildState);

                            if (IsWinningState(newChildState))
                            {
                                return InvertNodeHeritage(similarNode, possibleSolution.Parent);
                            }
                        }
                    }
                }

                return solution;
            }
            else
            {
                // If we are the reversed solution, we just need to take the normal steps and return if we found a solution.
                // all the Bi-Directional stuff is done in the non-reversed loop.
                BaseNode reverseSolution;
                reverseSolution = this.EvaluateAndExpandNode(this.PopQueue());
                return reverseSolution;
            }
        }

        /// <summary>
        /// Inverts the node heritage.
        /// </summary>
        /// <param name="basePath">The base path.</param>
        /// <param name="invertedPath">The inverted path.</param>
        /// <returns>The complete path</returns>
        private BaseNode InvertNodeHeritage(BaseNode basePath, BaseNode invertedPath)
        {
            if (invertedPath != null)
            {

                BaseNode newInvertedPath = invertedPath.Parent;

                BaseNode newBasePath = new BaseNode(
                    invertedPath.X,
                    invertedPath.Y,
                    invertedPath.CrateLocations,
                    this.CalculatePathCost(basePath, TreeHelpers.DetermineMove(basePath.X, basePath.Y, invertedPath.X, invertedPath.Y), invertedPath.CrateLocations),
                    this.CalculateHeuristicValue(basePath, TreeHelpers.DetermineMove(basePath.X, basePath.Y, invertedPath.X, invertedPath.Y), invertedPath.CrateLocations),
                    basePath,
                    TreeHelpers.DetermineMove(basePath.X, basePath.Y, invertedPath.X, invertedPath.Y));

                return this.InvertNodeHeritage(newBasePath, newInvertedPath);

            }

            return basePath;
        }

        /// <summary>
        /// Moves the is valid.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="node"></param>
        /// <returns>
        /// If the move is valid with no regard for if the crate moved
        /// </returns>
        public override bool MoveIsValid(AlgorithmConstants.Move move, BaseNode node)
        {
            if (!this.reversed)
            {
                return base.MoveIsValid(move, node);
            }
            else
            {
                int x = node.X;
                int y = node.Y;

                bool inbounds = TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);

                if (inbounds && this.map[x,y] != AlgorithmConstants.wall && !node.CrateLocations.Exists(crate => crate[0] == x && crate[1] == y))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Creates the child node.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="parent">The parent.</param>
        /// <param name="action">The action.</param>
        /// <param name="childState"></param>
        /// <returns>
        /// If the new child is the solution
        /// </returns>
        protected override BaseNode CreateChildNode(int x, int y, BaseNode parent, AlgorithmConstants.Move? action, List<int[]> childState)
        {
            if (parent.PathCost == -1)
            {
                parent = null;
                action = null;
            }

            BaseNode newChild = base.CreateChildNode(x, y, parent, action, childState);

            // If we are reversed, we need to add the previous, non-pulling child as well as add a child that pulled the crate
            if (this.reversed && parent != null)
            {
                // Find if there is a crate that could have been pulled
                int[] crateThatCouldBePulled = CrateCouldBePulled(newChild);

                if (crateThatCouldBePulled != null)
                {
                    // Clone the child state so we don't alter the other child
                    List<int[]> newChildState = new List<int[]>();
                    childState.ForEach(state => newChildState.Add(new int[] { state[0], state[1] }));
                    
                    // Get the crate out of the child state and update it to be where we used to be
                    int[] foundCrate = newChildState.Where(cs => cs[0] == crateThatCouldBePulled[0] && cs[1] == crateThatCouldBePulled[1]).First();
                    foundCrate[0] = parent.X;
                    foundCrate[1] = parent.Y;

                    // Add the crate with the moved crate in its state.
                    base.CreateChildNode(x, y, parent, action, newChildState);
                }
            }

            // Just return anything... we don't really care at this point.
            return newChild;
        }

        /// <summary>
        /// Crates the could be pulled.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>A crate that could have been pulled, if any.</returns>
        protected int[] CrateCouldBePulled(BaseNode node)
        {            
                // We need to calculate the difference of the move so we can use that to backtrack to find a crate
                int xDelta = node.Parent.X - node.X;
                int yDelta = node.Parent.Y - node.Y;
                int xTranslate = 0;
                int yTranslate = 0;
                
                // Either X or Y changed, never both. Determine which and set our translate coords.
                if (xDelta != 0)
                {
                    xTranslate = node.Parent.X + xDelta;
                    yTranslate = node.Parent.Y;
                }
                else
                {
                    xTranslate = node.Parent.X;
                    yTranslate = node.Parent.Y + yDelta;
                }

            // Determine if there was a crate directly behind us when we moved; if so, return it, otherwise, return null.
            if (node.Parent.CrateLocations.Any(loc => loc[0] == xTranslate && loc[1] == yTranslate))
            {
                int[] crate = node.Parent.CrateLocations.Where(loc => loc[0] == xTranslate && loc[1] == yTranslate).First();
                return crate;
            }
            else
            {
                return null;
            }
        }
    }
}
