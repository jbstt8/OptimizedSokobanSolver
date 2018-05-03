using System;
using System.Collections.Generic;
using System.Linq;
using SokobanSolver.Constants;
using SokobanSolver.Helpers;
using static SokobanSolver.Constants.AlgorithmConstants;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Sokoban Free Player class
    /// </summary>
    /// <remarks>This is a 'for fun' addition to allow users to play the levels by hand</remarks>
    public class SokobanFreePlayer : SokobanBase
    {
        private string wall = "██";
        private string crate = "[]";
        private string target = "░░";
        private string tile = "  ";
        private string filledGoal = "≡≡";
        private string player = "Å`";
        private double windowScale = 11;

        private int currentX;
        private int currentY; 
        private char[,] playMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="SokobanFreePlayer"/> class.
        /// </summary>
        /// <param name="map">The map.</param>
        /// <param name="mapDims">The map dims.</param>
        /// <param name="startingCoord">The starting coord.</param>
        public SokobanFreePlayer(char[,] map, int[] mapDims, int[] startingCoord, List<int[]> crates, List<int[]> targets) : base(map, mapDims, startingCoord, crates, targets)
        {
            // In algo-play, we strip out the crates, in freeplay, we use a map as the constant state so add em back.
            this.crateStarts.ForEach(crate => this.map[crate[0], crate[1]] = this.goalTargets.Any(target => target[0] == crate[0] && target[1] == crate[1]) ? AlgorithmConstants.filledGoal : AlgorithmConstants.crate);
            this.ResetGame();
        }

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="move">The move.</param>
        public void MovePlayer(Move move)
        {
            if (this.EvaluateMove(move, currentX, currentY, this.playMap) == State.Invalid)
            {
                return;
            }

            this.ExecuteMoveUpdatingEnvironment(move, ref currentX, ref currentY, this.map, ref this.playMap);
        }

        /// <summary>
        /// Resets the game.
        /// </summary>
        public void ResetGame()
        {
            this.currentX = startCoord[0];
            this.currentY = startCoord[1];
            this.playMap = (char[,])this.map.Clone();
        }

        /// <summary>
        /// Renders the crateLocations of the map.
        /// </summary>
        public void RenderMapState()
        {
            int renderX = this.currentX - Convert.ToInt32(Math.Floor(this.windowScale / 2));
            int renderY = this.currentY - Convert.ToInt32(Math.Floor(this.windowScale / 2));
            int renderXMax = renderX + Convert.ToInt32(this.windowScale);
            int renderYMax = renderY + Convert.ToInt32(this.windowScale);

            for (int row = renderY; row < renderYMax; row++)
            {
                string renderString = string.Empty;

                for (int column = renderX; column < renderXMax; column++)
                {
                    if (column == currentX && row == currentY)
                    {
                        renderString += this.player;
                        continue;
                    }

                    if (column < 0 || column >= this.mapDims[0] || row < 0 || row >= this.mapDims[1])
                    {
                        renderString += this.wall;
                        continue;
                    }

                    switch(this.playMap[column, row])
                    {
                        case AlgorithmConstants.crate:
                            renderString += this.crate;
                            break;
                        case AlgorithmConstants.wall:
                            renderString += this.wall;
                            break;
                        case AlgorithmConstants.tile:
                            renderString += this.tile;
                            break;
                        case AlgorithmConstants.target:
                            renderString += this.target;
                            break;
                        case AlgorithmConstants.filledGoal:
                            renderString += this.filledGoal;
                            break;
                    }
                }

                Console.WriteLine(MenuConstants.MenuIndent + MenuConstants.Bump + MenuConstants.Bump + renderString);
            }

            if (this.IsWinningState(this.playMap))
            {
                Console.WriteLine(MenuConstants.MenuIndent + "            You Won!!");
                Console.WriteLine(MenuConstants.MenuIndent + "Hit 'R' to reset. Hit 'E' to exit.");
            }
            else
            {
                Console.WriteLine("        Use arrows to move. Hit 'R' to reset. Hit 'E' to exit.");
                Console.WriteLine("                           Player = " + this.player);
                Console.WriteLine("                           Crate  = " + this.crate);
                Console.WriteLine("                           Target = " + this.target);
            }

        }

        /// <summary>
        /// Evaluates the move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="testMap">The test map.</param>
        /// <returns>The state of the next node after a move</returns>
        private State EvaluateMove(Move move, int x, int y, char[,] testMap)
        {
            // Given a current location and action or "Move", determine our new x & y and return if we are inbounds or not
            bool inbounds = TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);

            // If out of bounds return that this is an invalid movement
            if (!inbounds) return State.Invalid;

            // If in bounds, return the validity of the move based on if you walking into walks, crates, crates in-front of walls, etc.
            switch (testMap[x, y])
            {
                case AlgorithmConstants.tile:
                    return State.Queued;
                case AlgorithmConstants.filledGoal:
                case AlgorithmConstants.crate:
                    inbounds = TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);
                    if (!inbounds) return State.Invalid;

                    switch (testMap[x, y])
                    {
                        case AlgorithmConstants.tile:
                        case AlgorithmConstants.target:
                            return State.Queued;
                        default:
                            return State.Invalid;
                    }
                case AlgorithmConstants.wall:
                    return State.Invalid;
                case AlgorithmConstants.target:
                    return State.Goal;
                default:
                    return State.Invalid;
            }
        }

        /// <summary>
        /// Executes the move while updating environment.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="testMap">The test map.</param>
        private void ExecuteMoveUpdatingEnvironment(Move move, ref int x, ref int y, char[,] map, ref char[,] testMap)
        {
            // Move cursor to the new location
            TreeHelpers.CalculateMove(move, ref x, ref y, this.mapDims[0], this.mapDims[1]);

            // Determine if our movement would have pushed a crate
            bool pushCrate = testMap[x, y] == AlgorithmConstants.crate || testMap[x, y] == AlgorithmConstants.filledGoal;

            // Figure out what the spot I just move from was before we ever started
            char previous = map[x, y];

            // If it was a crate, make it a tile (cuz it was obviously moved) and put a target there if we moved off the 't'
            char replacement = (previous == AlgorithmConstants.target || previous == AlgorithmConstants.filledGoal) ? AlgorithmConstants.target : AlgorithmConstants.tile;

            // Put whatever should replace where the crate was
            testMap[x, y] = replacement;

            // Move the crate to its new home
            if (pushCrate)
            {
                int x2 = x;
                int y2 = y;
                TreeHelpers.CalculateMove(move, ref x2, ref y2, this.mapDims[0], this.mapDims[1]);
                testMap[x2, y2] = testMap[x2, y2] == AlgorithmConstants.target ? AlgorithmConstants.filledGoal : AlgorithmConstants.crate;
            }
        }

        /// <summary>
        /// Determines whether [is winning state] [the specified current node].
        /// </summary>
        /// <param name="testMap">The test map.</param>
        /// <returns>
        /// True if this path has won the game
        /// </returns>
        private bool IsWinningState(char[,] testMap)
        {
            bool success = true;

            // Check the original targets for crates. If one of them isn't a crate, we failed; break and move on.
            foreach (int[] target in this.goalTargets)
            {
                if (testMap[target[0], target[1]] != AlgorithmConstants.filledGoal)
                {
                    success = false;
                    break;
                }
            }

            return success;
        }
    }
}
