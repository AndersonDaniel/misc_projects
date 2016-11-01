#ifndef CONSOLE_UTIL_H
#define CONSOLE_UTIL_H

#include <Windows.h>

unsigned short GetColor(int textcolor, int backcolor);
void SetColor(int textcolor, int backcolor);
void SetColor(unsigned short attrs);
void ResizeConsole(int nWidth, int nHeight);
void SetFontSize(int nSize);
COORD MoveCursor(int nDestX, int nDestY);

#endif