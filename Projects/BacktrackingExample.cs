using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BacktrackingExample
{
    class Program
    {
        const char START_C = '@';
        const char END_C = '&';
        const string FILE_NAME = "maze.txt";
        static void Main(string[] args)
        {
            char[,] arrcMaze = GetMaze(FILE_NAME);
            int height = arrcMaze.GetLength(0);
            int width = arrcMaze.GetLength(1);
            bool[,] bVisited = new bool[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bVisited[i, j] = false;
                }
            }
            int startX = 0, startY = 0, endX = 0, endY = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (arrcMaze[i, j] == START_C)
                    {
                        startX = j;
                        startY = i;
                        Console.CursorTop = i;
                        Console.CursorLeft = j;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(START_C);
                        Console.ResetColor();
                    }
                    else if (arrcMaze[i, j] == END_C)
                    {
                        endX = j;
                        endY = i;
                        Console.CursorTop = i;
                        Console.CursorLeft = j;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.Write(END_C);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(arrcMaze[i, j]);
                    }
                }
                Console.WriteLine();
            }
            //Console.Clear();
            bool b = SolveX(arrcMaze, bVisited, startX, startY, endX, endY, width, height);
            System.Threading.Thread.Sleep(250);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (arrcMaze[i, j] == '?')
                    {
                        Console.CursorLeft = j;
                        Console.CursorTop = i;
                        Console.BackgroundColor = ConsoleColor.Cyan;
                        Console.Write(" ");
                    }
                }
            }
            Console.CursorLeft = 0;
            Console.CursorTop = height;
            Console.ReadLine();
        }

        /// <summary>
        /// Solves a maze using BACKTRACKING
        /// </summary>
        /// <param name="maze">char[,]. Matrix of the maze cells</param>
        /// <param name="bVisited"></param>
        /// <param name="curX">int. The current location x</param>
        /// <param name="curY">int. The current location y</param>
        /// <param name="destX">int. The destination's x</param>
        /// <param name="destY">int. The destination's y</param>
        /// <param name="w">int. The maze's width</param>
        /// <param name="h">int. The maze's height</param>
        /// <returns>bool. If the maze was solved or not</returns>
        static bool SolveX(char[,] maze,
                          bool[,] bVisited,
                          int curX,
                          int curY,
                          int destX,
                          int destY,
                          int w,
                          int h)
        {
            // Const definition
            const char EMPTY_CELL   = ' ';
            const char TEMP_CHAR    = '?';
            const char END_CHAR     = '&';

            // Create a direction vector
            List<Point> lstDirs = new List<Point>();
            lstDirs.Add(new Point(0, 1));
            lstDirs.Add(new Point(0, -1));
            lstDirs.Add(new Point(-1, 0));
            lstDirs.Add(new Point(1, 0));

            // For every possible direction
            foreach (Point dir in lstDirs)
            {
                // Calculate the next point in that direction
                int newX = curX + dir.X;
                int newY = curY + dir.Y;

                // If the next point is inside the maze,
                // and its cell is empty
                if ((newX >= 0) &&
                    (newX < w) &&
                    (newY >= 0) &&
                    (newY < h) &&
                    (maze[newY, newX] == ' ') &&
                    (!bVisited[newY, newX]))
                {
                    bVisited[newY, newX] = true;
                    System.Threading.Thread.Sleep(50);
                    maze[newY, newX] = '?';
                    Console.CursorLeft = newX;
                    Console.CursorTop = newY;
                    Console.Write("?");
                    if (SolveX(maze, bVisited, newX, newY, destX, destY, w, h))
                    {
                        return (true);
                    }
                    maze[newY, newX] = ' ';
                    System.Threading.Thread.Sleep(50);
                    Console.CursorLeft = newX;
                    Console.CursorTop = newY;
                    Console.Write(" ");
                }
                if (newX >= 0 && newX < w && newY >= 0 && newY < h && maze[newY, newX] == END_CHAR)
                {
                    return (true);
                }
            }

            return (false);
        }

        static char[,] GetMaze(string strFile)
        {
            FileStream fsFile = new FileStream(strFile, FileMode.Open);
            StreamReader reader = new StreamReader(fsFile);
            List<string> lstLines = new List<string>();
            while (!reader.EndOfStream)
            {
                lstLines.Add(reader.ReadLine());
            }

            char[,] arrcMaze = new char[lstLines.Count, lstLines[0].Length];

            for (int i = 0; i < lstLines.Count; i++)
            {
                char[] temp = lstLines[i].ToCharArray();
                for (int j = 0; j < temp.Length; j++)
                {
                    arrcMaze[i, j] = temp[j];
                }
            }

            return (arrcMaze);
        }

        class Point
        {
            public int X;
            public int Y;
            public Point(int x, int y)
            {
                this.X = x;
                this.Y = y;
            }
            public Point() : this(0, 0) { }
        }

        /// <summary>
        /// Solve a maze using backtracking
        /// </summary>
        /// <param name="maze">
        /// char[,]. A matrix representing the maze
        /// </param>
        /// <param name="curX">
        /// int. The current X location
        /// </param>
        /// <param name="curY">
        /// int. The current Y location
        /// </param>
        /// <param name="w">
        /// int. The maze's width
        /// </param>
        /// <param name="h">
        /// int. The maze's height
        /// </param>
        /// <returns>
        /// bool. True if the maze has been solved,
        /// otherwise false.
        /// </returns>
        static bool Solve(char[,] maze,
                          int curX,
                          int curY,
                          int w,
                          int h)
        {
            // Const declaration
            const char EMPTY_CELL = ' ';
            const char TEMP_CELL = '?';
            const char END_CELL = '&';

            // Defining the direction vector
            List<Point> lstDirs = new List<Point>();
            lstDirs.Add(new Point(0, 1));
            lstDirs.Add(new Point(0, -1));
            lstDirs.Add(new Point(-1, 0));
            lstDirs.Add(new Point(1, 0));

            // For every possible direction
            foreach (Point dir in lstDirs)
            {
                // Find the next location when moving in that
                // direction
                int newX = curX + dir.X;
                int newY = curY + dir.Y;

                // If the next location is within the maze,
                // and its cell is empty
                if ((newX >= 0) &&
                    (newX < w) &&
                    (newY >= 0) &&
                    (newY < h) &&
                    (maze[newY, newX] == EMPTY_CELL))
                {
                    // Mark that cell as a possible part
                    // of the solution
                    maze[newY, newX] = TEMP_CELL;

                    // If the maze can be solved from the new
                    // location
                    if (Solve(maze, newX, newY, destX, destY, w, h))
                    {
                        // Return true - the maze can be solved
                        return (true);
                    }

                    // If we reached here,
                    // the maze may not be solved from the new
                    // location (in the current
                    // circumstances) - mark that cell
                    // as an empty cell
                    maze[newY, newX] = EMPTY_CELL;
                }
                // Else, if the cell is in the maze and
                // is the destination cell
                else if ((newX >= 0) &&
                         (newX < w) &&
                         (newY >= 0) &&
                         (newY < h) &&
                         (maze[newY, newX] == END_CELL))
                {
                    // Return true - the maze can be solved
                    return (true);
                }
            }

            // If we reached here, the maze cannot be solved
            // from the current location (with the given solution)
            // - return false
            return (false);
        }
    }
}
