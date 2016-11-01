// PicsToAscii.cpp

#include <iostream>
#include <fstream>
#include <string.h>
#include <math.h>

using namespace std;

struct Color
{
	int R;
	int G;
	int B;
	int Code; 
};

struct ColorYUV
{
	double Y;
	double U;
	double V;
};

int ReadInvert(fstream*);
ColorYUV RGBtoYUV(Color c);
void PrintPixel(Color c);
char* strrev(char* str);
int Max(int, int, int);
int Min(int, int, int);
int Med(int, int, int);
void swap(int&, int&);
char* GetOrder(Color);

int main()
{
	fstream fsPic;
	fstream fsNew;
	fsPic.open("1.bmp", ios::in | ios::binary);
	int nWidth, nHeight, nOffset, nTemp;
	int B, G, R, A;
	fsPic.seekg(10, ios::beg);
	fsPic.read((char*)&nOffset, 4);
	fsPic.seekg(18, ios::beg);
	nWidth = ReadInvert(&fsPic);
	nHeight = ReadInvert(&fsPic); 
	Color* arrMat = new Color[nHeight * nWidth];
	fsPic.seekg(54, ios::beg);
	for (int nRow = nHeight - 1; nRow >= 0; nRow--)
	{
		for (int nCol = 0; nCol < nWidth; nCol++)
		{
			arrMat[nRow * nWidth + nCol].B = fsPic.get();
			arrMat[nRow * nWidth + nCol].G = fsPic.get();
			arrMat[nRow * nWidth + nCol].R = fsPic.get();
		}
		for (int nCol = 0; nCol < (4 - (nWidth * 3) % 4) % 4; nCol++)
		{
			fsPic.get();
		}
	}

	for (int nRow = 0; nRow < nHeight; nRow++)
	{
		for (int nCol = 0; nCol < nWidth; nCol++)
		{
			PrintPixel(arrMat[nRow * nWidth + nCol]);
		}
		cout << endl;
	}
	return 0;
}

int ReadInvert(fstream* fs)
{
	int nFirst = fs->get();
	int nSecond = fs->get(); 
	int nThird = fs->get();
	int nFourth = fs->get();
	return (nFirst + (2 << 7) * nSecond + (2 << 15) * nThird + (2 << 23) * nFourth);
}


ColorYUV RGBtoYUV(Color c)
{
	const double Wr = 0.299; 
	const double Wb = 0.144;
	const double Wg = 1 - Wr - Wb;
	const double Umax = 0.436;
	const double Vmax = 0.615;
	ColorYUV cYUV;
	cYUV.Y = Wr * c.R + Wb * c.B + Wg * c.G;
	cYUV.U = Umax * ((c.B - cYUV.Y) / (1 - Wb));
	cYUV.V = Vmax * ((c.R - cYUV.Y) / (1 - Wr));

	return (cYUV);
}


void PrintPixel(Color c)
{
	Color cols[] = {	{0, 0, 0, 30},
						{187, 0, 0, 31},
						{0, 187, 0, 32},
						{187, 187, 0, 33},
						{0, 0, 187, 34},
						{187, 0, 187, 35},
						{0, 187, 187, 36},
						{187, 187, 187, 37},
						{85, 85, 85, 90},
						{255, 85, 85, 91},
						{85, 255, 85, 92},
						{255, 255, 85, 93},
						{85, 85, 255, 94}, 
						{255, 85, 255, 95},
						{85, 255, 255, 96},
						{255, 255, 255, 97}};

	//double dMinDist = sqrt(256 * 256 * 2);
	double dMinDist = sqrt(256 * 256 * 5); 
	char pix[] = "MNdhymaos:+-.` ";
	int nMinDistCol = -1; 
	for (int i = 0; i < 16; i++)
	{
		ColorYUV cTemp1 = RGBtoYUV(c);
		ColorYUV cTemp2 = RGBtoYUV(cols[i]);
		//double dDist =	(pow(cTemp1.U - cTemp2.U, 2) + pow(cTemp1.V - cTemp2.V, 2));
		double dDist = (pow(c.R - cols[i].R, 2) + pow(c.B - cols[i].B, 2) + pow(c.G - cols[i].G, 2));
		dDist = sqrt(dDist);
		char* c1 = GetOrder(c);
		char* c2 = GetOrder(cols[i]);
		double dTemp = 0; 
		for (int j = 0; j < 3; j++)
		{
			if ((c1[j] & c2[j] != 'R') &&
				(c1[j] & c2[j] != 'B') && 
				(c1[j] & c2[j] != 'G'))
			{
				dTemp += 1.5 * dDist;
			}
		}
		dDist += dTemp;
		delete[] c1;
		delete[] c2;
		if (dDist < dMinDist)
		{
			dMinDist = dDist;
			nMinDistCol = i;
		}
	}
	int nMax = 256 / 3 + 1;  
	int nDarkness = (int)(RGBtoYUV(c).Y) / nMax;	
	int nBackColor, nForeColor; 
	switch (nDarkness)
	{
		case (0):
		{
			nBackColor = 40;
			nForeColor = cols[nMinDistCol].Code;
			break;
		}
		case (1):
		{
			nBackColor = cols[nMinDistCol].Code + 10;
			nForeColor = 97;
			break;
		}
		case (2): 
		{
			nBackColor = 107;
			nForeColor = cols[nMinDistCol].Code;
			break;
		}
	}

	char* szOpt = nDarkness > 1 ? (strdup(pix)) : (strrev(strdup(pix))); 
	char prefix[] = "\033[";
	cout << prefix << nBackColor << "m" << prefix << nForeColor << "m";
	cout << szOpt[strlen(szOpt) * ((int)RGBtoYUV(c).Y % nMax) / nMax];
	cout << "\033[0m";
	delete[] szOpt;
}


char* strrev(char* str)
{
	for (int i = 0; i < strlen(str) / 2; i++)
	{
		char cTemp = str[i];
		str[i] = str[strlen(str) - i - 1];
		str[strlen(str) - i] = cTemp;
	}
	return (str);
}

char* GetOrder(Color c)
{
	char* arrcOrder = new char[3];
	int nMax = Max(c.R, c.G, c.B);
	int nMid = Med(c.R, c.G, c.B);
	int nMin = Min(c.R, c.G, c.B);  
	arrcOrder[0] = arrcOrder[1] = arrcOrder[2] = 0;
	if (c.R == nMax) arrcOrder[0] |= 'R';
	if (c.B == nMax) arrcOrder[0] |= 'B';
	if (c.G == nMax) arrcOrder[0] |= 'G'; 
	if (c.R == nMid) arrcOrder[1] |= 'R';
	if (c.B == nMid) arrcOrder[1] |= 'B';
	if (c.G == nMid) arrcOrder[1] |= 'G';
	if (c.R == nMin) arrcOrder[2] |= 'R';
	if (c.B == nMin) arrcOrder[2] |= 'B';
	if (c.G == nMin) arrcOrder[2] |= 'G';
	return arrcOrder;
}

int Max(int a, int b, int c)
{
	if (a > b)
	{
		if (a > c) return a;
		else return c;
	}
	else if (b > c) return b;
	else return c;
}

int Med(int a, int b, int c)
{
	if (a < b) swap(a, b);
	if (b < c) swap(b , c);
	if (a < b) swap(a , b);
	return b;
}

int Min(int a, int b, int c)
{
	if (a < b) swap(a, b);
	if (b < c) swap(b, c);
	if (a < b) swap(a, b);
	return c;
}

void swap(int& a, int& b)
{
	a += b;
	b = a - b;
	a -= b;
}
