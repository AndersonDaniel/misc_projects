using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Collections;

namespace MyExplorer
{
    public partial class Form1 : Form, IFileSystemViewer
    {
        public Form1()
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            this.m_rRand = new Random();
            this.m_lstItems = new List<object[]>();
            this.m_dictObjects = new Dictionary<string, int>();
        }

        #region IFileSystemViewer Members

        private Random m_rRand;
        private string m_strCurrPath;
        private List<object[]> m_lstItems;
        private Bitmap m_bmpDetector;
        private Thread m_thFunThread;
        private Dictionary<string, int> m_dictObjects;
        

        private void HaveFun()
        {
            const int WAIT_TIME = 20;
            Graphics g = this.CreateGraphics();
            Graphics gDetect = Graphics.FromImage(this.m_bmpDetector);
            gDetect.Clear(Color.White);
            while (this.Visible)
            {
                object[] arrObjs;
                Rectangle rect, rectNew;
                for (int i = 0; i < this.m_lstItems.Count; i++)
                {
                    arrObjs = this.m_lstItems[i];
                    MovementHandler mvh = (MovementHandler)arrObjs[2];
                    Image img = (Image)arrObjs[1];
                    int oldx = mvh.X, oldy = mvh.Y;
                    rect = new Rectangle(mvh.X, mvh.Y, img.Width, img.Height);
                    //g.FillRectangle(new SolidBrush(this.BackColor), rect);
                    gDetect.FillRectangle(Brushes.White, rect);
                    mvh.Ping();
                    Bitmap bmp = new Bitmap(img.Width + Math.Abs(oldx - mvh.X),
                                            img.Height + Math.Abs(oldy - mvh.Y));
                    Graphics gBmp = Graphics.FromImage(bmp);
                    gBmp.Clear(this.BackColor);
                    gBmp.DrawImage(img, Math.Max(0, Math.Abs(mvh.X - oldx)),
                                        Math.Max(0, Math.Abs(mvh.Y - oldy)));
                    rectNew = new Rectangle(mvh.X, mvh.Y, img.Width, img.Height);
                    if (mvh.Collides)
                    {
                        CheckCollisions(rectNew, mvh, i);
                    }
                    gDetect.FillRectangle(new SolidBrush(Color.FromArgb(i, i, i)),
                                          rectNew);
                    //g.DrawImage(img, mvh.X, mvh.Y);
                    g.DrawImage(bmp, Math.Min(oldx, mvh.X), Math.Min(oldy, mvh.Y));
                }
                System.Threading.Thread.Sleep(WAIT_TIME);
            }
        }

        private void CheckCollisions(Rectangle rect, MovementHandler mvh, int nIndex)
        {
            int nMidX = this.Width / 2, nMidY = this.Height / 2;
            if ((rect.X < 30) ||
                (rect.Y < 30) ||
                (rect.TopRight().X > this.Width - 100) ||
                (rect.Bottom > this.Height - 100))
            {
                mvh.ImposeTurn(Math.Atan2(nMidY - mvh.Y, nMidX - mvh.X) * 180 / Math.PI);
            }
            else
            {
                /* 1      2 *
                 *          *
                 *          *
                 *          *
                 * 4      3 *
                 */
                int nFirstIndex = this.m_bmpDetector.GetPixel(rect.Location.X,
                                                              rect.Location.Y).R;
                int nSecondIndex = this.m_bmpDetector.GetPixel(rect.TopRight().X,
                                                               rect.TopRight().Y).R;
                int nThirdIndex = this.m_bmpDetector.GetPixel(rect.BottomRight().X,
                                                              rect.BottomRight().Y).R;
                int nFourthIndex = this.m_bmpDetector.GetPixel(rect.BottomLeft().X,
                                                               rect.BottomLeft().Y).R;
                if (nFirstIndex != 255)
                {
                    mvh.ImposeTurn(Math.Atan2(rect.Y - (((MovementHandler)this.m_lstItems[nFirstIndex][2]).Y),
                                              rect.X - (((MovementHandler)this.m_lstItems[nFirstIndex][2]).X)) *
                                              180 / Math.PI);
                }
                else if (nSecondIndex != 255)
                {
                    mvh.ImposeTurn(Math.Atan2(rect.Y - (((MovementHandler)this.m_lstItems[nSecondIndex][2]).Y),
                                              rect.X - (((MovementHandler)this.m_lstItems[nSecondIndex][2]).X)) *
                                              180 / Math.PI);
                }
                else if (nThirdIndex != 255)
                {
                    mvh.ImposeTurn(Math.Atan2(rect.Y - (((MovementHandler)this.m_lstItems[nThirdIndex][2]).Y),
                                              rect.X - (((MovementHandler)this.m_lstItems[nThirdIndex][2]).X)) *
                                              180 / Math.PI);
                }
                else if (nFourthIndex != 255)
                {
                    mvh.ImposeTurn(Math.Atan2(rect.Y - (((MovementHandler)this.m_lstItems[nFourthIndex][2]).Y),
                                              rect.X - (((MovementHandler)this.m_lstItems[nFourthIndex][2]).X)) *
                                              180 / Math.PI);
                }
            }
        }

        private Bitmap CreateItemIcon(FileSystemItem fsiItem)
        {
            Bitmap bmpIcon = new Bitmap(fsiItem.Icon.Width,
                                        fsiItem.Icon.Height + 20);
            Graphics g = Graphics.FromImage(bmpIcon);
            g.Clear(this.BackColor);
            g.DrawImage(fsiItem.Icon, 0, 0);
            Font fToDraw = new Font("Constantia", 12);
            g.DrawString(fsiItem.Name, fToDraw,
                         Brushes.Black, new Point(0, fsiItem.Icon.Height));
            return (bmpIcon);
        }

        public void AddFSItem(FileSystemItem fsiItem)
        {
            const int SPEED = 1;
            Image img = CreateItemIcon(fsiItem);
            Point pLoc =
                new Point(this.m_rRand.Next(0, this.Width - img.Width - 10),
                          this.m_rRand.Next(0, this.Height - img.Height - 10));
            MovementHandler mvh =
                new MovementHandler(this.m_rRand.Next(360), SPEED, 12, pLoc);
            this.m_lstItems.Add(new object[] { fsiItem, img, mvh});
            fsiItem.ContainingViewer = this;
            this.m_dictObjects.Add(fsiItem.Name, this.m_lstItems.Count - 1);
        }

        public string CurrentPath
        {
            get
            {
                return (this.m_strCurrPath);
            }
            set
            {
                this.m_strCurrPath = value;
                this.UpdatePresentation();
            }
        }

        // TODO: implement (updates according to current path)
        private void UpdatePresentation()
        {
            string[] arrPath = this.CurrentPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            string strLastFolder = arrPath[arrPath.Length - 1];
            int nChosenIndex = this.m_dictObjects[strLastFolder];
            for (int i = 0; i < this.m_lstItems.Count; i++)
            {
                if (i != nChosenIndex)
                {

                }
            }
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {
            this.m_bmpDetector = new Bitmap(this.Width, this.Height);
            Graphics.FromImage(this.m_bmpDetector).Clear(Color.White);
            foreach (FileSystemItem fsiItem in FileSystemItem.GetItems("..."))
            {
                this.AddFSItem(fsiItem);
            }

            this.m_thFunThread = new Thread(new ThreadStart(this.HaveFun));
            this.m_thFunThread.IsBackground = true;
            this.m_thFunThread.Start();
        }

        public void ShowChildren(List<FileSystemItem> lstChildren, FileSystemItem fsFather)
        {
        }


        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int index = this.m_bmpDetector.GetPixel(e.X, e.Y).R;
            Process.Start(((FileSystemItem)this.m_lstItems[index][0]).FullPath);
        }
    }
}
