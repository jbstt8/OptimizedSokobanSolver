using System;
using System.Collections.Generic;
using System.Linq;
using SokobanSolver.Constants;
using SokobanSolver.Helpers;
using SokobanSolver.Interfaces;
using static SokobanSolver.Constants.AlgorithmConstants;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Sokoban Crawler Base
    /// </summary>
    /// <remarks>This Class naturally implements a Breadth-First Tree Search Crawler and is overridden for other implementations. 
    /// Furthermore, these classes are all the base "Crawler" methods.</remarks>
    public class BaseSokobanCrawler : SokobanBase, ISokobanCrawler
    {
        protected double elapsedTime;
        protected DateTime endTime;
        protected List<BaseNode> queue = new List<BaseNode>();
        protected int stepCost = 1;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseSokobanCrawler"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public BaseSokobanCrawler(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {
            
        }

        /// <summary>
        /// Gets the start time.
        /// </summary>
        /// <value>
        /// The start time.
        /// </value>
        public virtual double ElapsedTime
        {
            get
            {
                return this.elapsedTime;
            }
        }

        /// <summary>
        /// Gets or sets the possible moves.
        /// </summary>
        /// <value>
        /// The possible moves.
        /// </value>
        public virtual Move[] PossibleMoves
        {
            get
            {
                return this.possibleMoves;
            }
        }

        /// <summary>
        /// Crunches this instance.
        /// </summary>
        /// <returns>
        /// Statistics from the Algorithms Success
        /// </returns>
        /// <remarks>I wanted Crunch seperate from Crawl. Crunch does the generic book keeping. Crawl does the actual algorithm.</remarks>
        public virtual string[] Crunch()
        {
            DateTime start = DateTime.Now;

            // Create our Starting Node
            this.AddNodeToQueue(new BaseNode(this.startCoord[0], this.startCoord[1], this.crateStarts.ToList(), 0, 0));

            // This is the Method where the Algorithm runs
            BaseNode solution = this.Crawl();

            // We broke out because there was either nothing left to look at or we found what we were looking for; Say done and report the results.
            DateTime stop = DateTime.Now;

            TimeSpan time = stop - start;

            this.elapsedTime = time.TotalMilliseconds;
            string[] results = this.CalculateResults(solution);

            return results;
        }

        /// <summary>
        /// Crawls the specified solution.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>A solution if found</returns>
        public virtual BaseNode Crawl()
        {
            BaseNode solution = null;
            
            // While we haven't traversed the whole tree and we haven't found a solution
            while (!this.IsQueueEmpty() && solution == null)
            {
                solution = this.EvaluateAndExpandNode(this.PopQueue());
            }

            return solution;
        }

        /// <summary>
        /// Expands the node.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="currentNode">The current node.</param>
        /// <param name="testMap">The test map.</param>
        public virtual void ExpandNode(BaseNode currentNode)
        {
            // Not a winner, expand by evaluating the validity of each action from our currentNode
            foreach (Move possibleMove in this.PossibleMoves)
            {
                // This returns the valid actions for this nodes location (i.e. the ACTIONS method from book).
                if (this.MoveIsValid(possibleMove, currentNode))
                {
                    int childX = currentNode.X;
                    int childY = currentNode.Y;

                    List<int[]> childCrateLocations = new List<int[]>();
                    currentNode.CrateLocations.ForEach(crate => childCrateLocations.Add(new int[] { crate[0], crate[1] }));

                    this.ExecuteMoveUpdatingEnvironment(possibleMove, ref childX, ref childY, ref childCrateLocations);

                    this.CreateChildNode(childX, childY, currentNode, possibleMove, childCrateLocations);
                }
            }
        }

        /// <summary>
        /// Expands the node.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        public virtual BaseNode EvaluateAndExpandNode(BaseNode currentNode)
        {
            if (currentNode == null)
            {
                return null;
            }
            
            // Now check if this is our winner
            if (this.IsWinningState(currentNode.CrateLocations))
            {
                return currentNode;
            }

            this.ExpandNode(currentNode);

            return null;
        }

        /// <summary>
        /// Creates the child node.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="action">The action.</param>
        /// <param name="state">The crateLocations.</param>
        /// <returns>If the new child is the solution</returns>
        protected virtual BaseNode CreateChildNode(int x, int y, BaseNode parent, Move? action, List<int[]> childState)
        {
            int pathCost = this.CalculatePathCost(parent, action, childState);
            double heuristic = this.CalculateHeuristicValue(parent, action, childState);

            BaseNode childNode = new BaseNode(x, y, childState, pathCost, heuristic , parent, action);

            this.AddNodeToQueue(childNode);

            return childNode;
        }

        protected virtual int CalculatePathCost(BaseNode parent, Move? action, List<int[]> childState)
        {
            return this.stepCost + (parent != null ? parent.PathCost : 0);
        }
        
        /// <summary>
        /// Calculates the node cost. This is a heuristic or cost function.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <param name="action">The action.</param>
        /// <param name="childState">The child state</param>
        /// <returns>The Cost of the Child Node</returns>
        protected virtual double CalculateHeuristicValue(BaseNode parent, Move? action, List<int[]> childState)
        {
            return 0;
        }

        /// <summary>
        /// Determines whether [is queue empty].
        /// </summary>
        /// <returns>True if empty</returns>
        protected bool IsQueueEmpty()
        {
            return !this.queue.Any();
        }

        /// <summary>
        /// Pops the queue.
        /// </summary>
        /// <returns>The first item of the queue</returns>
        protected virtual BaseNode PopQueue()
        {
            BaseNode first = this.queue.FirstOrDefault();
            if (first != null) this.queue.RemoveAt(0);

            return first;
        }

        /// <summary>
        /// Adds the child to queue.
        /// </summary>
        /// <param name="node">The node.</param>
        protected virtual void AddNodeToQueue(BaseNode node)
        {
            // The default Add method uses FIFO; this can be overridden to use LIFO with insert(0).
            this.queue.Add(node);
        }

        /// <summary>
        /// Calculates the results.
        /// </summary>
        /// <param name="solution">The solution.</param>
        /// <returns>The results of the exploration</returns>
        private string[] CalculateResults(BaseNode solution)
        {
            if (solution == null) return new[] { "Failure. No solution found." };

            List<string> compiledFinalState = new List<string>();

            string elapsedTime = this.ElapsedTime.ToString();

            List<Move> movesToSolution = TreeHelpers.ActionTracker(solution);

            string numberOfMoves = movesToSolution.Count.ToString();
            string moveBreakdown = movesToSolution.Aggregate(string.Empty, (current, move) => current + move.ToString());
            string prettyDims = this.mapDims[0] + " " + this.mapDims[1];

            compiledFinalState.Add(elapsedTime);
            compiledFinalState.Add(numberOfMoves);
            compiledFinalState.Add(moveBreakdown);
            compiledFinalState.Add(prettyDims);
            compiledFinalState.Add(solution.Key);

            // Put the crates in the map for display
            foreach(int[] crate in solution.CrateLocations)
            {
                this.map[crate[0], crate[1]] = AlgorithmConstants.crate;
            }

            for (int row = 0; row < this.mapDims[1]; row++)
            {
                string temp = string.Empty;
                for (int column = 0; column < this.mapDims[0]; column++)
                {
                    temp += this.map[column, row];
                }

                compiledFinalState.Add(temp);
            }

            return compiledFinalState.ToArray();
        }
    }
}
