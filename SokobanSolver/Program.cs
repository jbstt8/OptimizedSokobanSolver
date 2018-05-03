using System;
using SokobanSolver.Constants;
using SokobanSolver.Controllers;

namespace SokobanSolver
{
    class Program
    {
        /// <summary>
        /// Mains the specified arguments.
        /// </summary>
        /// <param name="args">The arguments.</param>
        static void Main(string[] args)
        {
            try
            {
                MenuController controller;

                // The TAs requested a method to short circuit the UI and load straight into a test using CMD-line. This check allows that from the Main.
                if (args.Length > 0)
                {
                    string algo = args[1].ToLower();
                    switch (algo)
                    {
                        case "bfts":
                            algo = AlgorithmConstants.BFTS;
                            break;
                        case "dfts":
                            algo = AlgorithmConstants.DFTS;
                            break;
                        case "iddfts":
                            algo = AlgorithmConstants.IDDFTS;
                            break;
                        case "grebefts":
                            algo = AlgorithmConstants.GreBeFTS;
                            break;
                        case "grebefgs":
                            algo = AlgorithmConstants.GreBeFGS;
                            break;
                        case "bidirgrebefgs":
                            algo = AlgorithmConstants.BiDirGreBeFGS;
                            break;
                        case "a*ts":
                            algo = AlgorithmConstants.AStarTS;
                            break;
                        case "a*gs":
                            algo = AlgorithmConstants.AStarGS;
                            break;
                        default:
                            throw new Exception("Non-valid input");
                    }

                    controller = new MenuController(args[0], algo);
                }
                else
                {
                    controller = new MenuController();
                }

                Console.CursorVisible = false;
                Console.Title = "Sokoban Solver";

                // Takes User Input - This for-loop stays open until the user closes the program, hits escape, or selects exit from the menu.
                for (;;)
                {
                    System.ConsoleKeyInfo key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Up);
                            break;
                        case ConsoleKey.DownArrow:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Down);
                            break;
                        case ConsoleKey.LeftArrow:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Left);
                            break;
                        case ConsoleKey.RightArrow:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Right);
                            break;
                        case ConsoleKey.R:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Cancel);
                            break;
                        case ConsoleKey.E:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Back);
                            break;
                        case ConsoleKey.Enter:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Enter);
                            break;
                        case ConsoleKey.O:
                            controller.ProcessUserInput(MenuConstants.MenuActions.Optimization);
                            break;
                        case ConsoleKey.Escape:
                            return;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                Console.WriteLine("USAGE: SokobanSolver.exe [puzzleFile] [algorithm]");
                Console.WriteLine("    ex. SokobanSolver.exe puzzle1.txt bfts\r\n ");
                Console.WriteLine("    Algorithms:");
                Console.WriteLine("        \"BFTS\" = Breadth - First Tree Search ");
                Console.WriteLine("        \"DFTS\" = Depth - First Tree Search");
                Console.WriteLine("        \"IDDFTS\" = Iterative Deepening - Depth First Tree Search ");
                Console.ReadKey();
            }
        }
    }
}
