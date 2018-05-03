using System.Collections.Generic;
using SokobanSolver.Constants;
using SokobanSolver.Helpers;
using SokobanSolver.Services;
using static SokobanSolver.Constants.AlgorithmConstants;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Sokoban Base
    /// </summary>
    /// <remarks>These are the methods that enforce the rules of Sokoban.</remarks>
    public class SokobanBase
    {
        protected readonly Move[] possibleMoves = { Move.D, Move.R, Move.U, Move.L };
        protected char[,] map;
        protected int[] mapDims;
        protected List<int[]> crateStarts;
        protected List<int[]> goalTargets;
        protected int[] startCoord;

        /// <summary>
        /// Initializes a new instance of the <see cref="SokobanBase"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public SokobanBase(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets)
        {
            this.map = map;
            this.mapDims = mapDims;

            this.goalTargets = targets;
            this.crateStarts = crates;
            this.startCoord = startingCoord;
        }

        /// <summary>
        /// Determines whether [is winning crateLocations].
        /// </summary>
        /// <param name="crateLocations">The crate locations.</param>
        /// <returns>
        /// True if this is a winning state
        /// </returns>
        public virtual bool IsWinningState(List<int[]> crateLocations)
        {
            return this.NumberOfGoalsAchieved(crateLocations) == this.goalTargets.Count;
        }

        /// <summary>
        /// Numbers the of goals achieved.
        /// </summary>
        /// <param name="crateLocations">The crate locations.</param>
        /// <returns>The number of achieved goals</returns>
        public virtual int NumberOfGoalsAchieved(List<int[]> crateLocations)
        {
            int count = 0;

            // Check the original targets for crates and count how many are complete.
            foreach (int[] target in this.goalTargets)
            {
                if (crateLocations.Exists(crate => crate[0] == target[0] && crate[1] == target[1]))
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// Moves to current node.
        /// </summary>
        /// <param name="movement">The movement.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="testmap">The testmap.</param>
        public void MoveToCurrentNode(List<Move> movement, ref int x, ref int y, ref List<int[]> crateLocations)
        {
            foreach (Move move in movement)
            {
                this.ExecuteMoveUpdatingEnvironment(move, ref x, ref y, ref crateLocations);
            }
        }

        /// <summary>
        /// Executes the move while updating environment.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="testMap">The test map.</param>
        public virtual void ExecuteMoveUpdatingEnvironment(Move move, ref int x, ref int y, ref List<int[]> crateLocations)
        {
            // Move cursor to the new location
            TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);

            int childX = x;
            int childY = y;

            // Determine if we walked into a crate
            if (crateLocations.Exists(crate => crate[0] == childX && crate[1] == childY))
            {
                // If so, push the crate.
                this.PushCrate(move, childX, childY, ref crateLocations);
            }
        }

        /// <summary>
        /// Evaluates the move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="node">The node.</param>
        /// <param name="crateMoved">if set to <c>true</c> [crate moved].</param>
        /// <returns>
        /// If the move is valid and if a crate was moved
        /// </returns>
        public bool MoveIsValid(Move move, BaseNode node, out bool crateMoved)
        {
            crateMoved = false;
            int x = node.X;
            int y = node.Y;
            List<int[]> crateLocations = new List<int[]>();
            node.CrateLocations.ForEach(crate => crateLocations.Add(new int[] { crate[0], crate[1] }));

            // Given a current location and action or "Move", determine our new x & y and return if we are inbounds or not
            bool inbounds = TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);

            // If out of bounds return that this is an invalid movement
            if (!inbounds) return false;

            // If we walked into a crate
            if (node.CrateLocations.Exists(crate => crate[0] == x && crate[1] == y))
            {
                // Push the crate and see if it moved
                crateMoved = this.PushCrate(move, x, y, ref crateLocations);

                // If the crate moved, this is a valid move, if it didn't this isn't valid
                if (crateMoved)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // We know our movement is in bounds and not into a crate; make sure it isn't into a wall
            if (this.map[x, y] == wall)
            {
                return false;
            }

            // We didn't walk into a crate, a wall, or out of bounds so this move must be valid.
            return true;
        }

        /// <summary>
        /// Pushes the crate.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="crates">The crates.</param>
        /// <returns>True if the crate successfully moved; else false.</returns>
        public bool PushCrate(Move move, int x, int y, ref List<int[]> crates)
        {
            // If we walked into a crate, determine if we can push it
            int[] pushedCrate = crates.Find(crate => crate[0] == x && crate[1] == y);

            // Calculate where the crate would need to move to
            bool inbounds = TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);

            // Make sure crate movement is allowed (i.e. within bounds and not into a wall or crate). 
            bool crateMoved = inbounds && !crates.Exists(crate => crate[0] == x && crate[1] == y) && this.map[x, y] != AlgorithmConstants.wall && !OptimizationService.Instance.Optimizer.IsCrateMoveSubOptimal(x, y);

            if (crateMoved)
            {
                pushedCrate[0] = x;
                pushedCrate[1] = y;
            }

            return crateMoved;
        }

        /// <summary>
        /// Moves the is valid.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="testMap">The test map.</param>
        /// <returns>If the move is valid with no regard for if the crate moved</returns>
        public virtual bool MoveIsValid(Move move, BaseNode node)
        {
            bool crateMoved = false;
            return this.MoveIsValid(move, node, out crateMoved);
        }
    }
}
