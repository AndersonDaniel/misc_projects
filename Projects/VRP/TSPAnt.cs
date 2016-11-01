using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VRPExp
{
    public class TSPAnt
    {
        public static List<Point> FindSolution(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails,
                                        double beta,
                                        double q0)
        {
            List<Point> lstRoute;
            List<Point> lstToVisit = (from kvp in dicPhTrails.Keys
                                      select kvp.Key).Distinct().ToList();
            Random rand = new Random();
            int nFirstLocIndex = rand.Next(lstToVisit.Count);
            double q;
            lstRoute = new List<Point>() { lstToVisit[nFirstLocIndex] };
            lstToVisit.RemoveAt(nFirstLocIndex);
            while (lstToVisit.Count > 0)
            {
                Point pNextLoc;
                q = rand.NextDouble();
                if (q <= q0)
                {
                    double dMaxPref = (from kvp in dicPhTrails.Keys
                                       where kvp.Key == lstRoute.Last() && lstToVisit.Contains(kvp.Value)
                                       select CalcTrailPereference(dicPhTrails, kvp.Key, kvp.Value, beta)).Max();
                    pNextLoc = (from kvp in dicPhTrails.Keys
                                where CalcTrailPereference(dicPhTrails, kvp.Key, kvp.Value, beta) == dMaxPref && lstToVisit.Contains(kvp.Value)
                                select kvp.Value).First();
                }
                else
                {
                    double dSumProbs =
                        (from kvp in dicPhTrails.Keys
                         where kvp.Key == lstRoute.Last() && lstToVisit.Contains(kvp.Value)
                         select CalcTrailPereference(dicPhTrails, kvp.Key, kvp.Value, beta)).Sum();
                    if (dSumProbs > 0)
                    {
                        Dictionary<Point, double> dicProbablities = new Dictionary<Point, double>();
                        foreach (KeyValuePair<double, Point> pair in
                            (from kvp in dicPhTrails.Keys
                             where lstToVisit.Contains(kvp.Value) && lstRoute.Last() == kvp.Key
                             orderby CalcTrailPereference(dicPhTrails, kvp.Key, kvp.Value, beta) descending
                             select new KeyValuePair<double, Point>(CalcTrailPereference(dicPhTrails, kvp.Key, kvp.Value, beta) /
                                                                    dSumProbs,
                                                                    kvp.Value)))
                        {
                            dicProbablities.Add(pair.Value, pair.Key);
                        }

                        double dSelectedLoc = rand.NextDouble();
                        double dCurrSum = dicProbablities.Values.First();
                        int nCurrIndex = 0;
                        while (dCurrSum < dSelectedLoc)
                        {
                            nCurrIndex++;
                            dCurrSum += dicProbablities.Values.ElementAt(nCurrIndex);
                        }

                        pNextLoc = dicProbablities.Keys.ElementAt(nCurrIndex);
                    }
                    else
                    {
                        pNextLoc = lstToVisit[rand.Next(lstToVisit.Count)];
                    }
                }

                lstRoute.Add(pNextLoc);
                lstToVisit.Remove(pNextLoc);
            }

            return (lstRoute);
        }

        private static double CalcTrailPereference(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails,
                                            Point pFrom,
                                            Point pTo,
                                            double beta)
        {
            return dicPhTrails[new KeyValuePair<Point, Point>(pFrom, pTo)] *
                   Math.Pow(Math.Sqrt(Math.Pow(pTo.X - pFrom.X, 2) + Math.Pow(pTo.Y - pFrom.Y, 2)), -beta);
        }
    }
}
