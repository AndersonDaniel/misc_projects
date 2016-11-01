using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Pentago
{
    public partial class frmMain : Form
    {
        // Const definition
        private const int START_X = 270;
        private const int START_Y = 215;
        private const int BOARD_SPACE = 5;
        private const int SPIN_DELTA = 50;

        // Variable definition
        private Image m_imgBackImage;
        private Image m_imgCurrImage;
        private DynamicImage[,] m_arrdiBoards;
        private Player m_plrCurrentTurn;
        private bool m_bClockWise;
        private Thread m_thSpinningThread;
        private bool m_bPlaceTime;
        private Point m_pLastOption;
        private DynamicImage m_diLast = null;
        private bool m_bMayCheckWinner;

        public frmMain()
        {
            InitializeComponent();
            this.Icon = Icon.FromHandle(((Bitmap)Consts.DECOR).GetHicon());
            this.Width = Consts.BACKGROUND_IMAGE.Width;
            this.Height = Consts.BACKGROUND_IMAGE.Height;
            this.m_plrCurrentTurn = Player.Empty;
            this.m_pLastOption = new Point(-1, -1);
            this.m_imgBackImage = Consts.BACKGROUND_IMAGE;
            this.m_imgCurrImage = (Image)this.m_imgBackImage.Clone();
            this.m_arrdiBoards = new DynamicImage[Consts.BOARDS, Consts.BOARDS];
            for (int i = 0; i < Consts.BOARDS; i++)
            {
                for (int j = 0; j < Consts.BOARDS; j++)
                {
                    this.m_arrdiBoards[i, j] =
                        new DynamicImage(Board.GetEmptyBoard(),
                                         START_X + (Consts.BRD_WIDTH + BOARD_SPACE) * j,
                                         START_Y + (Consts.BRD_HEIGHT + BOARD_SPACE) * i,
                                         START_X + (Consts.BRD_WIDTH + BOARD_SPACE) * j + SPIN_DELTA * (2 * j - 1),
                                         START_Y + (Consts.BRD_HEIGHT + BOARD_SPACE) * i + SPIN_DELTA * (2 * i - 1),
                                         Consts.BRD_WIDTH, Consts.BRD_HEIGHT);
                }
            }

            Graphics.FromImage(this.m_imgCurrImage).DrawImage(Consts.DECOR,
                                                              START_X, START_Y,
                                                              2 * Consts.BRD_WIDTH,
                                                              2 * Consts.BRD_HEIGHT);
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                            0, 0,
                                            this.Width, this.Height);
            this.m_pLastOption = new Point(-1, -1);
            this.ShowOption();
        }

        private void StartGame()
        {
            if ((this.m_thSpinningThread != null) &&
                (this.m_thSpinningThread.ThreadState == ThreadState.Running))
            {
                this.m_thSpinningThread.Abort();
                this.m_thSpinningThread = null;
            }
            this.m_imgCurrImage = (Image)this.m_imgBackImage.Clone();
            this.m_plrCurrentTurn = Player.Gold;
            this.m_bPlaceTime = true;
            foreach (DynamicImage di in this.m_arrdiBoards)
            {
                di.Image = Board.GetEmptyBoard();
                di.Board.Initialize();
            }
            this.DrawBoards();
        }

        private void DrawBoards()
        {
            this.m_imgCurrImage = (Image)Consts.BACKGROUND_IMAGE.Clone();
            Graphics g = Graphics.FromImage(this.m_imgCurrImage);
            foreach (DynamicImage di in this.m_arrdiBoards)
            {
                g.DrawImage(di.Image,
                            di.Location.X, di.Location.Y,
                            di.Size.Width, di.Size.Height);
            }

            this.CreateGraphics().DrawImage(this.m_imgCurrImage, 0, 0, this.Width, this.Height);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.StartGame();
        }

        private void SpinImage(object oToSpin)
        {
            DynamicImage diToSpin = (DynamicImage)oToSpin;
            diToSpin.Board.Rotate(this.m_bClockWise);
            this.m_bMayCheckWinner = true;
            Image imgTemp = (Image)Consts.BACKGROUND_IMAGE.Clone();
            Graphics g = Graphics.FromImage(imgTemp);
            foreach (DynamicImage di in this.m_arrdiBoards)
            {
                if (di != diToSpin)
                {
                    g.DrawImage(di.Image,
                                di.Location.X, di.Location.Y,
                                di.Size.Width, di.Size.Height);
                }
            }

            const double INC = 2.5;
            Image imgTemp2;
            for (double i = 0; i <= 90; i += INC)
            {
                int nOffsetX, nOffsetY;
                nOffsetX = (int)((diToSpin.Destination.X - diToSpin.Location.X) *
                           ((90 * i - i * i) / 2025));
                nOffsetY = (int)((diToSpin.Destination.Y - diToSpin.Location.Y) *
                           ((90 * i - i * i) / 2025));
                imgTemp2 = (Image)imgTemp.Clone();
                g = Graphics.FromImage(imgTemp2);
                g.TranslateTransform(diToSpin.Location.X + diToSpin.Size.Width / 2 + nOffsetX,
                                     diToSpin.Location.Y + diToSpin.Size.Height / 2 + nOffsetY);
                g.RotateTransform((this.m_bClockWise ? 1 : (-1)) * (float)i);
                g.TranslateTransform(-diToSpin.Size.Width / 2,
                                     -diToSpin.Size.Height / 2);
                g.DrawImage(diToSpin.Image, 0, 0, diToSpin.Size.Width, diToSpin.Size.Height);
                this.CreateGraphics().DrawImage(imgTemp2, 0, 0, this.Width, this.Height);
                this.m_imgCurrImage = (Image)imgTemp2.Clone();
            }

            diToSpin.Image.RotateFlip(this.m_bClockWise ? RotateFlipType.Rotate90FlipNone :
                                                          RotateFlipType.Rotate270FlipNone);
        }

        private void gameRulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form rulez = new frmRulez(this.Left + this.Width / 2,
                                      this.Top + this.Height);
            rulez.ShowDialog();
            this.Winner();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ((this.m_thSpinningThread != null) &&
                (this.m_thSpinningThread.ThreadState == ThreadState.Running))
            {
                this.m_thSpinningThread.Abort();
            }

            this.Close();
        }

        private static Bitmap SetAlpha(Image img, int nAlpha)
        {
            Bitmap bmp = (Bitmap)img.Clone();
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    Color cTemp = bmp.GetPixel(j, i);
                    if (cTemp.A == 255)
                    {
                        cTemp = Color.FromArgb(nAlpha, cTemp);
                        bmp.SetPixel(j, i, cTemp);
                    }
                }
            }

            return (bmp);
        }

        private DynamicImage ContainingBoard(int nX, int nY)
        {
            foreach (DynamicImage di in this.m_arrdiBoards)
            {
                if ((nX >= di.Location.X) &&
                    (nY >= di.Location.Y) &&
                    (nX < di.Location.X + di.Size.Width) &&
                    (nY < di.Location.Y + di.Size.Height))
                {
                    return (di);
                }
            }

            return (null);
        }

        private void ShowOption()
        {
            if ((!this.m_bPlaceTime) || (this.m_plrCurrentTurn == Player.Empty))
            {
                this.m_pLastOption = new Point(-1, -1);
                this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                                0,
                                                0,
                                                this.Width,
                                                this.Height);
                return;
            }

            Point pCursor = this.PointToClient(Cursor.Position);
            
            DynamicImage diCurrent = this.ContainingBoard(pCursor.X, pCursor.Y);
            if (diCurrent == null)
            {
                this.m_pLastOption = new Point(-1, -1);
                this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                                0,
                                                0,
                                                this.Width,
                                                this.Height);
                return;
            }

            Point pSphereLoc = Board.GetClosestSphere(pCursor.X - diCurrent.Location.X,
                                                               pCursor.Y - diCurrent.Location.Y);

            Point pToDraw =
                Board.GetSphereLocation(pSphereLoc);

            if (diCurrent.Board[pSphereLoc.X, pSphereLoc.Y] != Player.Empty)
            {
                return;
            }

            if (pToDraw != this.m_pLastOption)
            {
                this.m_pLastOption = pToDraw;
                pToDraw.X += diCurrent.Location.X;
                pToDraw.Y += diCurrent.Location.Y;
                int nNewSize = (int)(Consts.SPHERE_SIZE * Consts.RELATION);
                this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                                0,
                                                0,
                                                this.Width,
                                                this.Height);
                this.CreateGraphics().DrawImage(frmMain.SetAlpha(Consts.SPHERE_IMAGES[(int)this.m_plrCurrentTurn],
                                                                 200),
                                                pToDraw.X + (Consts.SPHERE_SIZE - nNewSize) / 2,
                                                pToDraw.Y + (Consts.SPHERE_SIZE - nNewSize) / 2,
                                                nNewSize,
                                                nNewSize);
            }
        }

        private void frmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_bPlaceTime)
            {
                this.ShowOption();
            }
            else
            {
                this.ShowRotateOption();
            }
        }

        private void ShowRotateOption()
        {
            if ((this.m_bPlaceTime) || (this.m_plrCurrentTurn == Player.Empty))
            {
                this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                                0, 0, this.Width, this.Height);
                return;
            }
            Point pCursor = this.PointToClient(Cursor.Position);
            DynamicImage diCurrent = this.ContainingBoard(pCursor.X, pCursor.Y);
            if (diCurrent != this.m_diLast)
            {
                this.m_diLast = diCurrent;
                if (diCurrent != null)
                {
                    Pen p = new Pen(this.m_plrCurrentTurn == Player.Red ?
                                    Color.Red : Color.Gold,
                                    4.0f);
                    Graphics g = this.CreateGraphics();
                    g.DrawImage(this.m_imgCurrImage,
                                0, 0, this.Width, this.Height);
                    g.DrawRectangle(p,
                                    new Rectangle(diCurrent.Location.X,
                                                  diCurrent.Location.Y,
                                                  diCurrent.Size.Width,
                                                  diCurrent.Size.Height));
                }
            }
        }

        private void PlaceSphere()
        {
            if ((!this.m_bPlaceTime) || (this.m_plrCurrentTurn == Player.Empty))
            {
                return;
            }

            Point pCursor = this.PointToClient(Cursor.Position);

            DynamicImage diCurrent = this.ContainingBoard(pCursor.X, pCursor.Y);
            if (diCurrent == null)
            {
                this.m_pLastOption = new Point(-1, -1);
                this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                                0,
                                                0,
                                                this.Width,
                                                this.Height);
                return;
            }

            Point pSphereLoc = Board.GetClosestSphere(pCursor.X - diCurrent.Location.X,
                                                               pCursor.Y - diCurrent.Location.Y);

            Point pToDraw =
                Board.GetSphereLocation(pSphereLoc);

            if (diCurrent.Board[pSphereLoc.X, pSphereLoc.Y] != Player.Empty)
            {
                return;
            }

            int nNewSize = (int)(Consts.SPHERE_SIZE * Consts.RELATION);
            Graphics.FromImage(diCurrent.Image).DrawImage(Consts.SPHERE_IMAGES[(int)this.m_plrCurrentTurn],
                                                          pSphereLoc.X * (Consts.SPHERE_SIZE + Consts.SPACE) +
                                                          Consts.SPACE,
                                                          pSphereLoc.Y * (Consts.SPHERE_SIZE + Consts.SPACE) +
                                                          Consts.SPACE,
                                                          Consts.SPHERE_SIZE,
                                                          Consts.SPHERE_SIZE);
            pToDraw.X += diCurrent.Location.X;
            pToDraw.Y += diCurrent.Location.Y;
            Graphics.FromImage(this.m_imgCurrImage).DrawImage(Consts.SPHERE_IMAGES[(int)this.m_plrCurrentTurn],
                                            pToDraw.X + (Consts.SPHERE_SIZE - nNewSize) / 2,
                                            pToDraw.Y + (Consts.SPHERE_SIZE - nNewSize) / 2,
                                            nNewSize,
                                            nNewSize);
            this.CreateGraphics().DrawImage(this.m_imgCurrImage, 0, 0, this.Width, this.Height);
            diCurrent.Board[pSphereLoc.X, pSphereLoc.Y] = this.m_plrCurrentTurn;
            this.m_bPlaceTime = false;
            this.m_diLast = null;
            this.ShowRotateOption();
        }

        private void frmMain_MouseClick(object sender, MouseEventArgs e)
        {
            if (this.m_bPlaceTime)
            {
                this.PlaceSphere();
            }
            else
            {
                this.RotateBoard(e.Button == MouseButtons.Right);
            }
        }

        private void RotateBoard(bool bClockWise)
        {
            this.m_bClockWise = bClockWise;
            Point pCursor = this.PointToClient(Cursor.Position);
            DynamicImage diCurrent = this.ContainingBoard(pCursor.X, pCursor.Y);
            if (diCurrent != null)
            {
                if ((this.m_thSpinningThread == null) ||
                    (this.m_thSpinningThread.ThreadState != ThreadState.Running))
                {
                    this.m_thSpinningThread =
                        new Thread(new ParameterizedThreadStart(this.SpinImage));
                    this.m_bMayCheckWinner = false;
                    this.m_thSpinningThread.Start(diCurrent);
                    while (!this.m_bMayCheckWinner) ;
                    this.OnFinishRotation();
                }
            }
        }

        private void OnFinishRotation()
        {
            if (this.Winner() == Player.Empty)
            {
                this.m_bPlaceTime = true;
                this.m_plrCurrentTurn = (Player)(3 - (int)this.m_plrCurrentTurn);
                this.m_pLastOption = new Point(-1, -1);
                this.ShowOption();
            }
            else
            {
                MessageBox.Show("We have a winner! " + this.Winner().ToString() + "!");
                this.CreateGraphics().DrawImage(this.m_imgCurrImage, 0, 0, this.Width, this.Height);
                for (int i = 0; i < 50; i++)
                {
                    this.CreateGraphics().DrawImage(Consts.SPHERE_IMAGES[(int)this.Winner()],
                                                    Consts.GetRand(50, this.Width - 100),
                                                    Consts.GetRand(50, this.Height - 100),
                                                    80, 80);
                    Thread.Sleep(60);
                }

                this.m_bPlaceTime = false;
                this.m_plrCurrentTurn = Player.Empty;
                this.m_imgCurrImage = (Image)this.m_imgBackImage.Clone();
                this.CreateGraphics().DrawImage(this.m_imgCurrImage,
                                                0, 0, this.Width, this.Height);
            }
        }

        private Player Winner()
        {
            Player[,] matTemp = new Player[Consts.SIZE * Consts.SIZE + 2, Consts.SIZE * Consts.SIZE + 2];
            for (int i = 0; i < Consts.SIZE * Consts.SIZE + 2; i++)
            {
                for (int j = 0; j < Consts.SIZE * Consts.SIZE + 2; j++)
                {
                    matTemp[i, j] = Player.Empty;
                }
            }
            for (int i = 0; i < Consts.BOARDS; i++)
            {
                for (int j = 0; j < Consts.BOARDS; j++)
                {
                    for (int k = 0; k < Consts.SIZE; k++)
                    {
                        for (int l = 0; l < Consts.SIZE; l++)
                        {
                            matTemp[1 + i * Consts.SIZE + k,
                                    1 + j * Consts.SIZE + l] = this.m_arrdiBoards[i, j].Board[l, k];
                        }
                    }
                }
            }

            Point[] arrDir = new Point[] {new Point(1, 0), 
                                          new Point(1, 1),
                                          new Point(0, 1),
                                          new Point(1, -1)};

            for (int i = 0; i < Consts.SIZE * Consts.BOARDS + 2; i++)
            {
                for (int j = 0; j < Consts.SIZE * Consts.BOARDS + 2; j++)
                {
                    if (matTemp[i, j] != Player.Empty)
                    {
                        for (int dirInd = 0; dirInd < arrDir.Length; dirInd++)
                        {
                            int nCount = 1;
                            Point pNext = new Point(j + arrDir[dirInd].X,
                                                    i + arrDir[dirInd].Y);
                            while (matTemp[pNext.Y, pNext.X] == matTemp[i, j])
                            {
                                nCount++;
                                pNext.X += arrDir[dirInd].X;
                                pNext.Y += arrDir[dirInd].Y;
                            }

                            pNext = new Point(j - arrDir[dirInd].X,
                                              i - arrDir[dirInd].Y);
                            while (matTemp[pNext.Y, pNext.X] == matTemp[i, j])
                            {
                                nCount++;
                                pNext.X -= arrDir[dirInd].X;
                                pNext.Y -= arrDir[dirInd].Y;
                            }

                            if (nCount >= 5)
                            {
                                return (matTemp[i, j]);
                            }
                        }
                    }
                }
            }

            return (Player.Empty);
        }
    }
}
