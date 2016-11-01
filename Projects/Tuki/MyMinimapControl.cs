using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TukiExp
{
    public partial class MyMinimapControl : UserControl
    {
        #region Consts

        private readonly Pen BORDER_PEN = new Pen(Color.Black, 1.5f);

        #endregion

        #region Data members

        private MyMapControl m_objObservedMap;
        private Bitmap m_bmpBaseMap;
        private bool m_bMouseDown = false;

        #endregion

        #region Properties

        public MyMapControl ObservedMap
        {
            get
            {
                return (this.m_objObservedMap);
            }
            set
            {
                this.m_objObservedMap = value;
                if (value != null && !this.DesignMode)
                {
                    this.m_objObservedMap.MiniMap = this;
                    this.m_bmpBaseMap = new Bitmap(this.ObservedMap.MapImage, this.Size);
                    Graphics.FromImage(this.m_bmpBaseMap).DrawRectangle(BORDER_PEN, 
                                                                        0, 0,
                                                                        this.Width - BORDER_PEN.Width / 2,
                                                                        this.Height - BORDER_PEN.Width / 2);
                    this.Render();
                }
            }
        }

        #endregion

        #region Ctor

        public MyMinimapControl()
        {
            InitializeComponent();
        }

        #endregion

        #region Public methods

        public void Render()
        {
            Bitmap bmpMinimap = new Bitmap((Image)this.m_bmpBaseMap.Clone());
            Graphics g = Graphics.FromImage(bmpMinimap);
            float fStartX = this.ObservedMap.LeftLocation * ((float)this.Width / this.ObservedMap.MapImage.Width);
            float fStartY = this.ObservedMap.TopLocation * ((float)this.Height / this.ObservedMap.MapImage.Height);
            float fEndX = (this.ObservedMap.LeftLocation + this.ObservedMap.Width / (float)this.ObservedMap.ZoomScale) *
                          ((float)this.Width / this.ObservedMap.MapImage.Width);
            fEndX = Math.Min(fEndX, this.Width - 1);
            float fEndY = (this.ObservedMap.TopLocation + this.ObservedMap.Height / (float)this.ObservedMap.ZoomScale) *
                          ((float)this.Height / this.ObservedMap.MapImage.Height);
            fEndY = Math.Min(fEndY, this.Height - 1);
            g.DrawRectangle(Pens.Red, fStartX, fStartY, fEndX - fStartX, fEndY - fStartY);
            this.BackgroundImage = bmpMinimap;
        }

        #endregion

        #region Event handling

        private void MyMinimapControl_MouseDown(object sender, MouseEventArgs e)
        {
            this.m_bMouseDown = true;
            this.MyMinimapControl_MouseMove(sender, e);
        }

        private void MyMinimapControl_MouseUp(object sender, MouseEventArgs e)
        {
            this.m_bMouseDown = false;
        }

        private void MyMinimapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.m_bMouseDown)
            {
                this.ObservedMap.LeftLocation =
                    (int)(e.X * (this.ObservedMap.MapImage.Width / (double)this.Width) -
                                 this.ObservedMap.Width / (2 * this.ObservedMap.ZoomScale));
                this.ObservedMap.TopLocation =
                    (int)(e.Y * (this.ObservedMap.MapImage.Height / (double)this.Height) -
                                 this.ObservedMap.Height / (2 * this.ObservedMap.ZoomScale));
                this.ObservedMap.Render();
            }
        }

        #endregion
    }
}
