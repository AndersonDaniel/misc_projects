using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VRPExp
{
    public static class TSPwACO
    {
        private static Action<double, Dictionary<KeyValuePair<Point, Point>, double>> s_actFinishedPercentageHandler;
        public static event Action<double, Dictionary<KeyValuePair<Point, Point>, double>> OnFinishedPercentageUpdated
        {
            add
            {
                TSPwACO.s_actFinishedPercentageHandler += value;
            }
            remove
            {
                TSPwACO.s_actFinishedPercentageHandler -= value;
            }
        }

        public static List<Point> SolveTSP(List<Point> lstLocs,
                                           int ants,
                                           double alpha,
                                           double beta,
                                           double q0,
                                           double tao0)
        {
            Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails =
                InitPheromoneTrails(lstLocs);

            const int ITERATIONS = 5;

            List<List<Point>> lstSolutions = new List<List<Point>>();
            List<Point> lstReallyBestSolution = null;

            for (int nIter = 0; nIter < ITERATIONS; nIter++)
            {
                lstSolutions.Clear();
                for (int nAnt = 0; nAnt < ants; nAnt++)
                {
                    lstSolutions.Add(TSPAnt.FindSolution(dicPhTrails, beta, q0));
                    if (TSPwACO.s_actFinishedPercentageHandler != null)
                    {
                        TSPwACO.s_actFinishedPercentageHandler.Invoke((nIter * ants + nAnt + 1) / (double)(ITERATIONS * ants),
                                                                      dicPhTrails);
                    }
                }
                
                // Local trail updating
                foreach (List<Point> lstSol in lstSolutions)
                {
                    TSPwACO.TrailUpdating(dicPhTrails, lstSol, alpha, tao0);
                }

                // Get best solution
                double dBestSolLength =
                    (from sol in lstSolutions
                     select Form1.GetPathTotalDistance(sol)).Min();
                List<Point> lstBestSolution =
                    (from sol in lstSolutions
                     where Form1.GetPathTotalDistance(sol) == dBestSolLength
                     select sol).First();

                // Global updating
                TSPwACO.TrailUpdating(dicPhTrails, lstBestSolution, alpha, 1 / dBestSolLength);

                // Save best solution
                if (lstReallyBestSolution == null || dBestSolLength < Form1.GetPathTotalDistance(lstReallyBestSolution))
                {
                    lstReallyBestSolution = lstBestSolution;
                }
            }

            return (lstReallyBestSolution);
        }

        private static Dictionary<KeyValuePair<Point, Point>, double> InitPheromoneTrails(List<Point> lstLocs)
        {
            Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails = 
                        new Dictionary<KeyValuePair<Point, Point>, double>();

            foreach (Point pFrom in lstLocs)
            {
                foreach (Point pTo in lstLocs)
                {
                    if (pFrom != pTo)
                    {
                        dicPhTrails.Add(new KeyValuePair<Point, Point>(pFrom, pTo), 0);
                    }
                }
            }

            return (dicPhTrails);
        }

        private static void TrailUpdating(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails,
                                               List<Point> lstRoute,
                                               double alpha,
                                               double taoVal)
        {
            double dCurrPheromoneVal;

            for (int nIndex = 0; nIndex < lstRoute.Count - 1; nIndex++)
            {
                dCurrPheromoneVal =
                    dicPhTrails[new KeyValuePair<Point,Point>(lstRoute[nIndex], lstRoute[nIndex + 1])];
                dCurrPheromoneVal = (1 - alpha) * dCurrPheromoneVal + alpha * taoVal;
                dicPhTrails[new KeyValuePair<Point, Point>(lstRoute[nIndex], lstRoute[nIndex + 1])] =
                    dCurrPheromoneVal;
            }
        }

        
    }
}
