***********************************************************************
**                     Sokoban Solving Algorithms                    **
**                           by Ben Smith                            **
**                                                                   **
**           Missouri University of Science and Technology           **
**                          Spring 2018                              **
**                       February 28, 2018                           **
***********************************************************************

SUMMARY:
	This program is designed to apply search algorithms to specific 
	programming problems and return timely and accurate solutions.
	The application also allows freeplay of Levels by a player.
	
DESIGN:
	The design of this application is in MVC pattern. Models contain
	logic and values, Controllers invoke models and return data to 
	views. Please note that, as a Console Application, the views are
	built and written to the Console from the Controller and aren't
	in their own seperate files.
	
	Each problem algorithm is defined in its own individual class. 
	The UI is designed to allow the user to select a puzzle, and an
	algorithm to run against it. The primary starting point for al-
	gorithm logic is the "Crunch" method of each class or base. 
	
	
USAGE: 
	The application may be used in several ways. NOTE: Short-circuiting
	results in slightly longer run times as the UI thread and bg-thread
	compete for a small period. Subsequent runs are much faster. 

	1) Double click the EXE and simply walk through the UI. The UI
		accepts user input in the form of the up/down arrows and the
		enter key. To run Puzzle1 and BFTS is as simple as hitting 
		enter three times. The applicaton allows for "Backing" so 
		re-running an algorithm is just another keystroke or two.

	2) Command Line arguments (Short-Circuiting the UI)
		SokobanSolver.exe [puzzleFile] [algorithm]

		ex. SokobanSolver.exe puzzle1.txt bfts
			- runs the "puzzle1.txt" level using the Breadth-First
			  Tree Search.
		
		Arg1 is the puzzle file name (including extension). Arg2
		is the acronym for the wanted algorithm. Possible entries
		are:
		
		"BFTS"		= Breadth-First Tree Search
		"DFTS"		= Depth-First Tree Search
		"IDDFTS"	= Iterative Deepening - Depth First Tree Search
		"GreBeFTS"	= Greedy Best-First Tree Search
		"GreBeFGS"	= Greedy Best-First Graph Search
		"BiDirGreBeFTS"	= Bi-Directional Greedy Best-First Tree Search
		"A*TS"			= A* Tree Search
		"A*GS"			= A* Graph Search

		Note: file name is case sensitive, but algo isn't.

	3) Bat-Files for each run case (Short-Circuiting the UI) 
	The author has created Bat-Files for the individual run cases that
	removes the need to type in arguments. Just click the Bat.

	  ex. Click "Run-Puzzle1-BFTS.bat" and it will run immediately

CUSTOMIZATION:
	Puzzles are loaded from the "Puzzles" folder. The provided defa-
	ult puzzles are included with the solution, but users may add 
	more to that folder and simply re-run the program for them to 
	display in the Puzzle Selection menu.

COMPILATION:
	This program was created using Visual Studio 2017 Community Ed.
	To compile, if source is owned, users should:
		1) Open the "SokobanSolver" containing folder.
		2) Double click "SokobanSolver.sln" (the VS 
		   solution file).
		3) Once VS opens the solution, build and run the application.