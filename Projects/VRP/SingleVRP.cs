using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace VRPExp
{
    public static class SingleVRP
    {
        public static List<List<Point>> SolveSingleVRP(List<Point> lstLocs, Point pCenter, int vehicles)
        {
            lstLocs = new List<Point>(lstLocs);
            List<List<Point>> lstSolution = new List<List<Point>>();
            List<List<Point>> lstSections = new List<List<Point>>();
            List<Point> lstCurrLocs = new List<Point>();
            lstLocs.Sort(new Comparison<Point>((p1, p2) =>
                {
                    double dAng1 = Math.Atan2(p1.Y - pCenter.Y, p1.X - pCenter.X);
                    double dAng2 = Math.Atan2(p2.Y - pCenter.Y, p2.X - pCenter.X);
                    return (dAng1.CompareTo(dAng2));
                }));

            if (vehicles > lstLocs.Count)
            {
                vehicles = lstLocs.Count;
            }

            int nLocsPerVehicle = lstLocs.Count / vehicles;
            int nExtraVehicleCount = lstLocs.Count % vehicles;
            int nVehicleIndex = 0;
            int nCurrLocIndex = 0;
            int nTempCurrLocIndex;

            while (nCurrLocIndex < lstLocs.Count)
            {
                lstCurrLocs = new List<Point>();
                lstCurrLocs.Add(pCenter);
                nTempCurrLocIndex = nCurrLocIndex;
                for (int nLoc = 0; nLoc < Math.Min(nLocsPerVehicle + (nExtraVehicleCount > nVehicleIndex ? 1 : 0),
                                                   lstLocs.Count - nTempCurrLocIndex); nLoc++)
                {
                    lstCurrLocs.Add(lstLocs[nCurrLocIndex++]);
                }

                lstSections.Add(lstCurrLocs);
                nVehicleIndex++;
            }

            Parallel.ForEach(lstSections, sect =>
                {
                    lstSolution.Add(Form1.TwoOptimization(TSPwACO.SolveTSP(sect,
                                                          10,
                                                          0.1,
                                                          2,
                                                          0.9,
                                                          sect.Count / Form1.GetPathTotalDistance(Form1.NearestNeighbour(sect)))));
                    lstSolution.Last().Add(lstSolution.Last()[0]);
                });

            return (lstSolution);
        }
    }
}
