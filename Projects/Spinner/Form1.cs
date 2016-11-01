using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using API;
using System.Diagnostics;
using System.Threading;

namespace SpinnerExp
{
    public partial class Form1 : Form
    {
        private IntPtr m_hWnd;
        public Form1()
        {
            InitializeComponent();
            User.RegisterHotKey(this.Handle, 0, 1, (int)Keys.S);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(this.DoFun)).Start();
        }

        private void DoFun()
        {
            this.SpinWindow((IntPtr)User.GetForegroundWindow());
        }

        private void SpinWindow(IntPtr hWnd)
        {
            RECT r = new RECT();
            User.GetWindowRect(hWnd, ref r);
            Bitmap bmp = new Bitmap(r.Right - r.Left, r.Bottom - r.Top);
            Point pWhere = new Point(r.Left, r.Top);
            Graphics g = Graphics.FromImage(bmp);
            //IntPtr hdc = g.GetHdc();
            //bool bGood = User.PrintWindow(hWnd, hdc, 0);
            g.CopyFromScreen(r.Left, r.Top, 0, 0, new Size(bmp.Width, bmp.Height));
            //g.ReleaseHdc(hdc);
            //this.BackgroundImage = bmp;
            double nSize = Math.Sqrt(bmp.Width * bmp.Width + bmp.Height * bmp.Height);
            Bitmap bmpNew = new Bitmap((int)nSize, (int)nSize);
            Graphics gNew = Graphics.FromImage(bmpNew);
            gNew.TranslateTransform(bmpNew.Width / 2, bmpNew.Height / 2);
            Graphics frmG = this.CreateGraphics();
            frmG.TranslateTransform(pWhere.X + bmp.Width / 2, pWhere.Y + bmp.Height / 2);
            Point pToDraw = new Point(-bmpNew.Width / 2, -bmpNew.Height / 2);
            //Point pToDraw = new Point(0, 0);
            Point pToDrawBmp = new Point(-bmp.Width / 2, -bmp.Height / 2);
            bool bHidden = false;
            for (int nAngle = 0; nAngle <= 360; nAngle++)
            {
                gNew.Clear(this.BackColor);
                gNew.DrawImage(bmp, pToDrawBmp);
                frmG.DrawImage(bmpNew, pToDraw);
                if (!bHidden)
                {
                    bHidden = true;
                    User.ShowWindow(hWnd, 0);
                    Thread.Sleep(100);
                }
                //frmG.FillRectangle(b, rect);
                gNew.RotateTransform(1);
                //frmG.DrawImage(bmp, rect.Location);
            }

            frmG.Clear(this.BackColor);
            User.ShowWindow(hWnd, 1);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == User.WM_HOTKEY)
            {
                new Thread(new ThreadStart(this.DoFun)).Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            User.ShowWindow(this.m_hWnd, 0);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            User.ShowWindow(this.m_hWnd, 1);
        }
    }
}
