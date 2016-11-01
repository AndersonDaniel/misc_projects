using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pentago
{
    public class Board
    {
        private Player[,] m_matPlayer;
        private static Image ms_imgEmptyBoard = null;
        public Board()
        {
            this.m_matPlayer = new Player[Consts.SIZE, Consts.SIZE];
            this.Initialize();
        }
        public Player this[int x, int y]
        {
            get
            {
                return (this.m_matPlayer[y, x]);
            }
            set
            {
                this.m_matPlayer[y, x] = value;
            }
        }

        public void Rotate(bool bClockWise)
        {
            int ri = bClockWise ? 3 : 1;
            Player[,] matNewBoard = new Player[Consts.SIZE, Consts.SIZE];
            for (int i = 0; i < Consts.SIZE; i++)
            {
                for (int j = 0; j < Consts.SIZE; j++)
                {
                    int frow = i - 1;
                    int fcol = j - 1;
                    int nRow = 
                        (1 - (ri - 1) % 4) * ((ri % 2) * fcol) +
                        (1 - ri % 4) * (Math.Abs(ri % 2 - 1) * frow);
                    int nCol =
                        (1 - ri % 4) * (Math.Abs(ri % 2 - 1) * fcol) +
                        ((ri - 1) % 4 - 1) * ((ri % 2) * frow);
                    matNewBoard[nCol + 1, nRow + 1] = this.m_matPlayer[fcol + 1, frow + 1];
                }
            }

            this.m_matPlayer = matNewBoard;
        }

        public static Image GetEmptyBoard()
        {
            if (Board.ms_imgEmptyBoard == null)
            {
                Image imgTemp = (Image)Consts.BOARD_IMAGE.Clone();
                Graphics g = Graphics.FromImage(imgTemp);
                for (int i = 0; i < Consts.SIZE; i++)
                {
                    for (int j = 0; j < Consts.SIZE; j++)
                    {
                        g.DrawImage(Consts.SPHERE_IMAGES[(int)Player.Empty],
                                    (Consts.SPHERE_SIZE + Consts.SPACE) * j + Consts.SPACE,
                                    (Consts.SPHERE_SIZE + Consts.SPACE) * i + Consts.SPACE,
                                    Consts.SPHERE_SIZE, Consts.SPHERE_SIZE);
                    }
                }

                Board.ms_imgEmptyBoard = imgTemp;
            }

            return ((Image)Board.ms_imgEmptyBoard.Clone());
        }

        public static Point GetClosestSphere(int nRelativeX, int nRelativeY)
        {
            Point pRes = new Point();
            pRes.X = (int)Math.Floor(nRelativeX / ((double)Consts.BRD_WIDTH / 3));
            pRes.Y = (int)Math.Floor(nRelativeY / ((double)Consts.BRD_HEIGHT / 3));
            return (pRes);
        }

        public static Point GetSphereLocation(Point pSphere)
        {
            return (new Point((int)(((Consts.SPHERE_SIZE + Consts.SPACE) * pSphere.X) * Consts.RELATION),
                              (int)(((Consts.SPHERE_SIZE + Consts.SPACE) * pSphere.Y) * Consts.RELATION)));
        }

        public void Initialize()
        {
            for (int i = 0; i < Consts.SIZE; i++)
            {
                for (int j = 0; j < Consts.SIZE; j++)
                {
                    this.m_matPlayer[j, i] = Player.Empty;
                }
            }
        }
    }
}
