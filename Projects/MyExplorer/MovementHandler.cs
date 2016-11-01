using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MyExplorer
{
    class MovementHandler
    {
        private double m_nMovementAngle;
        private int m_nMovementSpeed;
        private int m_nPing;
        private int m_nAvgPing;
        private Random m_rRand;
        private double m_dDeltaAngle;
        private int m_nChangeTimes;
        private PointF m_pLocation;
        private static int m_nSeed = 0;
        private bool m_bImposed = false;
        private bool m_bCollides;
        public MovementHandler(int nAngle, int nSpeed, int nAvgPing, PointF pFirstLoc)
        {
            this.m_bCollides = true;
            this.m_nMovementAngle = nAngle;
            this.m_nMovementSpeed = nSpeed;
            this.m_nPing = 0;
            this.m_rRand = new Random(MovementHandler.m_nSeed++);
            this.m_nAvgPing = nAvgPing;
            this.m_pLocation = pFirstLoc;
        }

        public int X
        {
            get
            {
                return ((int)this.m_pLocation.X);
            }
        }

        public int Y
        {
            get
            {
                return ((int)this.m_pLocation.Y);
            }
        }

        public bool Collides
        {
            get
            {
                return (this.m_bCollides);
            }
            set
            {
                this.m_bCollides = value;
            }
        }

        private double ToRad(double nAngle)
        {
            return (nAngle * Math.PI / 180);
        }

        public void Ping()
        {
            if (this.m_nChangeTimes <= 0)
            {
                this.m_bImposed = false;
                this.m_nPing++;
                if (this.m_nPing > 0)
                {
                    if (this.m_rRand.Next(this.m_nPing) < this.m_nAvgPing)
                    {
                        this.m_nPing = 0;
                        int nAngleChange = this.m_rRand.Next(90, 271);
                        this.m_dDeltaAngle = Math.Max(0.5, this.m_rRand.NextDouble()) *
                                             6 * (this.m_rRand.Next(-1, 1) * 2 + 1);
                        this.m_nChangeTimes = (int)(nAngleChange / Math.Abs(this.m_dDeltaAngle));
                    }
                }
            }
            else
            {
                this.m_nChangeTimes--;
                this.m_nMovementAngle += this.m_dDeltaAngle;
            }

            this.m_pLocation.X += (float)(this.Speed * Math.Cos(ToRad(this.Angle)));
            this.m_pLocation.Y += (float)(this.Speed * Math.Sin(ToRad(this.Angle)));
        }

        public void ImposeTurn(double dNewAngle)
        {
            if (!this.m_bImposed)
            {
                this.m_bImposed = true;
                this.m_nPing = -20;
                //this.m_dDeltaAngle = Math.Max(0.5, this.m_rRand.NextDouble()) *
                //                     6 * (dNewAngle < this.m_nMovementAngle ? -1 : 1);
                this.m_dDeltaAngle = 22 * (dNewAngle < this.m_nMovementAngle ? -1 : 1);
                this.m_nChangeTimes = (int)(Math.Abs(dNewAngle - this.m_nMovementAngle) /
                                            Math.Abs(this.m_dDeltaAngle));
            }
        }

        public double Angle { get { return this.m_nMovementAngle; } }
        public int Speed { get { return this.m_nMovementSpeed; } }
    }
}
