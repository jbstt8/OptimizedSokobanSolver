using SokobanSolver.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SokobanSolver.Constants.AlgorithmConstants;

namespace SokobanSolver.Models
{
    /// <summary>
    /// The Base Node
    /// </summary>
    public class BaseNode
    {
        private Tuple<int, int> location;
        private List<int[]> crateLocations;
        private BaseNode parent;
        private Move? action;
        private int pathCost;
        private double heuristicValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseNode"/> class.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="state">The crateLocations.</param>
        /// <param name="pathCost">The path cost.</param>
        /// <param name="parent">The parent.</param>
        /// <param name="action">The action.</param>
        public BaseNode(int x, int y, List<int[]> state, int pathCost, double heuristicValue, BaseNode parent = null, Move? action = null)
        {
            this.location = new Tuple<int, int>(x, y);
            this.crateLocations = state;
            this.pathCost = pathCost;
            this.parent = parent;
            this.action = action;
            this.heuristicValue = heuristicValue;
        }

        /// <summary>
        /// Gets the location of this node compiled into a string key.
        /// </summary>
        /// <value>
        /// The key.
        /// </value>
        public string Key
        {
            get
            {
                return location.Item1.ToString() + " " + location.Item2.ToString();
            }
        }

        /// <summary>
        /// Gets the x.
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public int X
        {
            get
            {
                return this.location.Item1;
            }
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public int Y
        {
            get
            {
                return this.location.Item2;
            }
        }

        /// <summary>
        /// Gets the crateLocations.
        /// </summary>
        /// <value>
        /// The crateLocations.
        /// </value>
        public List<int[]> CrateLocations
        {
            get
            {
                return this.crateLocations;
            }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public BaseNode Parent
        {
            get
            {
                return this.parent;
            }
        }

        /// <summary>
        /// Gets the action.
        /// </summary>
        /// <value>
        /// The action.
        /// </value>
        public Move? Action
        {
            get
            {
                return this.action;
            }

            set
            {
                this.action = value;
            }
        }

        /// <summary>
        /// Gets the path cost.
        /// </summary>
        /// <value>
        /// The path cost.
        /// </value>
        public int PathCost
        {
            get
            {
                return this.pathCost;
            }
        }

        /// <summary>
        /// Gets the heuristic value.
        /// </summary>
        /// <value>
        /// The heuristic value.
        /// </value>
        public double HeuristicValue
        {
            get
            {
                return this.heuristicValue;
            }
        }
    }
}
