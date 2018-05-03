using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SokobanSolver.Constants;
using SokobanSolver.Interfaces;
using SokobanSolver.Models;
using SokobanSolver.Services;
using static SokobanSolver.Constants.MenuConstants;

namespace SokobanSolver.Controllers
{
    /// <summary>
    /// The Menu Controller
    /// </summary>
    public class MenuController
    {
        private List<string> puzzles = new List<string>();
        private List<string> algorithms = new List<string>()
        {
            AlgorithmConstants.BFTS,
            AlgorithmConstants.DFTS,
            AlgorithmConstants.IDDFTS,
            AlgorithmConstants.GreBeFTS,
            AlgorithmConstants.GreBeFGS,
            AlgorithmConstants.BiDirGreBeFGS,
            AlgorithmConstants.AStarTS,
            AlgorithmConstants.AStarGS,
            AlgorithmConstants.FREE
        };
        private string puzzlesDir = string.Empty;
        private string solutionsDir = string.Empty;
        private Menus currentMenu = Menus.PuzzleSelection;
        private List<Menus> menuHistory = new List<Menus>();
        private int currentOption = 0;
        private int menuDisplayMax = 4;
        private int menuDisplayIndex = 0;
        private int totalOptions = 0;
        private string selectedPuzzle = string.Empty;
        private string selectedAlgorithm = string.Empty;
        private List<string> currentOptions = new List<string>();
        private object threadLocker = new object();
        private Func<string[]> algoHandler;

        private bool isBusy;
        private System.Timers.Timer animationTimer = new System.Timers.Timer();
        private string animationProgress;
        private int processCounter = 0;
        private List<MenuActions> bannedActionsList = new List<MenuActions>();
        private bool freePlay = false;
        private SokobanFreePlayer freePlayState;

        /// <summary>
        /// Initializes a new instance of the <see cref="MenuController"/> class.
        /// </summary>
        public MenuController(string puzzle = null, string algorithm = null)
        {
            string path = System.IO.Directory.GetCurrentDirectory();
            this.puzzlesDir = path + "\\Puzzles";
            this.solutionsDir = path + "\\Solutions";

            this.animationTimer.Interval = MenuConstants.AnimationTick;
            this.animationTimer.Elapsed += this.Animate;

            // The TAs requested a method to short circuit the UI and load straight into a test using CMD-line. This check allows that from the Main.
            if (puzzle != null && algorithm != null)
            {
                this.selectedPuzzle = puzzle;
                this.selectedAlgorithm = algorithm;
                this.currentMenu = Menus.SelectionConfirmation;
                this.currentOptions.Add("Short Circuit.");
                this.SelectMenuOption();
                return;
            }

            // If not shortcircuited, show the puzzle selection view.
            this.DisplayCurrentView();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is busy.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is busy; otherwise, <c>false</c>.
        /// </value>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.isBusy = value;

                if (value)
                {
                    this.animationTimer.Start();
                }
                else
                {
                    this.animationTimer.Stop();
                }
            }
        }

        /// <summary>
        /// Processes the user input.
        /// </summary>
        /// <param name="action">The action.</param>
        public void ProcessUserInput(MenuActions action)
        {
            // Some Menus need to lock some actions. This is only pertinent in the menu system.
            if (this.bannedActionsList.Contains(action))
            {
                return;
            }

            this.bannedActionsList.Clear();

            switch (action)
            {
                case MenuActions.Up:
                    if (this.freePlay)
                    {
                        this.freePlayState.MovePlayer(AlgorithmConstants.Move.U);
                    }
                    else
                    {
                        this.currentOption = this.currentOption == 0 ? 0 : this.currentOption - 1;
                    }

                    this.DisplayCurrentView();
                    break;
                case MenuActions.Down:
                    if (this.freePlay)
                    {
                        this.freePlayState.MovePlayer(AlgorithmConstants.Move.D);
                    }
                    else
                    {
                        this.currentOption = this.currentOption == this.totalOptions - 1 ? this.totalOptions - 1 : this.currentOption + 1;
                    }

                    this.DisplayCurrentView();
                    break;
                case MenuActions.Left:
                    if (this.freePlay)
                    {
                        this.freePlayState.MovePlayer(AlgorithmConstants.Move.L);
                        this.DisplayCurrentView();
                    }

                    break;
                case MenuActions.Right:
                    if (this.freePlay)
                    {
                        this.freePlayState.MovePlayer(AlgorithmConstants.Move.R);
                        this.DisplayCurrentView();
                    }

                    break;
                case MenuActions.Cancel:
                    if (this.freePlay)
                    {
                        this.freePlayState.ResetGame();
                        this.DisplayCurrentView();
                    }

                    break;
                case MenuActions.Back:
                    if (this.freePlay)
                    {
                        this.freePlay = false;
                        this.freePlayState = null;
                        this.menuHistory = new List<Menus>() { Menus.PuzzleSelection };
                        this.currentMenu = Menus.PuzzleSelection;
                        this.DisplayCurrentView();
                    }

                    break;
                case MenuActions.Enter:
                    this.SelectMenuOption();
                    break;
                case MenuActions.Optimization:
                    OptimizationService.Instance.Optimize = !OptimizationService.Instance.Optimize;
                    this.DisplayCurrentView();
                    Console.WriteLine("Optimization has been " + (OptimizationService.Instance.Optimize ? "Enabled." : "Disabled."));
                    break;
            }
        }

        /// <summary>
        /// Displays the program header.
        /// </summary>
        private static void DisplayProgramHeader()
        {
            Console.WriteLine(Constants.MenuConstants.FullRunner);
            Console.WriteLine(Constants.MenuConstants.ProgramTitle);
            Console.WriteLine(Constants.MenuConstants.Author);
            Console.WriteLine(Constants.MenuConstants.FullRunner);
        }

        /// <summary>
        /// Selects the menu option.
        /// </summary>
        private void SelectMenuOption()
        {
            if (!this.currentOptions.Any() || this.currentOptions.Count < this.currentOption)
            {
                return;
            }

            // Get the currently selected menu option
            string selected = this.currentOptions[this.currentOption];
            this.currentOption = 0;
            this.menuDisplayIndex = 0;

            // User Selected Exit; Kill yourself
            if (selected == MenuActions.Exit.ToString())
            {
                Console.Clear();
                Environment.Exit(0);
            }

            // User Selected Back; Go Back
            if (selected == MenuActions.Back.ToString())
            {
                if (this.menuHistory.Any())
                {
                    this.currentMenu = this.menuHistory.Last();
                    this.menuHistory = this.menuHistory.Take(this.menuHistory.Count - 1).ToList();
                }
                else
                {
                    this.currentMenu = Menus.PuzzleSelection;
                }

                this.DisplayCurrentView();
                return;
            }

            // React to the selection based on the current menu
            switch (this.currentMenu)
            {
                case Constants.MenuConstants.Menus.PuzzleSelection:
                    this.menuHistory.Add(Menus.PuzzleSelection);

                    if (selected == Menus.Help.ToString())
                    {
                        this.currentMenu = Menus.Help;
                        this.DisplayCurrentView(new string[2] { "Here, you can select a puzzle to play.",
                                                                "Use Up/Down Arrows and Enter to select."});
                        return;
                    }

                    this.selectedPuzzle = selected;
                    this.currentMenu = Menus.AlgorithmSelection;
                    break;
                case Constants.MenuConstants.Menus.AlgorithmSelection:
                    this.menuHistory.Add(Menus.AlgorithmSelection);

                    if (selected == Menus.Help.ToString())
                    {
                        this.currentMenu = Menus.Help;
                        this.DisplayCurrentView(new string[2] { "Here, you can select an algorithm to run.",
                                                                "Use Up/Down Arrows and Enter to select."});
                        return;
                    }

                    if (selected == AlgorithmConstants.FREE)
                    {
                        this.freePlay = true;
                        this.currentMenu = Menus.FreePlay;
                        break;
                    }

                    this.selectedAlgorithm = selected;
                    this.currentMenu = Menus.SelectionConfirmation;
                    break;
                case Constants.MenuConstants.Menus.SelectionConfirmation:
                    this.menuHistory.Clear();
                    this.currentMenu = Menus.Loading;
                    this.DisplayCurrentView();
                    this.ExecuteAlgorithm();
                    return;
                case Constants.MenuConstants.Menus.Loading:
                    // The only input we could get from this screen is the cancel. 
                    // Point at the PuzzleSel menu, incr the current process, and 
                    // mark is busy false so the user can start over.
                    this.currentMenu = Menus.PuzzleSelection;
                    this.processCounter++;
                    this.IsBusy = false;
                    break;
                case Constants.MenuConstants.Menus.ResultsReview:
                    break;
            }

            this.DisplayCurrentView();
        }

        /// <summary>
        /// Displays the current view.
        /// </summary>
        private void DisplayCurrentView(string[] displayText = null)
        {
            // Locking may not be the best way to do this, but... Only one thread at a time should be changing my display and there is only one possible
            // background thread so... If there is a deadlock, it is a UI design issue where the user could get click-happy, not a threading issue.
            lock (this.threadLocker)
            {
                Console.Clear();
                DisplayProgramHeader();

                switch (this.currentMenu)
                {
                    case Constants.MenuConstants.Menus.PuzzleSelection:
                        this.ShowPuzzleSelection();
                        break;
                    case Constants.MenuConstants.Menus.AlgorithmSelection:
                        this.ShowAlgorithmSelection();
                        break;
                    case Constants.MenuConstants.Menus.SelectionConfirmation:
                        this.ShowConfirmationScreen();
                        break;
                    case Constants.MenuConstants.Menus.ResultsReview:
                        this.ShowResultsScreen(displayText);
                        break;
                    case Constants.MenuConstants.Menus.Help:
                        this.ShowHelpScreen(displayText);
                        break;
                    case Constants.MenuConstants.Menus.Loading:
                        this.ShowLoadingScreen();
                        break;
                    case Constants.MenuConstants.Menus.FreePlay:
                        this.ShowFreePlayScreen();
                        break;
                }
            }
        }

        /// <summary>
        /// Shows the free play screen.
        /// </summary>
        private void ShowFreePlayScreen()
        {
            if (this.freePlayState == null)
            {
                int width = 0;
                int height = 0;
                int x = 0;
                int y = 0;
                List<int[]> crates = new List<int[]>();
                List<int[]> targets = new List<int[]>();

                char[,] gameMap = this.LoadMap(ref x, ref y, ref width, ref height, ref crates, ref targets);
                this.freePlayState = new SokobanFreePlayer(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
            }

            this.freePlayState.RenderMapState();
        }

        /// <summary>
        /// Shows the loading screen.
        /// </summary>
        private void ShowLoadingScreen()
        {
            this.bannedActionsList.AddRange(new List<MenuActions> { MenuActions.Up, MenuActions.Down, MenuActions.Left, MenuActions.Right });

            Console.WriteLine(Constants.MenuConstants.Results);
            Console.WriteLine(Constants.MenuConstants.Underline);
            Console.WriteLine("\r\n");

            Console.WriteLine(Constants.MenuConstants.Executing + this.animationProgress);
            Console.WriteLine("\r\n");
            this.RenderMenuOptions(new List<string>(), new string[] { MenuActions.Cancel.ToString() });
        }

        /// <summary>
        /// Shows the puzzle selection.
        /// </summary>
        private void ShowPuzzleSelection()
        {
            if (!this.puzzles.Any())
            {
                string[] puzzlesAvailable = System.IO.Directory.GetFiles(this.puzzlesDir);

                foreach (string puzzle in puzzlesAvailable)
                {
                    this.puzzles.Add(puzzle.Substring(this.puzzlesDir.Length + 1));
                }
            }

            Console.WriteLine(Constants.MenuConstants.Puzzle);
            Console.WriteLine(Constants.MenuConstants.Underline);
            this.RenderMenuOptions(this.puzzles, new string[2] { Menus.Help.ToString(), MenuActions.Exit.ToString() });
        }

        /// <summary>
        /// Shows the algorithm selection.
        /// </summary>
        private void ShowAlgorithmSelection()
        {
            Console.WriteLine(Constants.MenuConstants.Algorithm);
            Console.WriteLine(Constants.MenuConstants.Underline);
            this.RenderMenuOptions(this.algorithms, new string[3] { MenuActions.Back.ToString(), Menus.Help.ToString(), MenuActions.Exit.ToString() });
        }

        /// <summary>
        /// Shows the confirmation screen.
        /// </summary>
        private void ShowConfirmationScreen()
        {
            Console.WriteLine(Constants.MenuConstants.Confirmation);
            Console.WriteLine(Constants.MenuConstants.Underline);
            Console.WriteLine(Constants.MenuConstants.MenuIndent + "Puzzle: " + this.selectedPuzzle);
            Console.WriteLine(Constants.MenuConstants.MenuIndent + "Algorithm: " + this.selectedAlgorithm);
            Console.WriteLine("\r\n\r\n" + Constants.MenuConstants.MenuIndent + "OK ?");
            Console.WriteLine(Constants.MenuConstants.Underline);
            this.RenderMenuOptions(new List<string>(), new string[2] { "Yep, Go!", MenuActions.Back.ToString() });
        }

        /// <summary>
        /// Shows the results screen.
        /// </summary>
        /// <param name="results">The results.</param>
        private void ShowResultsScreen(string[] results, bool writeToFile = true)
        {
            try
            {
                this.bannedActionsList.AddRange(new List<MenuActions> { MenuActions.Up, MenuActions.Down, MenuActions.Left, MenuActions.Right });

                Console.WriteLine(Constants.MenuConstants.Results);
                Console.WriteLine(Constants.MenuConstants.Underline);

                if (results == null || !results.Any()) results = new string[1] { "Failure" };

                if (!System.IO.Directory.Exists(this.solutionsDir))
                {
                    System.IO.Directory.CreateDirectory(this.solutionsDir);
                }

                System.IO.StreamWriter sw = null;

                if (writeToFile)
                {
                    // TAs have now started requesting a solution file that saves out
                    string solFile = this.solutionsDir + "\\" + this.selectedPuzzle + "_" + this.selectedAlgorithm.Replace("*", "Star") + "_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss", CultureInfo.InvariantCulture);
                    sw = System.IO.File.CreateText(solFile);
                }
                
                foreach (string item in results)
                {
                    Console.WriteLine(Constants.MenuConstants.MenuIndent + item);

                    if (writeToFile)
                    {
                        sw.WriteLine(item);
                    }
                }

                sw.Close();
                sw.Dispose();

                Console.WriteLine("\r\n");
                this.menuHistory.Add(Menus.PuzzleSelection);
                this.RenderMenuOptions(new List<string>(), new string[] { MenuActions.Back.ToString() });
            }
            catch
            {
                if (writeToFile)
                {
                    // If we blow up writing out to the File, redo this but don't write to file.
                    Console.Clear();
                    this.ShowResultsScreen(results, false);
                }
                else
                {
                    // Otherwise, just die; we are in an endless loop otherwise.
                    string[] errorText = new string[] { "An error occured writing out results." };
                    this.currentMenu = Menus.Help;
                    this.DisplayCurrentView(errorText);
                }
            }
        }

        /// <summary>
        /// Shows the help screen.
        /// </summary>
        /// <param name="HelpText">The help text.</param>
        private void ShowHelpScreen(string[] HelpText)
        {
            this.bannedActionsList.AddRange(new List<MenuActions> { MenuActions.Up, MenuActions.Down, MenuActions.Left, MenuActions.Right });

            Console.WriteLine(Constants.MenuConstants.Help);
            Console.WriteLine(Constants.MenuConstants.Underline);

            foreach (string item in HelpText)
            {
                Console.WriteLine(Constants.MenuConstants.MenuIndent + item);
            }

            Console.WriteLine("\r\n");
            this.RenderMenuOptions(new List<string>(), new string[1] { MenuActions.Back.ToString() });
        }

        /// <summary>
        /// Executes the algorithm.
        /// </summary>
        private void ExecuteAlgorithm()
        {
            this.IsBusy = true;

            int width = 0;
            int height = 0;
            int x = 0;
            int y = 0;
            List<int[]> crates = new List<int[]>();
            List<int[]> targets = new List<int[]>();


            char[,] gameMap = this.LoadMap(ref x, ref y, ref width, ref height, ref crates, ref targets);

            if (gameMap == null) return;

            // Define the Sokoban Crawler to use based on user input new it up.
            ISokobanCrawler crawler;
            switch (this.selectedAlgorithm)
            {
                case AlgorithmConstants.BFTS:
                    crawler = new BFTSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.DFTS:
                    crawler = new DFTSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.IDDFTS:
                    crawler = new IDDFTSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.GreBeFTS:
                    crawler = new GreBeFTSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.GreBeFGS:
                    crawler = new GreBeFGSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.BiDirGreBeFGS:
                    crawler = new BiDirectGreBeFGSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.AStarTS:
                    crawler = new AStarTSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                case AlgorithmConstants.AStarGS:
                    crawler = new AStarGSCrawler(gameMap, new int[2] { width, height }, new int[2] { x, y }, crates, targets);
                    break;
                default:
                    return;
            }

            OptimizationService.Instance.SetOptimizer(new ClusteringOptimizer());
            OptimizationService.Instance.Optimizer.PreProcess(gameMap, crates, targets);

            // Call the primary "Crunch" method on a background thread
            this.algoHandler = crawler.Crunch;
            IAsyncResult result = this.algoHandler.BeginInvoke(this.OnAlgorithmCompleted, this.processCounter.ToString());
        }

        private void PreProcessMap(char[,] gameMap)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the map.
        /// </summary>
        /// <returns>The loaded map</returns>
        private char[,] LoadMap(ref int x, ref int y, ref int width, ref int height, ref List<int[]> crates, ref List<int[]> targets)
        {
            try
            {
                // Break up the parameters from the puzzle file
                if (!System.IO.File.Exists(this.puzzlesDir + "\\" + this.selectedPuzzle))
                {
                    this.IsBusy = false;
                    this.currentMenu = Menus.Help;
                    this.DisplayCurrentView(new string[2] { "File not found. Ensure you included the file",
                                                        "extension or move the file to Puzzles folder."});
                    return null;
                }

                string[] level = System.IO.File.ReadAllLines(this.puzzlesDir + "\\" + this.selectedPuzzle);
                string[] levelDims = level[0].Split(' ');
                string[] startCoords = level[1].Split(' ');

                int.TryParse(levelDims[0], out width);
                int.TryParse(levelDims[1], out height);

                int.TryParse(startCoords[0], out x);
                int.TryParse(startCoords[1], out y);

                // Build an Multi-Dim Char array representation of the Map for use in algo-crunching
                char[,] gameMap = new char[width, height];
                for (int row = 0; row < height; row++)
                {
                    char[] mapRow = level[row + 2].ToCharArray();

                    for (int column = 0; column < width; column++)
                    {
                        gameMap[column, row] = mapRow[column];
                        switch (gameMap[column, row])
                        {
                            case AlgorithmConstants.target:
                                targets.Add(new int[] { column, row });
                                break;
                            case AlgorithmConstants.crate:
                                crates.Add(new int[] { column, row });
                                gameMap[column, row] = AlgorithmConstants.tile;
                                break;
                            case AlgorithmConstants.filledGoal:
                                crates.Add(new int[] { column, row });
                                targets.Add(new int[] { column, row });
                                break;
                        }
                    }
                }

                return gameMap;
            }
            catch
            {
                this.IsBusy = false;
                this.currentMenu = Menus.Help;
                this.DisplayCurrentView(new string[] { "An error occured opening the puzzle. Please",
                                                       "verify the puzzle is formatted correctly."});
                return null;
            }
        }

        /// <summary>
        /// Called when [algorithm completed].
        /// </summary>
        /// <param name="result">The result.</param>
        private void OnAlgorithmCompleted(IAsyncResult result)
        {
            // If we cancelled a previous task and this is it returning, just ignore it; otherwise show results.
            if (result.AsyncState.ToString() == this.processCounter.ToString())
            {
                this.IsBusy = false;

                // Invoke the returned AsyncResult to receive the results from our crunch method
                string[] results = this.algoHandler.EndInvoke(result);

                this.currentMenu = Menus.ResultsReview;
                this.DisplayCurrentView(results);
            }
        }

        /// <summary>
        /// Renders the menu options.
        /// </summary>
        /// <param name="menuOptions">The menu options.</param>
        /// <param name="additionalOptions">The additional options.</param>
        private void RenderMenuOptions(List<string> menuOptions, string[] additionalOptions = null)
        {
            // Clear the UI CrateLocations variables to ensure we re-Up the list each time.
            this.currentOptions = new List<string>();
            this.totalOptions = 0;

            // Create the menu options for the current menu
            int incr = 0;
            this.currentOptions.AddRange(menuOptions);

            if (additionalOptions != null)
            {
                foreach (string addOpt in additionalOptions)
                {
                    this.currentOptions.Add(addOpt);
                }
            }

            if (this.currentOption > this.menuDisplayIndex + this.menuDisplayMax)
            {
                this.menuDisplayIndex++; 
            }
            else if (this.currentOption < this.menuDisplayIndex)
            {
                this.menuDisplayIndex--;
            }

            foreach (string option in this.currentOptions)
            {
                string optionBuilder = string.Empty;

                if (incr >= this.menuDisplayIndex && incr <= (this.menuDisplayIndex + this.menuDisplayMax))
                {
                    optionBuilder += Constants.MenuConstants.MenuIndent;

                    if (incr == this.currentOption)
                    {
                        optionBuilder += Constants.MenuConstants.Selector;
                    }
                    else
                    {
                        optionBuilder += Constants.MenuConstants.Bump;
                    }

                    optionBuilder += Constants.MenuConstants.Bump;
                    optionBuilder += option + "\r\n";

                    Console.WriteLine(optionBuilder);
                }

                incr++;
                this.totalOptions++;
            }
        }

        /// <summary>
        /// Animates the specified sender.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
        private void Animate(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (this.IsBusy)
            {
                this.animationProgress = this.animationProgress == "..." ? "" : this.animationProgress += ".";
                this.DisplayCurrentView();
            }
        }
    }
}
