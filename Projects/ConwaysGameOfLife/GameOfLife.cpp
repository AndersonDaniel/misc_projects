// GameOfLife.cpp

#include <iostream>
#include <time.h>
#include <windows.h>
#include <conio.h>

using namespace std;

// Enum declaration
enum CellState
{
	DEAD,
	ALIVE
};
enum ArrowKey
{
	UP		= 72,
	DOWN	= 80,
	LEFT	= 75,
	RIGHT	= 77
};

// Typedef declaration
typedef CellState** World;

// Global const declaration
const	int		WORLD_WIDTH			= 30;
const	int		WORLD_HEIGHT		= 15;
const	int		MAX_PROMPT_LINES	= 7;
const	int		ESCAPE_CODE			= 27;
const	int		RETURN_CODE			= 13;
const	int		SPACE_CODE			= 32;
const	int		ARROW_CODE			= 224;
const	char	LIVE_CHAR			= '@';
const	char	DEAD_CHAR			= ' ';
const	int		SLEEP_TIME			= 35;

// Global variable declaration
int	g_nConsoleWidth;

// Function prototypes
COORD	MoveCursor(int nDestX, int nDestY);
void	Prompt(char szPromptStr[]);
World	EditWorld(World wToEdit);
void	LoadConsoleWidth();
void	PrintWorld(World wToPrint);
int		CountNeighbours(World wWorld, int nX, int nY);
World	MoveGeneration(World wCurrentWorld);
World	CreateNewWorld();
World	Run(World wStartingWorld);
void	ShowCursor();

//---------------------------------------------------------------------------------------
//										Game of Life
//										------------
//
// General	: This is Conway's Game of Life.
//
// Input	: The user's orders.
//
// Process	: Computing the next generations.
//
// Output	: The world, through its various generations.
//
//---------------------------------------------------------------------------------------
// Programmer	: Daniel Anderson
// Student no.	: 205
// Date			: 21/12/2010
//---------------------------------------------------------------------------------------
void main()
{
	// Variable definitions
	World	wGameWorld;
	int		nInputCode;

	// Code Section

	// Load the console width
	LoadConsoleWidth();

	// Get the world from the user
	wGameWorld = EditWorld(NULL);

	// Run the world
	wGameWorld = Run(wGameWorld);

	// Ask the user wether he wants to continue, or not
	Prompt("What would you like to do?");

	// Prompt the options
	Prompt("Escape - quit; space - edit; enter - run the world.");

	// Get the user input
	nInputCode = getch();
	
	// While the user doesn't want to quit
	while (nInputCode != ESCAPE_CODE)
	{
		// If the user wants to edit the world
		if (nInputCode == SPACE_CODE)
		{
			// Edit the world
			wGameWorld = EditWorld(wGameWorld);
		}

		// Run the world
		wGameWorld = Run(wGameWorld);

		// Ask the user wether he wants to continue, or not
		Prompt("What would you like to do?");

		// Prompt the options
		Prompt("Escape - quit; space - edit; enter - run the world.");

		// Get the user input
		nInputCode = getch();
	}
}

//---------------------------------------------------------------------------------------
//								ShowCursor
//								----------
//
// General		: This function shows the console cursor in the desired visibility.
//
// Parameters	: bShow	- boolean. Indication wether to show the cursor or not.
//
// Return Value	: None.
//
//---------------------------------------------------------------------------------------
void ShowCursor(bool bShow)
{
	// Variable definitions
	CONSOLE_CURSOR_INFO cci;
	static HANDLE hStdout = GetStdHandle(STD_OUTPUT_HANDLE);

	// Code Section
	
	// Set the cursor properties
	cci.bVisible = bShow;
	SetConsoleCursorInfo(hStdout, &cci);
}

//---------------------------------------------------------------------------------------
//								MoveCursor
//								----------
//
// General		: This function moves the console cursor to the desired location.
//
// Parameters	: int nDestX	- the x coordinate of the destination (IN)
//				  int nDestY	- the y coordinate of the destination (IN)
//
// Return Value	: COORD	- the previous cursor position
//
//---------------------------------------------------------------------------------------
COORD MoveCursor(int nDestX, int nDestY)
{
	// Variable definitions
	static HANDLE hStdout = GetStdHandle(STD_OUTPUT_HANDLE);
	CONSOLE_SCREEN_BUFFER_INFO csbiInfo;
	COORD cPrevPosition;

	// Code Section
	
	// Hide the cursor
	ShowCursor(false);

	// Get the console buffer info
	GetConsoleScreenBufferInfo(hStdout, &csbiInfo);
	SetConsoleTextAttribute(hStdout, 10);
	// Save the current position
	cPrevPosition.X = csbiInfo.dwCursorPosition.X;
	cPrevPosition.Y = csbiInfo.dwCursorPosition.Y;

	// Move the cursor
	csbiInfo.dwCursorPosition.X = nDestX;
	csbiInfo.dwCursorPosition.Y = nDestY;

	// Set the console cursor to the position desired
	SetConsoleCursorPosition(hStdout, csbiInfo.dwCursorPosition);

	// Show the cursor
	ShowCursor(true);

	// Return the previous position
	return (cPrevPosition);
}

//---------------------------------------------------------------------------------------
//								Prompt
//								------
//
// General		: This function prompts a message to the user.
//
// Parameters	: nLine			- int, the line to write to (IO).
//				  szPromptStr	- string, the string to prompt (IN).
//
// Return Value	: None.
//
//---------------------------------------------------------------------------------------
void Prompt(char szPromptStr[])
{
	// Variable definitions
	static	int nPromptLine = WORLD_HEIGHT;
	int		nLineIndex;
	int		nCharIndex;
	COORD	cPrevious;

	// Code Section

	// Move the cursor to the desired line
	cPrevious = MoveCursor(0, nPromptLine);

	// If the line is beyond the maximum prompt lines
	if (nPromptLine > WORLD_HEIGHT + MAX_PROMPT_LINES - 1)
	{
		// Move it back to the first line
		nPromptLine = WORLD_HEIGHT;
		
		// Move the cursor to the first prompt line
		MoveCursor(0, WORLD_HEIGHT);

		// Erase all the lines
		for (nLineIndex = WORLD_HEIGHT; 
			 nLineIndex < WORLD_HEIGHT + MAX_PROMPT_LINES;
			 nLineIndex++)
		{
			// For each character in a console line
			for (nCharIndex = 0; nCharIndex < g_nConsoleWidth; nCharIndex++)
			{
				cout << " ";
			}
		}

		// Move the cursor to the first prompt line
		MoveCursor(0, WORLD_HEIGHT);
	}

	// Write the message
	cout << szPromptStr;

	// Increment the line
	nPromptLine++;

	// Return the cursor to its previous position
	MoveCursor(cPrevious.X, cPrevious.Y);
}

//---------------------------------------------------------------------------------------
//								GetInitialWorld
//								---------------
//
// General		: This function gets the initial world from the user.
//
// Parameters	: None.
//
// Return Value	: World - the initial world of the user.
//
//---------------------------------------------------------------------------------------
World EditWorld(World wToEdit)
{
	// Variable definitions
	int		nInputCode;
	COORD	cCursorPosition;
	World	wNewWorld;
	int		nColIndex;
	int		nRowIndex;
	char	cToDraw;

	// Code Section

	// Initialize the cursor location
	cCursorPosition.X = 0;
	cCursorPosition.Y = 0;
	
	// If the world to edit is null
	if (wToEdit == NULL)
	{
		// Create a new world
		wNewWorld = CreateNewWorld();
	}
	// Else
	else
	{
		// Edit the world
		wNewWorld = wToEdit;
	}

	// Prompt a message
	Prompt("Use the arrows to navigate; spacebar to change cell; return to finish.");

	// Move the cursor to the top-left corner of the screen
	MoveCursor(0, 0);

	// Get the first code
	nInputCode = getch();

	// While the user doesn't want to finish
	while (nInputCode != RETURN_CODE)
	{
		// If the user wants to change to cell
		if (nInputCode == SPACE_CODE)
		{
			// If the cell is alive
			if (wNewWorld[cCursorPosition.Y][cCursorPosition.X] == ALIVE)
			{
				// Kill it
				wNewWorld[cCursorPosition.Y][cCursorPosition.X] = DEAD;

				// The character to draw is a dead one
				cToDraw = DEAD_CHAR;
			}
			// Else, the cell is dead
			else
			{
				// Make it live!!! HAHAHAHAHAHAHA!!!!
				wNewWorld[cCursorPosition.Y][cCursorPosition.X] = ALIVE;

				// The character to draw is a living one
				cToDraw = LIVE_CHAR;
			}

			// Draw the character
			cout << cToDraw;

			// Move the cursor back to its position
			MoveCursor(cCursorPosition.X, cCursorPosition.Y);
		}
		// Else, if the user wants to move
		else if (nInputCode == ARROW_CODE)
		{
			// Get the additional data
			nInputCode = getch();

			// Check what direction was pressed
			switch((ArrowKey)nInputCode)
			{
				// If UP
				case (UP):
				{
					// If the cursor is not at the top
					if (cCursorPosition.Y > 0)
					{
						// Move the cursor up
						cCursorPosition.Y--;
					}

					break;
				}
				// If DOWN
				case (DOWN):
				{
					// If the cursor is not at botton
					if (cCursorPosition.Y < WORLD_HEIGHT - 1)
					{
						// Move the cursor up
						cCursorPosition.Y++;
					}

					break;
				}
				// If LEFT
				case (LEFT):
				{
					// If the cursor is not at the leftmost edge
					if (cCursorPosition.X > 0)
					{
						// Move the cursor up
						cCursorPosition.X--;
					}

					break;
				}
				// If RIGHT
				case (RIGHT):
				{
					// If the cursor is not at rightmost edge
					if (cCursorPosition.X < WORLD_WIDTH - 1)
					{
						// Move the cursor up
						cCursorPosition.X++;
					}

					break;
				}
			}

			// Move the cursor to its position
			MoveCursor(cCursorPosition.X, cCursorPosition.Y);
		}
		// Else, the user input is invalid
		else
		{
			// Prompt a message
			Prompt("Invalid user input!");
		}

		// Get the next input
		nInputCode = getch();
	}

	// Return the world
	return (wNewWorld);
}

//---------------------------------------------------------------------------------------
//								LoadConsoleWidth
//								----------------
//
// General		: This function gets the console width into a global variable.
//
// Parameters	: None.
//
// Return Value	: None.
//
//---------------------------------------------------------------------------------------
void LoadConsoleWidth()
{
	// Variable definitions
	HANDLE hStdout;
	CONSOLE_SCREEN_BUFFER_INFO csbiInfo;

	// Code Section

	// Create an output handle
	hStdout = GetStdHandle(STD_OUTPUT_HANDLE);

	// Get the console buffer info
	GetConsoleScreenBufferInfo(hStdout, &csbiInfo);

	// Set the global variable to the console width
	g_nConsoleWidth = csbiInfo.dwSize.X;
}

//---------------------------------------------------------------------------------------
//								PrintWorld
//								----------
//
// General		: This function prints a world.
//
// Parameters	: World wToPrint - the world to print.
//
// Return Value	: None.
//
//---------------------------------------------------------------------------------------
void PrintWorld(World wToPrint)
{
	// Variable definitions
	int nColIndex;
	int nRowIndex;

	// Code Section

	// Move the cursor to the top-left corner
	MoveCursor(0, 0);

	// Move over each cell
	for (nRowIndex = 0; nRowIndex < WORLD_HEIGHT; nRowIndex++)
	{
		for (nColIndex = 0; nColIndex < WORLD_WIDTH; nColIndex++)
		{
			// If the cell is alive
			if (wToPrint[nRowIndex][nColIndex] == ALIVE)
			{
				// Print the living cell character
				cout << LIVE_CHAR;
			}
			// Else, the cell is dead
			else
			{
				// Print the dead cell character
				cout << DEAD_CHAR;
			}
		}

		// Move down one line
		cout << endl;
	}
}

//---------------------------------------------------------------------------------------
//								CountNeighbours
//								---------------
//
// General		: This function counts the neighbours of a cell.
//
// Parameters	: World wWorld	- the world in which the cell lives (IN).
//				  nX			- the X value of the cell (IN).
//				  nY			- the Y value of the cell (IN).
//
// Return Value	: int	- the number of neighbours of the cell.
//
//---------------------------------------------------------------------------------------
int CountNeighbours(World wWorld, int nX, int nY)
{
	// Variable definitions
	int nColIndex;
	int nRowIndex;
	int nNeighbours;
	int nMinX;
	int nMinY;
	int nMaxX;
	int nMaxY;

	// Code Section
	
	// Initialize the minimum and maximum values
	nMinX = nX - 1;
	nMaxX = nX + 1;
	nMinY = nY - 1;
	nMaxY = nY + 1;

	// If the limits are outside the world
	if (nMinX < 0)
	{
		nMinX = 0;
	}
	if (nMaxX >= WORLD_WIDTH)
	{
		nMaxX = WORLD_WIDTH - 1;
	}
	if (nMinY < 0)
	{
		nMinY = 0;
	}
	if (nMaxY >= WORLD_HEIGHT)
	{
		nMaxY = WORLD_HEIGHT - 1;
	}

	// Right now there are no neighbours
	nNeighbours = 0;

	// Move over all of the neighbour cells
	for (nRowIndex = nMinY;
		 nRowIndex <= nMaxY;
		 nRowIndex++)
	{
		for (nColIndex = nMinX;
			 nColIndex <= nMaxX;
			 nColIndex++)
		{
			// If the current cell is not the original cell itself
			if ((nColIndex != nX) || (nRowIndex != nY))
			{
				// If the cell is alive, add one to the count of neighbours
				nNeighbours += (wWorld[nRowIndex][nColIndex] == ALIVE);
			}
		}
	}

	// Return the number of neighbours
	return (nNeighbours);
}

//---------------------------------------------------------------------------------------
//								MoveGeneration
//								--------------
//
// General		: This function generates the next generation of a world.
//
// Parameters	: World wWorld	- the world in the current generation.
//
// Return Value	: World	- the next generation.
//
//---------------------------------------------------------------------------------------
World	MoveGeneration(World wCurrentWorld)
{
	// Variable definitions
	int			nColIndex;
	int			nRowIndex;
	int			nNeighbours;
	World		wNextGen;

	// Code Section

	// Hide the cursor
	ShowCursor(false);

	// Create the new world
	wNextGen = CreateNewWorld();

	// Move over the world
	for (nRowIndex = 0;
		 nRowIndex < WORLD_HEIGHT;
		 nRowIndex++)
	{
		for (nColIndex = 0;
			 nColIndex < WORLD_WIDTH;
			 nColIndex++)
		{
			// Copy the previous world's cell from that place
			wNextGen[nRowIndex][nColIndex] = wCurrentWorld[nRowIndex][nColIndex];

			// If the cell is dead, and has three neighbours
			if ((wCurrentWorld[nRowIndex][nColIndex] == DEAD) &&
				(CountNeighbours(wCurrentWorld, nColIndex, nRowIndex) == 3))
			{
				// Bring the cell to life!!!!!!!
				wNextGen[nRowIndex][nColIndex] = ALIVE;

				// Move the cursor to that place
				MoveCursor(nColIndex, nRowIndex);

				// Print the live character
				cout << LIVE_CHAR;
			}
			// Else, if the cell is alive and has more than 3 neighbours
			// or less than 2 neighbours
			else if ((wCurrentWorld[nRowIndex][nColIndex] == ALIVE) &&
					 ((CountNeighbours(wCurrentWorld, nColIndex, nRowIndex) < 2)) ||
					  (CountNeighbours(wCurrentWorld, nColIndex, nRowIndex) > 3))
			{
				// Kill it :(
				wNextGen[nRowIndex][nColIndex] = DEAD;

				// Move the cursor to that place
				MoveCursor(nColIndex, nRowIndex);

				// Print the dead character
				cout << DEAD_CHAR;
			}
		}
	}

	// Show the cursor
	ShowCursor(true);

	// Return the next generation
	return (wNextGen);
}

//---------------------------------------------------------------------------------------
//								MoveGeneration
//								--------------
//
// General		: This function creates a new world.
//
// Parameters	: None.
//
// Return Value	: World - the new world.
//
//---------------------------------------------------------------------------------------
World CreateNewWorld()
{
	// Variable definitions
	World wNewWorld;
	int nColIndex;
	int nRowIndex;

	// Code Section

	// Create the new world
	wNewWorld = new CellState*[WORLD_HEIGHT];

	// Initialize the world
	for (nRowIndex = 0; nRowIndex < WORLD_HEIGHT; nRowIndex++)
	{
		// Initialize the current row
		wNewWorld[nRowIndex] = new CellState[WORLD_WIDTH];

		// Make all the cells dead
		for (nColIndex = 0; nColIndex < WORLD_WIDTH; nColIndex++)
		{
			// Signify the cell as dead
			wNewWorld[nRowIndex][nColIndex] = DEAD;
		}
	}

	// Return the new world
	return (wNewWorld);
}

//---------------------------------------------------------------------------------------
//								Run
//								---
//
// General		: This function runs the world.
//
// Parameters	: wStartingWorld	- the world at the beginning.
//
// Return Value	: World - the world at the end of the run.
//
//---------------------------------------------------------------------------------------
World Run(World wWorld)
{
	// Code Section
	
	// While there is no key available, or the key is not escape
	while ((!_kbhit() /*conio.h*/) || (getch() != ESCAPE_CODE))
	{
		// Advance the world to the next generation
		wWorld = MoveGeneration(wWorld);

		// Sleep for the desired duration
		Sleep(SLEEP_TIME); // windows.h
	}

	// Return the world
	return (wWorld);
}