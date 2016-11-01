using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace BooExp
{
    public partial class Form1 : Form
    {
        private Random m_r = new Random();
        private Bitmap BMP_BOO = new Bitmap(BooExp.Properties.Resources.boo1, new Size(75, 50));
        private Bitmap BMP_BOO_BIG = new Bitmap(BooExp.Properties.Resources.boo1, new Size(150, 100));
        private Action m_aCurr = null;
        const int WAIT = 1;

        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        
        [DllImport("user32")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("user32")]
        public static extern int GetForegroundWindow();

        [DllImport("user32")]
        public static extern int RegisterHotKey(IntPtr hwnd, int id, int fsModifiers, int vk);

        IntPtr m_hWnd;
        public Form1()
        {
            InitializeComponent();
            RegisterHotKey(this.Handle, 0, 8, (int)Keys.Y);
            BMP_BOO.MakeTransparent(Color.White);
            BMP_BOO_BIG.MakeTransparent(Color.White);
        }

        //private void Form1_Load(object sender, EventArgs e)
        //{
        //    new Thread(new ThreadStart(this.Foo)).Start();
        //}

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x312)
            {
                this.m_hWnd = (IntPtr)GetForegroundWindow();
                if (this.m_aCurr == null)
                {
                    this.m_aCurr = new Action(() => this.BigBoo());
                    this.m_aCurr.BeginInvoke(t => this.m_aCurr = null, null);
                }
            }

        }

        private void BigBoo()
        {
            Action[] actions = new Action[] { this.BooTop, this.BooBottom, this.BooLeft, this.BooRight,
                                              this.BooTopScreen, this.BooBottomScreen, this.BooLeftScreen, this.BooRightScreen };
            actions[this.m_r.Next(0, actions.Length)].Invoke();
        }

        private void BooTop()
        {
            RECT r = new RECT();
            GetWindowRect(this.m_hWnd, ref r);
            Point pLoc = new Point(this.m_r.Next(r.Left, r.Right - BMP_BOO.Width), r.Top - 1);
            while (r.Top - pLoc.Y < BMP_BOO.Height)
            {
                Bitmap bmp = new Bitmap(BMP_BOO.Width, r.Top - pLoc.Y);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.DrawImage(BMP_BOO, 0, 0);
                this.CreateGraphics().DrawImage(bmp, pLoc);

                Thread.Sleep(WAIT);
                pLoc.Y -= 1;
            }

            Thread.Sleep(1500);

            while (r.Top - pLoc.Y >= 0)
            {
                Bitmap bmp = new Bitmap(BMP_BOO.Width, r.Top - pLoc.Y + 1);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.DrawImage(BMP_BOO, 0, 1);
                this.CreateGraphics().DrawImage(bmp, pLoc);

                Thread.Sleep(WAIT);
                pLoc.Y += 1;
            }
        }

        private void BooRight()
        {
            RECT r = new RECT();
            GetWindowRect(this.m_hWnd, ref r);
            Point pLoc = new Point(r.Right + 1, this.m_r.Next(r.Top, r.Bottom - BMP_BOO.Width));
            while (pLoc.X - r.Right < BMP_BOO.Height)
            {
                Bitmap bmp = new Bitmap(pLoc.X - r.Right, BMP_BOO.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                gBmp.RotateTransform(90);
                gBmp.TranslateTransform(-bmp.Height / 2, -bmp.Width / 2);
                gBmp.DrawImage(BMP_BOO, 0, 0);
                this.CreateGraphics().DrawImage(bmp, r.Right, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X += 1;
            }

            Thread.Sleep(1500);

            while (pLoc.X - r.Right >= 0)
            {
                Bitmap bmp = new Bitmap(pLoc.X - r.Right + 1, BMP_BOO.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform((bmp.Width - 1) / 2, bmp.Height / 2);
                gBmp.RotateTransform(90);
                gBmp.TranslateTransform(-bmp.Height / 2, -(bmp.Width - 1) / 2);
                gBmp.DrawImage(BMP_BOO, 0, 0);
                this.CreateGraphics().DrawImage(bmp, r.Right, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X -= 1;
            }
        }

        private void BooLeft()
        {
            RECT r = new RECT();
            GetWindowRect(this.m_hWnd, ref r);
            Point pLoc = new Point(r.Left - 1, this.m_r.Next(r.Top, r.Bottom - BMP_BOO.Width));
            while (r.Left - pLoc.X < BMP_BOO.Height)
            {
                Bitmap bmp = new Bitmap(r.Left - pLoc.X, BMP_BOO.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                gBmp.RotateTransform(270);
                gBmp.TranslateTransform(-bmp.Height / 2, -bmp.Width / 2);
                gBmp.DrawImage(BMP_BOO, 0, 0);
                this.CreateGraphics().DrawImage(bmp, pLoc.X, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X -= 1;
            }

            Thread.Sleep(1500);

            while (r.Left - pLoc.X >= 0)
            {
                Bitmap bmp = new Bitmap(r.Left - pLoc.X + 1, BMP_BOO.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                gBmp.RotateTransform(270);
                gBmp.TranslateTransform(-bmp.Height / 2, -bmp.Width / 2);
                gBmp.DrawImage(BMP_BOO, 0, 1);
                this.CreateGraphics().DrawImage(bmp, pLoc.X, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X += 1;
            }
        }

        private void BooBottom()
        {
            RECT r = new RECT();
            GetWindowRect(this.m_hWnd, ref r);
            Point pLoc = new Point(this.m_r.Next(r.Left, r.Right - BMP_BOO.Width), r.Bottom + 1);
            while (pLoc.Y - r.Bottom < BMP_BOO.Height)
            {
                Bitmap bmp = new Bitmap(BMP_BOO.Width, pLoc.Y - r.Bottom);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(0, -BMP_BOO.Height + bmp.Height);
                gBmp.TranslateTransform(BMP_BOO.Width / 2, BMP_BOO.Height / 2);
                gBmp.RotateTransform(180);
                gBmp.TranslateTransform(-BMP_BOO.Width / 2, -BMP_BOO.Height / 2);
                gBmp.DrawImage(BMP_BOO, 0, 0);
                Graphics gThis = this.CreateGraphics();
                gThis.DrawImage(bmp, pLoc.X, r.Bottom);

                Thread.Sleep(WAIT);
                pLoc.Y += 1;
            }

            Thread.Sleep(1500);

            while (pLoc.Y - r.Bottom >= 0)
            {
                Bitmap bmp = new Bitmap(BMP_BOO.Width, pLoc.Y - r.Bottom + 1);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(0, -BMP_BOO.Height + bmp.Height - 1);
                gBmp.TranslateTransform(BMP_BOO.Width / 2, BMP_BOO.Height / 2);
                gBmp.RotateTransform(180);
                gBmp.TranslateTransform(-BMP_BOO.Width / 2, -BMP_BOO.Height / 2);
                gBmp.DrawImage(BMP_BOO, 0, 0);
                Graphics gThis = this.CreateGraphics();
                gThis.DrawImage(bmp, pLoc.X, r.Bottom);

                Thread.Sleep(WAIT);
                pLoc.Y -= 1;
            }
        }

        private void BooTopScreen()
        {
            RECT r = new RECT();
            r.Left = 0;
            r.Right = this.Width;
            r.Top = this.Height - 20;
            Point pLoc = new Point(this.m_r.Next(r.Left, r.Right - BMP_BOO_BIG.Width), r.Top - 1);
            while (r.Top - pLoc.Y < BMP_BOO_BIG.Height)
            {
                Bitmap bmp = new Bitmap(BMP_BOO_BIG.Width, r.Top - pLoc.Y);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 0);
                this.CreateGraphics().DrawImage(bmp, pLoc);

                Thread.Sleep(WAIT);
                pLoc.Y -= 1;
            }

            Thread.Sleep(1500);

            while (r.Top - pLoc.Y >= 0)
            {
                Bitmap bmp = new Bitmap(BMP_BOO_BIG.Width, r.Top - pLoc.Y + 1);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 1);
                this.CreateGraphics().DrawImage(bmp, pLoc);

                Thread.Sleep(WAIT);
                pLoc.Y += 1;
            }
        }

        private void BooRightScreen()
        {
            RECT r = new RECT();
            r.Left = 0;
            r.Bottom = this.Height;
            r.Top = 0;
            Point pLoc = new Point(r.Right + 1, this.m_r.Next(r.Top, r.Bottom - BMP_BOO_BIG.Height));
            while (pLoc.X - r.Right < BMP_BOO_BIG.Height)
            {
                Bitmap bmp = new Bitmap(pLoc.X - r.Right, BMP_BOO_BIG.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                gBmp.RotateTransform(90);
                gBmp.TranslateTransform(-bmp.Height / 2, -bmp.Width / 2);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 0);
                this.CreateGraphics().DrawImage(bmp, r.Right, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X += 1;
            }

            Thread.Sleep(1500);

            while (pLoc.X - r.Right >= 0)
            {
                Bitmap bmp = new Bitmap(pLoc.X - r.Right + 1, BMP_BOO_BIG.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform((bmp.Width - 1) / 2, bmp.Height / 2);
                gBmp.RotateTransform(90);
                gBmp.TranslateTransform(-bmp.Height / 2, -(bmp.Width - 1) / 2);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 0);
                this.CreateGraphics().DrawImage(bmp, r.Right, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X -= 1;
            }
        }

        private void BooLeftScreen()
        {
            RECT r = new RECT();
            r.Left = this.Width;
            r.Bottom = this.Height;
            r.Top = 0;
            Point pLoc = new Point(r.Left - 1, this.m_r.Next(r.Top, r.Bottom - BMP_BOO_BIG.Height));
            while (r.Left - pLoc.X < BMP_BOO_BIG.Height)
            {
                Bitmap bmp = new Bitmap(r.Left - pLoc.X, BMP_BOO_BIG.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                gBmp.RotateTransform(270);
                gBmp.TranslateTransform(-bmp.Height / 2, -bmp.Width / 2);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 0);
                this.CreateGraphics().DrawImage(bmp, pLoc.X, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X -= 1;
            }

            Thread.Sleep(1500);

            while (r.Left - pLoc.X >= 0)
            {
                Bitmap bmp = new Bitmap(r.Left - pLoc.X + 1, BMP_BOO_BIG.Width);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                gBmp.RotateTransform(270);
                gBmp.TranslateTransform(-bmp.Height / 2, -bmp.Width / 2);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 1);
                this.CreateGraphics().DrawImage(bmp, pLoc.X, pLoc.Y);

                Thread.Sleep(WAIT);
                pLoc.X += 1;
            }
        }

        private void BooBottomScreen()
        {
            RECT r = new RECT();
            r.Bottom = 0;
            r.Left = 0;
            r.Right = this.Width;
            Point pLoc = new Point(this.m_r.Next(r.Left, r.Right - BMP_BOO_BIG.Width), r.Bottom + 1);
            while (pLoc.Y - r.Bottom < BMP_BOO_BIG.Height)
            {
                Bitmap bmp = new Bitmap(BMP_BOO_BIG.Width, pLoc.Y - r.Bottom);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(0, -BMP_BOO_BIG.Height + bmp.Height);
                gBmp.TranslateTransform(BMP_BOO_BIG.Width / 2, BMP_BOO_BIG.Height / 2);
                gBmp.RotateTransform(180);
                gBmp.TranslateTransform(-BMP_BOO_BIG.Width / 2, -BMP_BOO_BIG.Height / 2);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 0);
                Graphics gThis = this.CreateGraphics();
                gThis.DrawImage(bmp, pLoc.X, r.Bottom);

                Thread.Sleep(WAIT);
                pLoc.Y += 1;
            }

            Thread.Sleep(1500);

            while (pLoc.Y - r.Bottom >= 0)
            {
                Bitmap bmp = new Bitmap(BMP_BOO_BIG.Width, pLoc.Y - r.Bottom + 1);
                Graphics gBmp = Graphics.FromImage(bmp);
                gBmp.Clear(this.BackColor);
                gBmp.TranslateTransform(0, -BMP_BOO_BIG.Height + bmp.Height - 1);
                gBmp.TranslateTransform(BMP_BOO_BIG.Width / 2, BMP_BOO_BIG.Height / 2);
                gBmp.RotateTransform(180);
                gBmp.TranslateTransform(-BMP_BOO_BIG.Width / 2, -BMP_BOO_BIG.Height / 2);
                gBmp.DrawImage(BMP_BOO_BIG, 0, 0);
                Graphics gThis = this.CreateGraphics();
                gThis.DrawImage(bmp, pLoc.X, r.Bottom);

                Thread.Sleep(WAIT);
                pLoc.Y -= 1;
            }
        }
    }
}
