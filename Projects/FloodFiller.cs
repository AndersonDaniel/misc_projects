using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Threading;

namespace FloodFiller
{
    class FloodPoint
    {
        private int m_nX;
        private int m_nY;
        private int m_nID;
        public FloodPoint(int x, int y, int id)
        {
            this.m_nX = x;
            this.m_nY = y;
            this.m_nID = id;
        }
        public int X
        {
            get
            {
                return (this.m_nX);
            }
        }
        public int Y
        {
            get
            {
                return (this.m_nY);
            }
        }
        public int ID
        {
            get
            {
                return (this.m_nID);
            }
        }
    }

    public class FloodFill
    {
        public delegate Point? Tracer(int nTimeElapsed);
        public delegate Point[] MultiTracer(int nTimeEaplsed);
        public delegate void FloodFinishEventHandler();
        public event FloodFinishEventHandler OnFinishFlood;
        private Bitmap m_bmpBitmap;
        private List<Queue> m_lqFillQueue;
        private Graphics m_gToDraw;
        private bool m_bIsFilling;
        private Color m_colColor;
        private int m_nWaitFrequency;
        private int m_nIDGenerator;
        private Dictionary<int, uint> m_dWaitIDS;
        private Mutex m_mtxQueueModification = new Mutex();
        private bool m_bIsFinished = false;

        public FloodFill(Bitmap bmpImage, Graphics gToDraw, Color col, int nWaitF)
        {
            this.m_bIsFilling = false;
            this.m_bmpBitmap = (Bitmap)bmpImage.Clone();
            this.m_gToDraw = gToDraw;
            this.m_lqFillQueue = new List<Queue>();
            this.m_colColor = col;
            this.m_nWaitFrequency = nWaitF;
            this.m_dWaitIDS = new Dictionary<int, uint>();
        }

        private void PaintPixel(FloodPoint p)
        {
            try
            {
                Color colToDraw;
                if (this.m_colColor.ToArgb() == Color.Transparent.ToArgb())
                {
                    colToDraw = this.m_bmpBitmap.GetPixel(p.X, p.Y);
                }
                else
                {
                    colToDraw = this.m_colColor;
                }
                this.m_bmpBitmap.SetPixel(p.X, p.Y, Color.White);
                this.m_gToDraw.FillRectangle(new SolidBrush(colToDraw),
                                             (float)p.X, (float)p.Y, 1, 1);
                if (!this.m_dWaitIDS.ContainsKey(p.ID))
                {
                    this.m_dWaitIDS.Add(p.ID, 0);
                }
                const double QU = 0.01;
                if ((this.m_dWaitIDS[p.ID] + 1) % (this.m_nWaitFrequency * this.m_lqFillQueue.Count) == 0)
                {
                    System.Threading.Thread.Sleep(1);
                }
                this.m_dWaitIDS[p.ID]++;
            }
            catch
            {

            }
        }

        private void StartFlood()
        {
            FloodPoint pTemp;
            while (this.m_lqFillQueue.Count > 0)
            {
                this.m_mtxQueueModification.WaitOne();
                foreach (Queue q in this.m_lqFillQueue)
                {
                    if ((q != null) && q.Count > 0)
                    {
                        pTemp = (FloodPoint)q.Dequeue();
                        if ((pTemp.X >= 0) &&
                            (pTemp.Y >= 0) &&
                            (pTemp.X < this.m_bmpBitmap.Width) &&
                            (pTemp.Y < this.m_bmpBitmap.Height) &&
                            (this.m_bmpBitmap.GetPixel(pTemp.X, pTemp.Y).ToArgb() !=
                             Color.White.ToArgb()))
                        {
                            this.PaintPixel(pTemp);
                            q.Enqueue(new FloodPoint(pTemp.X - 1, pTemp.Y, pTemp.ID));
                            q.Enqueue(new FloodPoint(pTemp.X + 1, pTemp.Y, pTemp.ID));
                            q.Enqueue(new FloodPoint(pTemp.X, pTemp.Y - 1, pTemp.ID));
                            q.Enqueue(new FloodPoint(pTemp.X, pTemp.Y + 1, pTemp.ID));
                            if (this.m_dWaitIDS[pTemp.ID] % 2 == 0)
                            {
                                q.Enqueue(new FloodPoint(pTemp.X - 1, pTemp.Y - 1, pTemp.ID));
                                q.Enqueue(new FloodPoint(pTemp.X + 1, pTemp.Y + 1, pTemp.ID));
                                q.Enqueue(new FloodPoint(pTemp.X + 1, pTemp.Y - 1, pTemp.ID));
                                q.Enqueue(new FloodPoint(pTemp.X - 1, pTemp.Y + 1, pTemp.ID));
                            }
                        }
                    }
                }
                this.m_mtxQueueModification.ReleaseMutex();
                this.m_lqFillQueue.Remove(null);
                this.m_lqFillQueue.ForEach(Q => 
                {
                    if (Q != null && Q.Count == 0)
                    {
                        this.m_lqFillQueue.Remove(Q);
                    }
                });
            }
            this.m_bIsFilling = false;
            if (this.OnFinishFlood != null)
            {
                this.m_bIsFinished = true;
                this.OnFinishFlood.Invoke();
            }
        }

        public void Flood(int x, int y)
        {
            if (!this.m_bIsFinished)
            {
                Queue q = new Queue();
                q.Enqueue(new FloodPoint(x, y, this.m_nIDGenerator++));
                this.m_mtxQueueModification.WaitOne();
                this.m_lqFillQueue.Add(q);
                this.m_mtxQueueModification.ReleaseMutex();
                if (!this.m_bIsFilling)
                {
                    this.m_nIDGenerator = 0;
                    this.m_bIsFilling = true;
                    Thread t = new Thread(new ThreadStart(this.StartFlood));
                    t.Start();
                }
            }
        }
        
        /// <summary>
        /// Traces the shape
        /// </summary>
        /// <param name="TraceFunc">Function that gets current (time) and returns the point to flood</param>
        /// <param name="nTimeUnitInMillsec">No. of milli-seconds each time unit is worth</param>
        public void StartTrace(Tracer TraceFunc, int nTimeUnitInMillsec)
        {
            int nCurrTime = 0;
            Point? p = TraceFunc(nCurrTime++);
            while (p != null)
            {
                this.Flood(p.Value.X, p.Value.Y);
                Thread.Sleep(nTimeUnitInMillsec);
                p = TraceFunc(nCurrTime++);
            }
        }

        /// <summary>
        /// Traces the shape
        /// </summary>
        /// <param name="TraceFunc">Function that gets current (time) and returns the point to flood</param>
        /// <param name="nTimeUnitInMillsec">No. of milli-seconds each time unit is worth</param>
        public void StartMultiTrace(MultiTracer TraceFunc, int nTimeUnitInMillsec)
        {
            int nCurrTime = 0;
            Point[] p = TraceFunc(nCurrTime++);
            while (p != null)
            {
                foreach (Point pFill in p)
                {
                    this.Flood(pFill.X, pFill.Y);
                }
                Thread.Sleep(nTimeUnitInMillsec);
                p = TraceFunc(nCurrTime++);
            }
        }
    }
}
