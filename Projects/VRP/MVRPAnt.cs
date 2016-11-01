using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace VRPExp
{
    public static class MVRPAnt
    {
        public static Dictionary<Point, List<Point>> MatchLocsToCenter(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails,
                                                                double beta,
                                                                double q0)
        {
            List<Point> lstCenters = (from kvp in dicPhTrails.Keys
                                      select kvp.Key).Distinct().ToList();
            List<Point> lstLocs = (from kvp in dicPhTrails.Keys
                                   select kvp.Value).Distinct().ToList();
            Dictionary<Point, List<Point>> dicMatch = new Dictionary<Point, List<Point>>();
            int nMaximumLocsPerCenter = (int)(Math.Ceiling(lstLocs.Count / (double)lstCenters.Count));

            double q;
            Random rand = new Random();
            Point pSelectedCenter;

            bool bFirstTime = !dicPhTrails.Values.ToList().Exists(d => d > 0);

            while (lstLocs.Count > 0)
            {
                Point pCurrLoc = lstLocs[0];
                q = rand.NextDouble();
                if (q <= q0 && !bFirstTime)
                {
                    double dMaxPref = (from kvp in dicPhTrails.Keys
                                       where kvp.Value == pCurrLoc && lstCenters.Contains(kvp.Key)
                                       select CalcTrailPreference(dicPhTrails, kvp.Key, kvp.Value, beta)).Max();
                    pSelectedCenter = (from kvp in dicPhTrails.Keys
                                       where kvp.Value == pCurrLoc && lstCenters.Contains(kvp.Key)
                                       select kvp.Key).First();
                }
                else
                {
                    double dSumProbs =
                        (from kvp in dicPhTrails.Keys
                         where kvp.Value == pCurrLoc && lstCenters.Contains(kvp.Key)
                         select CalcTrailPreference(dicPhTrails, kvp.Key, kvp.Value, beta)).Sum();
                    if (dSumProbs > 0)
                    {
                        Dictionary<Point, double> dicProbablities = new Dictionary<Point, double>();
                        foreach (KeyValuePair<double, Point> pair in
                            (from kvp in dicPhTrails.Keys
                             where kvp.Value == pCurrLoc && lstCenters.Contains(kvp.Key)
                             orderby CalcTrailPreference(dicPhTrails, kvp.Key, kvp.Value, beta) descending
                             select new KeyValuePair<double, Point>(CalcTrailPreference(dicPhTrails, kvp.Key, kvp.Value, beta) /
                                                                    dSumProbs,
                                                                    kvp.Key)))
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

                        pSelectedCenter = dicProbablities.Keys.ElementAt(nCurrIndex);
                    }
                    else
                    {
                        pSelectedCenter = lstCenters[rand.Next(lstCenters.Count)];
                    }
                }

                if (!dicMatch.ContainsKey(pSelectedCenter))
                {
                    dicMatch.Add(pSelectedCenter, new List<Point>());
                }

                dicMatch[pSelectedCenter].Add(pCurrLoc);
                lstLocs.Remove(pCurrLoc);
                if (dicMatch[pSelectedCenter].Count > nMaximumLocsPerCenter)
                {
                    lstCenters.Remove(pSelectedCenter);
                }
            }

            return (dicMatch);
        }

        private static double CalcTrailPreference(Dictionary<KeyValuePair<Point, Point>, double> dicPhTrails,
                                                  Point pCenter,
                                                  Point pLoc,
                                                  double beta)
        {
            return dicPhTrails[new KeyValuePair<Point, Point>(pCenter, pLoc)] *
                   Math.Pow(Math.Sqrt(Math.Pow(pLoc.X - pCenter.X, 2) + Math.Pow(pLoc.Y - pCenter.Y, 2)), -beta);
        }
    }
}
