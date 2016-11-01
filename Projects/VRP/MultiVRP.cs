using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading.Tasks;

namespace VRPExp
{
    public static class MultiVRP
    {
        public static List<List<List<Point>>> SolveMultiVRP(List<Point> lstLocs,
                                                            Dictionary<Point, int> dicCentersVehicles,
                                                            int ants,
                                                            double alpha,
                                                            double beta,
                                                            double q0,
                                                            double tao0)
        {
            List<List<List<Point>>> lstSolution = new List<List<List<Point>>>();
            Dictionary<Point, List<Point>> dicLocsToCenters =
                MultiVRP.DivideLocationsToCenters(lstLocs,
                                                  dicCentersVehicles,
                                                  ants,
                                                  alpha,
                                                  beta,
                                                  q0,
                                                  tao0);

            Parallel.ForEach(dicLocsToCenters.Keys, pCenter =>
                {
                    lstSolution.Add(SingleVRP.SolveSingleVRP(dicLocsToCenters[pCenter],
                                                             pCenter,
                                                             dicCentersVehicles[pCenter]));
                });

            return (lstSolution);
        }

        private static Dictionary<Point, List<Point>> DivideLocationsToCenters(List<Point> lstLocs,
                                                                               Dictionary<Point, int> dicCentersVehicles,
                                                                               int ants,
                                                                               double alpha,
                                                                               double beta,
                                                                               double q0,
                                                                               double tao0)
        {
            const int ITERATIONS = 5;

            Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails =
                MultiVRP.InitPheromoneTrails(dicCentersVehicles.Keys.ToList(), lstLocs);

            List<Dictionary<Point, List<Point>>> lstSolutions =
                new List<Dictionary<Point, List<Point>>>();

            Dictionary<Point, List<Point>> dicReallyBestSolution = null;

            for (int nIter = 0; nIter < ITERATIONS; nIter++)
            {
                lstSolutions.Clear();
                for (int nAnt = 0; nAnt < ants; nAnt++)
                {
                    lstSolutions.Add(MVRPAnt.MatchLocsToCenter(dicPhTrails, beta, q0));
                }

                for (int nInd = 0; nInd < lstSolutions.Count; nInd++)
                {
                    TrailUpdating(dicPhTrails, lstSolutions[nInd], alpha, tao0);
                }

                double dBestSolLength =
                    (from sol in lstSolutions
                     select GetTotalDist(sol)).Min();
                Dictionary<Point, List<Point>> dicBestSol =
                    (from sol in lstSolutions
                     where GetTotalDist(sol) == dBestSolLength
                     select sol).First();

                TrailUpdating(dicPhTrails, dicBestSol, alpha, 1 / dBestSolLength);

                if (dicReallyBestSolution == null || dBestSolLength < GetTotalDist(dicReallyBestSolution))
                {
                    dicReallyBestSolution = dicBestSol;
                }
            }

            return (dicReallyBestSolution);
        }

        private static Dictionary<KeyValuePair<Point, Point>, double> InitPheromoneTrails(List<Point> lstCenters,
                                                                                          List<Point> lstLocs)
        {
            Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails =
                new Dictionary<KeyValuePair<Point, Point>, double>();

            foreach (Point pCenter in lstCenters)
            {
                foreach (Point pLoc in lstLocs)
                {
                    dicPhTrails.Add(new KeyValuePair<Point, Point>(pCenter, pLoc), 0);
                }
            }

            return (dicPhTrails);
        }

        private static void TrailUpdating(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails,
                                          Dictionary<Point, List<Point>> dicCenterMapping,
                                          double alpha,
                                          double taoVal)
        {
            double dCurrPheromoneVal;

            foreach (Point pCenter in dicCenterMapping.Keys)
            {
                foreach (Point pLoc in dicCenterMapping[pCenter])
                {
                    dCurrPheromoneVal = dicPhTrails[new KeyValuePair<Point, Point>(pCenter, pLoc)];
                    dCurrPheromoneVal = (1 - alpha) * dCurrPheromoneVal + alpha * taoVal;
                    dicPhTrails[new KeyValuePair<Point, Point>(pCenter, pLoc)] = dCurrPheromoneVal;
                }
            }
        }

        private static double GetTotalDist(Dictionary<Point, List<Point>> dicMatches)
        {
            double dTotalDist = 0;

            foreach (Point pCenter in dicMatches.Keys)
            {
                foreach (Point pLoc in dicMatches[pCenter])
                {
                    dTotalDist += Math.Sqrt(Math.Pow(pCenter.X - pLoc.X, 2) +
                                            Math.Pow(pCenter.Y - pLoc.Y, 2));
                }
            }

            return (dTotalDist);
        }
    }
}
