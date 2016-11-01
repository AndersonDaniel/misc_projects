using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace MirrorExp
{
    public partial class Form1 : Form
    {
        private int m_nLastWParam = -1;
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private int boo = 0;

        [DllImport("user32")]
        public static extern int GetWindowRect(IntPtr hwnd, ref RECT lpRect);

        [DllImport("user32")]
        public static extern int SetWindowPos(IntPtr hwnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("user32")]
        private static extern bool PrintWindow(HandleRef hWnd, IntPtr hDCBlt, int nFlags);

        [DllImport("user32")]
        public static extern int SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32")]
        public static extern int GetForegroundWindow();

        [DllImport("user32")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, IntPtr lParam);

        [DllImport("user32")]
        public static extern int PostMessage(IntPtr hwnd, Int32 wMsg, int wParam, int lParam);

        private IntPtr m_hWnd;
        private Graphics m_gGraphics;
        private float m_fAngle = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_hWnd =
                Process.GetProcessesByName("notepad")[0].MainWindowHandle;
            RECT r = new RECT();
            GetWindowRect(m_hWnd, ref r);
            //SetWindowPos(m_hWnd, IntPtr.Zero, -1000, -1000, (r.Right - r.Left) / 2, (r.Bottom - r.Top) / 2, 0);
            SetWindowPos(m_hWnd, IntPtr.Zero, -1000, -1000, 500, 500, 0);
            this.timer1.Enabled = true;
            m_gGraphics = this.CreateGraphics();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            IntPtr hCurrWnd = (IntPtr)(GetForegroundWindow());
            if (hCurrWnd == this.Handle)
            {
                SetForegroundWindow(m_hWnd);
            }
            RECT r = new RECT();
            GetWindowRect(m_hWnd, ref r);
            Bitmap bmp = new Bitmap(r.Right - r.Left, r.Bottom - r.Top);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hDC = g.GetHdc();
                //SetForegroundWindow(hWnd);
                PrintWindow(new HandleRef(g, m_hWnd), hDC, 0);
                //SetForegroundWindow(hCurrWnd);
                g.ReleaseHdc(hDC);
            }

            //m_gGraphics.Clear(this.BackColor);
            m_gGraphics.TranslateTransform((r.Right - r.Left) / 2 + 400, (r.Bottom - r.Top) / 2 + 300);
            //m_gGraphics.RotateTransform(m_fAngle);
            m_gGraphics.RotateTransform(56);
            m_gGraphics.TranslateTransform(-(r.Right - r.Left) / 2, -(r.Bottom - r.Top) / 2);
            m_gGraphics.DrawImage(bmp, 0, 0);
            m_gGraphics.ResetTransform();
            m_fAngle += 1f;
        }

        //protected override void WndProc(ref Message m)
        //{
        //    base.WndProc(ref m);
        //    if (m.Msg == 0x100)
        //    {
        //        int b = 4;
        //    }
        //    else if (m.Msg == 0x101)
        //    {
        //        int a = 3;
        //    }
        //}

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
                //this.timer1.Enabled = false;
                //IntPtr hWnd =
                //    Process.GetProcessesByName("notepad")[0].MainWindowHandle;
                //IntPtr hCurrWnd = (IntPtr)(GetForegroundWindow());
                //SetForegroundWindow(hWnd);
                ////Thread.Sleep(1000);
                ////SendKeys.Send("R");
                //SendKeys.SendWait(KeyToString(e));
                //////Thread.Sleep(400);
                //////SendKeys.Send("S");
                ////Thread.Sleep(1000);
                ////PostMessage(hWnd, 0x100, (int)Keys.R, 0);
                ////PostMessage(hWnd, 0x100, (int)e.KeyCode, 0);
                ////SendMessage(hWnd, 0x101, (int)e.KeyCode, (IntPtr)1);
                //SetForegroundWindow(hCurrWnd);
                //this.timer1.Enabled = true;
        }

        private void Form1_Enter(object sender, EventArgs e)
        {
            MessageBox.Show("Enter");
            SetForegroundWindow(m_hWnd);
        }

        //private string KeyToString(KeyEventArgs e)
        //{
        //    //int k = (int)e.KeyCode;
        //    //if (k >= (int)Keys.A && k <= (int)Keys.Z)
        //    //{
        //    //    return ((char)('A' + (int)(k - Keys.A))).ToString();
        //    //}
        //    //else if (k == (int)Keys.Space)
        //    //{
        //    //    return " ";
        //    //}
        //    //else if (k == (int)Keys.Enter)
        //    //{
        //    //    return "{ENTER}";
        //    //}
        //    return "a";

        //    return "";
        //}
    }
}
