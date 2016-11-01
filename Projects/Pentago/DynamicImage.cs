using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pentago
{
    public class DynamicImage
    {
        private Image m_imgImage;
        private Size m_szImgSize;
        private Point m_pLocation;
        private Point m_pDestination;
        private int m_nID;
        private Board m_brdBoard;
        private static int ms_nImgCount = 0;

        public Image Image
        {
            get
            {
                return (this.m_imgImage);
            }
            set
            {
                this.m_imgImage = value;
            }
        }

        public Size Size
        {
            get
            {
                return (this.m_szImgSize);
            }
            set
            {
                this.m_szImgSize = value;
            }
        }

        public Point Location
        {
            get
            {
                return (this.m_pLocation);
            }
            set
            {
                this.m_pLocation = value;
            }
        }

        private int ID
        {
            get
            {
                return (this.m_nID);
            }
            set
            {
                this.m_nID = value;
            }
        }

        public Point Destination
        {
            get
            {
                return (this.m_pDestination);
            }
            set
            {
                this.m_pDestination = value;
            }
        }

        public Board Board
        {
            get
            {
                return (this.m_brdBoard);
            }
            set
            {
                this.m_brdBoard = value;
            }
        }

        public DynamicImage(Image imgImg, int nX, int nY, int nDX, int nDY, int nWidth, int nHeight)
        {
            this.ID = ++DynamicImage.ms_nImgCount;
            this.Image = (Image)imgImg.Clone();
            this.Size = new Size(nWidth, nHeight);
            this.Location = new Point(nX, nY);
            this.Destination = new Point(nDX, nDY);
            this.Board = new Board();
        }

        public override bool Equals(object obj)
        {
            return ((obj != null) && (obj is DynamicImage) && (this == (DynamicImage)obj));
        }

        public static bool operator==(DynamicImage imgFirst, DynamicImage imgSecond)
        {
            if (((object)imgFirst == null) || ((object)imgSecond == null))
            {
                if (((object)imgFirst == null) && ((object)imgSecond == null))
                {
                    return (true);
                }
                else
                {
                    return (false);
                }
            }
            else
            {
                return (imgFirst.ID == imgSecond.ID);
            }
        }

        public static bool operator !=(DynamicImage imgFirst, DynamicImage imgSecond)
        {
            return (!(imgFirst == imgSecond));
        }


    }
}
