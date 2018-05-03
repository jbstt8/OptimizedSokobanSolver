namespace SokobanSolver.Constants
{
    public class AlgorithmConstants
    {
        // Node States and Movements
        public enum State { Initial, Goal, Invalid, Queued, Explored, CutOff }
        public enum Move { U, D, L, R }

        // Map Components
        public const char crate = 'c';
        public const char target = 't';
        public const char filledGoal = '$';
        public const char wall = 'w';
        public const char tile = '.';

        public const string FREE = "Free Play";

        // Available Algorithms
        public const string BFTS            = "Breadth-First Tree Search";
        public const string DFTS            = "Depth-First Tree Search";
        public const string IDDFTS          = "Iterative Deepening - Depth-First Tree Search";
        public const string GreBeFTS        = "Greedy Best-First Tree Search";
        public const string GreBeFGS        = "Greedy Best-First Graph Search";
        public const string BiDirGreBeFGS   = "Bi-Directional Greedy Best-First Graph Search";
        public const string AStarTS         = "A* Tree Search";
        public const string AStarGS         = "A* Graph Search";
    }
}
