using System.Collections.Generic;
using SokobanSolver.Interfaces;
using SokobanSolver.Models;

namespace SokobanSolver.Services
{
    public class OptimizationService
    {
        private static OptimizationService singleton;
        private List<IPreProcess> preProcessors = new List<IPreProcess>();
        private List<IMoveOptimize> moveOptimizers = new List<IMoveOptimize>();
        private ClusteringOptimizer clusteringOptimizer;
        private bool optimize = false;

        /// <summary>
        /// Prevents a default instance of the <see cref="OptimizationService"/> class from being created.
        /// </summary>
        private OptimizationService()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static OptimizationService Instance
        {
            get
            {
                return singleton ?? (singleton = new OptimizationService());
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="OptimizationService"/> is optimize.
        /// </summary>
        /// <value>
        ///   <c>true</c> if optimize; otherwise, <c>false</c>.
        /// </value>
        public bool Optimize
        {
            get
            {
                return this.optimize;
            }

            set
            {
                this.optimize = value;
            }
        }

        /// <summary>
        /// Gets the optimizer.
        /// </summary>
        /// <value>
        /// The optimizer.
        /// </value>
        public ClusteringOptimizer Optimizer
        {
            get
            {
                return this.clusteringOptimizer;
            }
        }

        /// <summary>
        /// Sets the optimizer.
        /// </summary>
        /// <param name="clusteringOptimizer">The clustering optimizer.</param>
        public void SetOptimizer(ClusteringOptimizer clusteringOptimizer)
        {
            this.clusteringOptimizer = clusteringOptimizer;
            this.clusteringOptimizer.Optimize = this.optimize;
        }

        /// <summary>
        /// Adds the optimizers.
        /// </summary>
        /// <param name="optimizers">The optimizers.</param>
        public void AddOptimizers(List<IOptimize> optimizers)
        {
            foreach (IOptimize optimizer in optimizers)
            {
                if (optimizer is IPreProcess preProcessor)
                {
                    this.preProcessors.Add(preProcessor);
                }

                if (optimizer is IMoveOptimize moveOptimize)
                {
                    this.moveOptimizers.Add(moveOptimize);
                }
            }
        }
    }
}
