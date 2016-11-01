#include "ConsoleUtils.h"

using namespace std;

//---------------------------------------------------------------------------------------
// ...
// Gets color code for certain text and background colors
//---------------------------------------------------------------------------------------
unsigned short GetColor(int textcolor, int backcolor)
{
	return ((unsigned)backcolor << 4) | ((unsigned)textcolor);
}

//---------------------------------------------------------------------------------------
// ...
// Sets console color for certain text and background colors
//---------------------------------------------------------------------------------------
void SetColor(int textcolor, int backcolor)
{
	textcolor %= 16;
	backcolor %= 16;
	unsigned short wAttributes = GetColor(textcolor, backcolor);
	SetColor(wAttributes);
}

//---------------------------------------------------------------------------------------
// ...
// Sets console color to a specific color code
//---------------------------------------------------------------------------------------
void SetColor(unsigned short attrs)
{
	SetConsoleTextAttribute(GetStdHandle(STD_OUTPUT_HANDLE), attrs);
}

//---------------------------------------------------------------------------------------
// ...
// Makes console larger
//---------------------------------------------------------------------------------------
void ResizeConsole(int nWidth, int nHeight)
{
	system("mode CON: COLS=480");
	HWND console = GetConsoleWindow();
	RECT r;
	GetWindowRect(console, &r);
	MoveWindow(console, 30, 20, 30 + nWidth, 20 + nHeight, TRUE);
}

//---------------------------------------------------------------------------------------
// ...
// Moves the writing cursor to a specific destination
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
// ...
// Set the console font size to a specific size
//---------------------------------------------------------------------------------------
void SetFontSize(int nSize)
{
	CONSOLE_FONT_INFOEX cfie = { 0 };
	GetCurrentConsoleFontEx(GetStdHandle(STD_OUTPUT_HANDLE), 0, &cfie);
	cfie.cbSize = sizeof(cfie);
	cfie.dwFontSize.Y = nSize;
	cfie.FontWeight = FW_NORMAL;
	wcscpy_s(cfie.FaceName, L"Lucida Console");
	SetCurrentConsoleFontEx(GetStdHandle(STD_OUTPUT_HANDLE), 0, &cfie);
}