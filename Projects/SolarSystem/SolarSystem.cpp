#include <stdio.h>
#include <iostream>
#include <math.h>
#include "ConsoleUtils.h"

using namespace std;

void FillCircle(int x, int y, int r, int c);
void DrawCircle(int x, int y, int r, int c);
void DrawCircle(int x, int y, int r, int c, int startX, int endX, int startY, int endY);
void MyFillCircle(int x, int y, int r, int c, int cx, int cy, int cr, int cc);

void main(int argc, char* argv[])
{
	const int WIDTH = 5000, HEIGHT = 3000, X = 115, Y = 105;
	const int R2 = 50, R3 = 80, R4 = 102;
	ResizeConsole(WIDTH, HEIGHT);
	SetFontSize(4);
	SetColor(0, 1);
	system("cls");
	FillCircle(X, Y, 20, 14);
	DrawCircle(X, Y, R2, 15);
	DrawCircle(X, Y, R3, 15);
	DrawCircle(X, Y, R4, 15);
	double angle1 = 0;
	double angle2 = 0;
	double angle3 = 0;
	double dang1 = 0.03;
	double dang2 = -0.025;
	double dang3 = 0.031;
	bool done = false;
	while (!done)
	{
		//FillCircle(tempX, tempY, 8, 1);
		//DrawCircle(X, Y, R3, 15, tempX - 8, tempX + 8, tempY - 8, tempY + 8);
		angle2 += dang1;
		angle1 += dang2;
		angle3 += dang3;
		int tempX1 = (int)(X + R2 * cos(angle1));
		int tempY1 = (int)(Y + R2 * sin(angle1));

		int tempX2 = (int)(X + R3 * cos(angle2));
		int tempY2 = (int)(Y + R3 * sin(angle2));

		int tempX3 = (int)(X + R4 * cos(angle3));
		int tempY3 = (int)(Y + R4 * sin(angle3));
		//FillCircle((int)(X + R3 * cos(angle)), (int)(Y + R3 * sin(angle)), 8, 13);
		MyFillCircle(tempX3, tempY3, 6, 10, X, Y, R4, 15);
		MyFillCircle(tempX2, tempY2, 8, 12, X, Y, R3, 15);
		MyFillCircle(tempX1, tempY1, 3, 13, X, Y, R2, 15);
		Sleep(20);
		if (GetAsyncKeyState(VK_ESCAPE))
		{
			done = true;
		}
		if (GetAsyncKeyState('1'))
		{
			dang1 *= -1;
		}
		if (GetAsyncKeyState('2'))
		{
			dang2 *= -1;
		}
		if (GetAsyncKeyState('3'))
		{
			dang3 *= -1;
		}
	}
}

void MyFillCircle(int x, int y, int r, int c, int cx, int cy, int cr, int cc)
{
	const int BUFFER = (int)(r / 2 + 2);
	for (int j = y - r - BUFFER; j < y + r + BUFFER; j++)
	{
		MoveCursor(2 * (x - r - BUFFER), j);

		for (int i = x - r - BUFFER; i < x + r + BUFFER; i++)
		{
			if (abs(sqrt(pow(i - x, 2) + pow(j - y, 2))) <= r)
			{
				SetColor(0, c);
			}
			else if (abs(cr - sqrt(pow(i - cx, 2) + pow(j - cy, 2))) <= 1)
			{
				SetColor(0, cc);
			}
			else
			{
				SetColor(0, 1);
			}

			cout << "  ";
		}
	}
}

void DrawCircle(int x, int y, int r, int c)
{
	SetColor(0, c);
	for (int i = x - r; i < x + r; i++)
	{
		for (int j = y - r; j < y + r; j++)
		{
			if (abs(sqrt(pow(i - x, 2) + pow(j - y, 2)) - r) <= 1)
			{
				MoveCursor(2 * i, j);
				cout << "  ";
			}
		}
	}
}

void DrawCircle(int x, int y, int r, int c, int startX, int endX, int startY, int endY)
{
	for (int j = startY; j < endY; j++)
	{
		MoveCursor(startX * 2, j);
		for (int i = startX; i < endX; i++)
		{
			if (abs(sqrt(pow(i - x, 2) + pow(j - y, 2)) - r) <= 1)
			{
				SetColor(0, c);
			}
			else
			{
				SetColor(0, 1);
			}

			cout << "  ";
		}

		
	}
}

void FillCircle(int x, int y, int r, int c)
{
	SetColor(0, c);
	for (int i = x - r; i < x + r; i++)
	{
		for (int j = y - r; j < y + r; j++)
		{
			if (sqrt(pow(i - x, 2) + pow(j - y, 2)) <= r)
			{
				MoveCursor(2 * i, j);
				cout << "  ";
			}
		}
	}
}