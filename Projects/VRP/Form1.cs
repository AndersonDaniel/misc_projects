using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VRPExp
{
    public partial class Form1 : Form
    {
        private List<Point> lstPoints = new List<Point>();
        private List<Point> lstCenters = new List<Point>();

        public Form1()
        {
            InitializeComponent();
            this.lstPoints = new List<Point>();
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.lstPoints.Add(new Point(e.X, e.Y));
                //this.CreateGraphics().FillEllipse(Brushes.Blue, e.X - 5, e.Y - 5, 10, 10);
                this.DrawLocation(this.CreateGraphics(), new Point(e.X, e.Y));
            }
            else
            {
                this.lstCenters.Add(new Point(e.X, e.Y));
                //this.CreateGraphics().FillRectangle(Brushes.Red, e.X - 5, e.Y - 5, 10, 10);
                this.DrawCenter(this.CreateGraphics(), new Point(e.X, e.Y));
            }

            if (this.label2.Visible)
            {
                this.label2.Visible = false;
            }
        }

        private List<Point> SolveTSP(List<Point> lstLocs)
        {
            TSPwACO.OnFinishedPercentageUpdated += (d, ph) =>
                { 
                    //this.label1.Text = (d * 100).ToString() + "%";
                    this.ShowPhTrails(ph);
                    Application.DoEvents();
                };
            List<Point> lstResult = 
                TSPwACO.SolveTSP(lstLocs,
                                 10,
                                 0.1,
                                 2,
                                 0.9,
                                 lstLocs.Count / GetPathTotalDistance(NearestNeighbour(lstLocs)));
            this.CreateGraphics().Clear(this.BackColor);
            lstResult = TwoOptimization(lstResult);
            lstResult.Add(lstResult[0]);
            return (lstResult);
        }

        

        private static double GetPointsDist(Point p1, Point p2)
        {
            return (Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)));
        }

        public static double GetPathTotalDistance(List<Point> lstLocs)
        {
            double dTotDist = GetPointsDist(lstLocs[0], lstLocs[lstLocs.Count - 1]);

            for (int nIndex = 0; nIndex < lstLocs.Count - 1; nIndex++)
            {
                dTotDist += GetPointsDist(lstLocs[nIndex], lstLocs[nIndex + 1]);
            }

            return (dTotDist);
        }

        public static List<Point> TwoOptSwap(List<Point> lstPoints, int from, int to)
        {
            Point[] arrPoints = new Point[lstPoints.Count];

            lstPoints.CopyTo(0, arrPoints, 0, from);
            for (int nIndex = to; nIndex >= from; nIndex--)
            {
                arrPoints[from + to - nIndex] = lstPoints[nIndex];
            }
            lstPoints.CopyTo(to + 1, arrPoints, to + 1, lstPoints.Count - to - 1);

            return (arrPoints.ToList());
        }

        public static List<Point> TwoOptimization(List<Point> lstPoints)
        {
            List<Point> lstTemp;

            for (int nFrom = 0; nFrom < lstPoints.Count - 1; nFrom++)
            {
                for (int nTo = nFrom + 1; nTo < lstPoints.Count; nTo++)
                {
                    lstTemp = TwoOptSwap(lstPoints, nFrom, nTo);
                    if (GetPathTotalDistance(lstPoints) > GetPathTotalDistance(lstTemp))
                    {
                        return (TwoOptimization(lstTemp));
                    }
                }
            }

            return (lstPoints);
        }

        public static List<Point> NearestNeighbour(List<Point> lstPoints)
        {
            List<Point> lstRoute = new List<Point>();
            Point pTemp;
            lstPoints = new List<Point>(lstPoints);
            lstRoute.Add(lstPoints[0]);
            lstPoints.RemoveAt(0);
            while (lstPoints.Count > 0)
            {
                double dMinDist = (from p in lstPoints
                                   select GetPointsDist(p, lstRoute.Last())).Min();
                pTemp = (from p in lstPoints
                         where GetPointsDist(p, lstRoute.Last()) == dMinDist
                         select p).Single();
                lstRoute.Add(pTemp);
                lstPoints.Remove(pTemp);
            }

            return (lstRoute);
        }

        private void ShowSolution(List<Point> lstLocs)
        {
            Graphics g = this.CreateGraphics();

            for (int nIndex = 0; nIndex < lstLocs.Count - 1; nIndex++)
            {
                //g.FillEllipse(Brushes.Blue, lstLocs[nIndex].X - 5, lstLocs[nIndex].Y - 5, 10, 10);
                this.DrawLocation(g, lstLocs[nIndex]);
            }

            for (int nIndex = 0; nIndex < lstLocs.Count - 1; nIndex++)
            {
                g.DrawLine(new Pen(Color.Gold, 2.5f), lstLocs[nIndex], lstLocs[nIndex + 1]);
            }
        }

        private void ShowVRPSolution(List<List<Point>> lstSolution)
        {
            Graphics g = this.CreateGraphics();

            foreach (Point p in this.lstCenters)
            {
                //g.FillRectangle(Brushes.Red, p.X - 5, p.Y - 5, 10, 10);
                this.DrawCenter(g, p);
            }

            foreach (Point p in this.lstPoints)
            {
                //g.FillEllipse(Brushes.Blue, p.X - 5, p.Y - 5, 10, 10);
                this.DrawLocation(g, p);
            }

            int nSectIndex = 0;

            Random rand = new Random();

            foreach (List<Point> lstSubSol in lstSolution)
            {
                Color c = Color.FromArgb(rand.Next(20, 220), rand.Next(20, 220), rand.Next(20, 220));
                for (int nIndex = 0; nIndex < lstSubSol.Count - 1; nIndex++)
                {
                    //g.DrawLine(new Pen(Color.FromArgb(nSectIndex * 50, nSectIndex * 50, nSectIndex * 50), 2f), lstSubSol[nIndex], lstSubSol[nIndex + 1]);
                    g.DrawLine(new Pen(c, 1.7f), lstSubSol[nIndex], lstSubSol[nIndex + 1]);
                }
                nSectIndex++;
            }
        }

        private void ShowPhTrails(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails)
        {
            Graphics g = this.CreateGraphics();
            foreach (Point p in (from kvp in dicPhTrails.Keys select kvp.Key).Distinct())
            {
                //g.FillEllipse(Brushes.Blue, p.X - 5, p.Y - 5, 10, 10);
                this.DrawLocation(g, p);
            }

            List<double> lstValues = dicPhTrails.Values.Distinct().ToList();
            lstValues.Sort();

            foreach (KeyValuePair<Point, Point> arc in dicPhTrails.Keys)
            {
                if (lstValues.IndexOf(dicPhTrails[arc]) > 2)
                {
                    g.DrawLine(new Pen(Color.Gold, (float)lstValues.IndexOf(dicPhTrails[arc])), arc.Key, arc.Value);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //if (this.lstCenters.Count == 0 || this.lstPoints.Count == 0)
            //{
            //    MessageBox.Show("יש להוסיף לפחות מרכז אחד ולפחות לקוח אחד");
            //}
            //else
            {
                this.CreateGraphics().Clear(this.BackColor);
                //this.ShowSolution(this.SolveTSP(this.lstPoints));
                //this.ShowVRPSolution(SingleVRP.SolveSingleVRP(this.lstPoints, this.lstCenters[0], (int)this.numericUpDown1.Value));
                Dictionary<Point, int> dicCenterVehicles = new Dictionary<Point, int>();
                foreach (Point pCenter in lstCenters)
                {
                    dicCenterVehicles.Add(pCenter, (int)this.numericUpDown1.Value);
                }
                foreach (List<List<Point>> lstBla in MultiVRP.
                                                     SolveMultiVRP(this.lstPoints,
                                                                   dicCenterVehicles,
                                                                   10,
                                                                   0.1,
                                                                   2,
                                                                   0.9,
                                                                   0))
                {
                    this.ShowVRPSolution(lstBla);
                }
            }
        }

        private void DrawLocation(Graphics g, Point pWhere)
        {
            g.FillEllipse(Brushes.Blue, pWhere.X - 3, pWhere.Y - 3, 6, 6);
        }

        private void DrawCenter(Graphics g, Point pWhere)
        {
            g.FillRectangle(Brushes.Red, pWhere.X - 6, pWhere.Y - 6, 12, 12);
        }

        private static double GetMultiVRPTao0(List<Point> lstCenters, List<Point> lstLocs)
        {
            double dBestDist = 0, dWorstDist = 0;
            foreach (Point pLoc in lstLocs)
            {
                dBestDist += (from pCenter in lstCenters
                              select GetPointsDist(pLoc, pCenter)).Min();
                dWorstDist  += (from pCenter in lstCenters
                              select GetPointsDist(pLoc, pCenter)).Max();
            }

            return 1 * lstCenters.Count / (dBestDist + dWorstDist);
        }
    }
}
