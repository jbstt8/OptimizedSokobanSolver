using SokobanSolver.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SokobanSolver.Constants.AlgorithmConstants;

namespace SokobanSolver.Helpers
{
    /// <summary>
    /// The Tree Helpers Class
    /// </summary>
    /// <remarks>These methods are true of any tree, not just Sokoban. So they are broken out of the Sokoban classes.</remarks>
    public static class TreeHelpers
    {
        /// <summary>
        /// Actions the tracker.
        /// </summary>
        /// <param name="currentNode">The current node.</param>
        /// <param name="moveList">The move list.</param>
        /// <returns>The list of actions that lead U to this node</returns>
        public static List<Move> ActionTracker(BaseNode currentNode, List<Move> moveList = null)
        {
            if (moveList == null)
            {
                moveList = new List<Move>();
            }

            // If not the initial node, walk down this nodes parent nodes until you get to the initial compiling a list of actions taken to get here
            if (currentNode.Parent == null) return moveList;

            // Insert the moves rather than add them to insure they are in chronological order when returned
            moveList.Insert(0, (Move)currentNode.Action);

            return TreeHelpers.ActionTracker(currentNode.Parent, moveList);
        }

        /// <summary>
        /// Calculates the move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>[TRUE] if in bounds. [FALSE] if out of bounds</returns>
        public static bool CalculateMove(Move move, ref int x, ref int y, int mapWidth, int mapHeight)
        {
            // Update x and y
            switch (move)
            {
                case Move.L:
                    x -= 1;
                    break;
                case Move.U:
                    y -= 1;
                    break;
                case Move.R:
                    x += 1;
                    break;
                case Move.D:
                    y += 1;
                    break;
            }

            // If x and y are in-bounds, return true 
            if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Determines the move.
        /// </summary>
        /// <param name="prevX">The previous x.</param>
        /// <param name="prevY">The previous y.</param>
        /// <param name="currX">The curr x.</param>
        /// <param name="currY">The curr y.</param>
        /// <returns>What move had occcured</returns>
        /// <exception cref="Exception">Invalid Move </exception>
        public static Move DetermineMove(int prevX, int prevY, int currX, int currY)
        {
            // We need to calculate the difference of the move so we can use that to backtrack to find a crate
            int xDelta = currX - prevX;
            int yDelta = currY - prevY;

            // Either X or Y changed, never both. Determine which and set our translate coords.
            if (xDelta != 0)
            {
                switch (xDelta)
                {
                    case -1:
                        return Move.L;
                    case 1:
                        return Move.R;
                }
            }
            else
            {
                switch (yDelta)
                {
                    case -1:
                        return Move.U;
                    case 1:
                        return Move.D;
                }
            }

            throw new Exception("Invalid Move ");
        }

        /// <summary>
        /// Reflects the moves.
        /// </summary>
        /// <param name="moves">The moves.</param>
        /// <returns>A reflected moves list</returns>
        public static List<Move> ReflectMoves(List<Move> moves)
        {
            List<Move> reflectedMoves = new List<Move>();
            foreach (Move move in moves)
            {
                switch (move)
                {
                    case Move.D:
                        reflectedMoves.Add(Move.U);
                        break;
                    case Move.U:
                        reflectedMoves.Add(Move.D);
                        break;
                    case Move.L:
                        reflectedMoves.Add(Move.R);
                        break;
                    case Move.R:
                        reflectedMoves.Add(Move.L);
                        break;
                }
            }

            return reflectedMoves;
        }
    }
}
