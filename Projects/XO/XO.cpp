// XO.cpp

#include <iostream>
#include <Windows.h>
#include <conio.h>

using namespace std;

const int WORLD_SIZE = 3;
const int CONST_VICT_VALUE = 100;
const	int		ESCAPE_CODE = 27;
const	int		RETURN_CODE = 13;
const	int		SPACE_CODE = 32;
const	int		ARROW_CODE = 224;


enum Cell
{
	empty = ' ',
	x = 'X',
	o = 'O'
};

enum ArrowKey
{
	UP = 72,
	DOWN = 80,
	LEFT = 75,
	RIGHT = 77
};

Cell OtherPlayer(Cell cPlayer);
void PrintGameState(Cell matGame[WORLD_SIZE][WORLD_SIZE]);
void InitGameState(Cell matGame[WORLD_SIZE][WORLD_SIZE]);
COORD GetLocFromUser(Cell matGame[WORLD_SIZE][WORLD_SIZE]);
COORD GetLocFromComp(Cell matGame[WORLD_SIZE][WORLD_SIZE], Cell cPlayer, int* pnValue);
void Prompt(char strText[]);
COORD MoveCursor(int nDestX, int nDestY);
Cell FindWinner(Cell matGame[WORLD_SIZE][WORLD_SIZE]);
Cell MatAtIndex(Cell matGame[WORLD_SIZE][WORLD_SIZE], int nIndex);
void CopyGameState(Cell matGameSource[WORLD_SIZE][WORLD_SIZE],
				   Cell matGameDest[WORLD_SIZE][WORLD_SIZE]);
int GetValue(Cell matGame[WORLD_SIZE][WORLD_SIZE], Cell cPlayer);
bool IsFull(Cell matGame[WORLD_SIZE][WORLD_SIZE]);

void main()
{
	Cell cCurrPlayer = x;
	Cell matGame[WORLD_SIZE][WORLD_SIZE];
	InitGameState(matGame);
	PrintGameState(matGame);

	while (FindWinner(matGame) == empty && !IsFull(matGame))
	{
		COORD cMove;

		if (cCurrPlayer == x)
		{
			Prompt("Enter move for X!");
			cMove = GetLocFromUser(matGame);
		}
		else
		{
			Prompt("Thinking...");
			int nTemp;
			cMove = GetLocFromComp(matGame, cCurrPlayer, &nTemp);
		}

		matGame[cMove.Y][cMove.X] = cCurrPlayer;
		PrintGameState(matGame);
		cCurrPlayer = OtherPlayer(cCurrPlayer);
	}
	
	if (FindWinner(matGame) == o)
	{
		Prompt("Game over! O won!");
	}
	else if (FindWinner(matGame) == x)
	{
		Prompt("Game over! X won!");
	}
	else
	{
		Prompt("Game over! Draw");
	}

	system("PAUSE");
}

void InitGameState(Cell matGame[WORLD_SIZE][WORLD_SIZE])
{
	for (int nRowIndex = 0; nRowIndex < WORLD_SIZE; nRowIndex++)
	{
		for (int nColIndex = 0; nColIndex < WORLD_SIZE; nColIndex++)
		{
			matGame[nRowIndex][nColIndex] = empty;
		}
	}
}

void PrintGameState(Cell matGame[WORLD_SIZE][WORLD_SIZE])
{
	MoveCursor(0, 0);
	for (int nRowIndex = 0; nRowIndex < WORLD_SIZE; nRowIndex++)
	{
		for (int nColIndex = 0; nColIndex < WORLD_SIZE; nColIndex++)
		{
			cout << (char)(matGame[nRowIndex][nColIndex]);
			if (nColIndex < WORLD_SIZE - 1)
			{
				cout << "|";
			}
		}

		cout << endl;
		if (nRowIndex < WORLD_SIZE - 1)
		{
			cout << "-----" << endl;
		}
	}
}

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

COORD GetLocFromUser(Cell matGame[WORLD_SIZE][WORLD_SIZE])
{
	COORD	cCursorPosition;

	cCursorPosition.X = 0;
	cCursorPosition.Y = 0;
	MoveCursor(cCursorPosition.X, cCursorPosition.Y);

	int nInputCode = _getch();

	// While the user doesn't want to quit
	while (true)
	{
		if (nInputCode == SPACE_CODE && matGame[cCursorPosition.Y][cCursorPosition.X] == empty)
		{
			return (cCursorPosition);
		}
		else if (nInputCode == ARROW_CODE)
		{
			nInputCode = _getch();

			switch ((ArrowKey)nInputCode)
			{
			case (UP) :
			{
						  // If the cursor is not at the top
						  if (cCursorPosition.Y > 0)
						  {
							  // Move the cursor up
							  cCursorPosition.Y--;
						  }

						  break;
			}
			case (DOWN) :
			{
							// If the cursor is not at botton
							if (cCursorPosition.Y < WORLD_SIZE - 1)
							{
								// Move the cursor up
								cCursorPosition.Y++;
							}

							break;
			}
			case (LEFT) :
			{
							// If the cursor is not at the leftmost edge
							if (cCursorPosition.X > 0)
							{
								// Move the cursor up
								cCursorPosition.X--;
							}

							break;
			}
			case (RIGHT) :
			{
							 // If the cursor is not at rightmost edge
							 if (cCursorPosition.X < WORLD_SIZE - 1)
							 {
								 // Move the cursor up
								 cCursorPosition.X++;
							 }

							 break;
			}
			}

			MoveCursor(cCursorPosition.X * 2, cCursorPosition.Y * 2);
		}

		nInputCode = _getch();
	}
}

void Prompt(char strText[])
{
	COORD cInitial = MoveCursor(0, 7);
	cout << strText << "                 " << endl;
}

Cell FindWinner(Cell matGame[WORLD_SIZE][WORLD_SIZE])
{
	const int CONST_VICTORY[][WORLD_SIZE] = { { 0, 1, 2 },
											  { 3, 4, 5 },
											  { 6, 7, 8 },
											  { 0, 3, 6 },
											  { 1, 4, 7 },
											  { 2, 5, 8 },
											  { 0, 4, 8 },
											  { 2, 4, 6 } };
	const int VICTORY_OPTIONS = 8;

	for (int nVicIndex = 0; nVicIndex < VICTORY_OPTIONS; nVicIndex++)
	{
		if ((MatAtIndex(matGame, CONST_VICTORY[nVicIndex][0]) == MatAtIndex(matGame, CONST_VICTORY[nVicIndex][1])) &&
			(MatAtIndex(matGame, CONST_VICTORY[nVicIndex][1]) == MatAtIndex(matGame, CONST_VICTORY[nVicIndex][2])) &&
			(MatAtIndex(matGame, CONST_VICTORY[nVicIndex][0]) != empty))
		{
			return (MatAtIndex(matGame, CONST_VICTORY[nVicIndex][0]));
		}
	}

	return (empty);
}

Cell MatAtIndex(Cell matGame[WORLD_SIZE][WORLD_SIZE], int nIndex)
{
	return (matGame[nIndex / WORLD_SIZE][nIndex % WORLD_SIZE]);
}

void CopyGameState(Cell matGameSource[WORLD_SIZE][WORLD_SIZE],
				   Cell matGameDest[WORLD_SIZE][WORLD_SIZE])
{
	for (int nRowIndex = 0; nRowIndex < WORLD_SIZE; nRowIndex++)
	{
		for (int nColIndex = 0; nColIndex < WORLD_SIZE; nColIndex++)
		{
			matGameDest[nRowIndex][nColIndex] = matGameSource[nRowIndex][nColIndex];
		}
	}
}

COORD GetLocFromComp(Cell matGame[WORLD_SIZE][WORLD_SIZE], Cell cPlayer, int* pnValue)
{
	COORD cRes;
	cRes.X = 0;
	cRes.Y = 0;
	int nMaxValue = -2 * CONST_VICT_VALUE;
	Cell matTempGame[WORLD_SIZE][WORLD_SIZE];

	for (int nRowIndex = 0; nRowIndex < WORLD_SIZE; nRowIndex++)
	{
		for (int nColIndex = 0; nColIndex < WORLD_SIZE; nColIndex++)
		{
			if (matGame[nRowIndex][nColIndex] == empty)
			{
				CopyGameState(matGame, matTempGame);
				matTempGame[nRowIndex][nColIndex] = cPlayer;
				if ((*pnValue = GetValue(matTempGame, cPlayer)) > nMaxValue)
				{
					cRes.X = nColIndex;
					cRes.Y = nRowIndex;
					nMaxValue = *pnValue;
				}
			}
		}
	}

	*pnValue = nMaxValue;

	return (cRes);
}

int GetValue(Cell matGame[WORLD_SIZE][WORLD_SIZE], Cell cPlayer)
{
	Cell cWinner = FindWinner(matGame);
	if (cWinner != empty)
	{
		return (CONST_VICT_VALUE * (cPlayer == cWinner ? 1 : -1));
	}

	if (IsFull(matGame))
	{
		return (0);
	}

	int nValue;
	COORD cMove = GetLocFromComp(matGame, OtherPlayer(cPlayer), &nValue);

	return (-nValue);
}

Cell OtherPlayer(Cell cPlayer)
{
	return (cPlayer == x ? o : x);
}

bool IsFull(Cell matGame[WORLD_SIZE][WORLD_SIZE])
{
	bool bAllFull = true;
	for (int nRowIndex = 0; nRowIndex < WORLD_SIZE; nRowIndex++)
	{
		for (int nColIndex = 0; nColIndex < WORLD_SIZE; nColIndex++)
		{
			if (matGame[nRowIndex][nColIndex] == empty)
			{
				bAllFull = false;
			}
		}
	}

	return (bAllFull);
}