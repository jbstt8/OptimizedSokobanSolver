namespace SokobanSolver.Constants
{
    public static class MenuConstants
    {
        public const int AnimationTick = 500;
        
        // View Constants for the available menus
        public const string FullRunner      = "********************************************************************\r\n";
        public const string EmptyRunner     = "**                                                                **\r\n";
        public const string ProgramTitle    = "**                    Sokoban Solving Algorithms                  **\r\n";
        public const string Underline       = "                     ----------------------------                   \r\n";       
        public const string Author          = "**                          by Ben Smith                          **\r\n";
        public const string Puzzle          = "                           Select a Puzzle                          \r\n";
        public const string Algorithm       = "                         Select an Algorithm                        \r\n";
        public const string Confirmation    = "                      Continue with Selections?                     \r\n";
        public const string Results         = "                              Results                               \r\n";
        public const string Help            = "                                Help                                \r\n";
        public const string Executing       = "                             Executing";
        public const string Selector        = "->";
        public const string Bump            = "  ";
        public const string MenuIndent      = "                  ";

        /// <summary>
        /// The Possible Menus
        /// </summary>
        public enum Menus
        {
            PuzzleSelection,
            AlgorithmSelection,
            SelectionConfirmation,
            ResultsReview,
            Help,
            Loading,
            FreePlay
        };

        /// <summary>
        /// The Possible Menu Actions
        /// </summary>
        public enum MenuActions
        {
            Up,
            Down,
            Left,
            Right,
            Enter,
            Back,
            Cancel,
            Exit,
            Optimization
        }
    }
}
