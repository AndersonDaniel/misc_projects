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
    public partial class MyMapControl : UserControl
    {
        #region Consts

        private Pen ZOOM_IN = new Pen(Color.Red, 2);
        private Pen ZOOM_OUT = new Pen(Color.Blue, 2);
        private double MIN_ZOOM = 1;
        private double MAX_ZOOM = 15;

        #endregion

        #region Data members

        private bool m_bMouseDown = false;
        private Point m_pPrevMouseLoc;
        private Point m_pInitialMouseLoc;
        private Point m_pTopLeftLoc;
        private MapAction m_objCurrAction;
        private Bitmap m_imgBaseMap;
        private Image m_imgMapImage;
        private double m_dZoomScale;
        private MyMinimapControl m_objMinimap;

        #endregion

        #region Properties

        public Image MapImage
        {
            get
            {
                return (this.m_imgMapImage);
            }
            set
            {
                this.m_imgMapImage = value;
                this.MIN_ZOOM = Math.Max((double)this.Height / value.Height,
                                         (double)this.Width / value.Width);
            }
        }
        public Point TopLeftLocation
        {
            get
            {
                return (this.m_pTopLeftLoc);
            }
            set
            {
                this.m_pTopLeftLoc = value;
            }
        }

        public int LeftLocation
        {
            get
            {
                return (this.TopLeftLocation.X);
            }
            set
            {
                if ((this.MapImage == null) ||
                    ((value >= 0) &&
                     (value + (int)(this.Width / this.ZoomScale) < this.MapImage.Width)))
                {
                    this.m_pTopLeftLoc.X = value;
                }
                else if (value < 0)
                {
                    this.m_pTopLeftLoc.X = 0;
                }
                else
                {
                    this.m_pTopLeftLoc.X = this.MapImage.Width - (int)(this.Width / this.ZoomScale);
                }
            }
        }

        public int TopLocation
        {
            get
            {
                return (this.TopLeftLocation.Y);
            }
            set
            {
                if ((this.MapImage == null) ||
                    ((value >= 0) &&
                     (value + (int)(this.Height / this.ZoomScale) < this.MapImage.Height)))
                {
                    this.m_pTopLeftLoc.Y = value;
                }
                else if (value < 0)
                {
                    this.m_pTopLeftLoc.Y = 0;
                }
                else
                {
                    this.m_pTopLeftLoc.Y = this.MapImage.Height - (int)(this.Height / this.ZoomScale);
                }
            }
        }

        // Bigger means closer
        public double ZoomScale
        {
            get
            {
                return (this.m_dZoomScale);
            }
            set
            {
                if (value >= this.MIN_ZOOM && value <= this.MAX_ZOOM)
                {
                    this.m_dZoomScale = value;
                }
                else if (value < this.MIN_ZOOM)
                {
                    this.m_dZoomScale = MIN_ZOOM;
                }
                else
                {
                    this.m_dZoomScale = MAX_ZOOM;
                }
            }
        }

        public MapAction CurrentAction
        {
            get
            {
                return (this.m_objCurrAction);
            }
            set
            {
                this.m_objCurrAction = value;

                switch (value)
                {
                    case MapAction.None:
                    {
                        this.Cursor = Cursors.Arrow;
                        break;
                    }
                    case MapAction.Drag:
                    {
                        this.Cursor = Cursors.SizeAll;
                        break;
                    }
                    case MapAction.ZoomIn:
                    {
                        this.Cursor = Cursors.Cross;
                        break;
                    }
                    case MapAction.ZoomOut:
                    {
                        this.Cursor = Cursors.Cross;
                        break;
                    }
                    default:
                    {
                        this.Cursor = Cursors.Arrow;
                        break;
                    }
                }
            }
        }

        public MyMinimapControl MiniMap
        {
            get
            {
                return (this.m_objMinimap);
            }
            set
            {
                this.m_objMinimap = value;
            }
        }

        #endregion

        #region Ctor

        public MyMapControl()
        {
            InitializeComponent();

            this.pbMapPic.Dock = DockStyle.Fill;
            this.TopLeftLocation = new Point(0, 0);
        }

        #endregion

        #region Public methods

        #region Render

        public void Render()
        {
            this.Render(null, true);
        }

        public void Render(Action<Graphics> objExtraDraw)
        {
            this.Render(objExtraDraw, true);
        }

        private void Render(bool bRedrawBase)
        {
            this.Render(null, bRedrawBase);
        }

        public void Render(Action<Graphics> objExtraDraw, bool bRedrawBase)
        {
            if (this.MapImage != null)
            {
                Bitmap bmpMap;

                if (bRedrawBase)
                {
                    RectangleF rCropRect = new RectangleF((float)this.TopLeftLocation.X,
                                                          (float)this.TopLeftLocation.Y,
                                                          (float)Math.Floor((this.Width / this.ZoomScale)),
                                                          (float)Math.Floor((this.Height / this.ZoomScale)));

                    bmpMap = new Bitmap(((Bitmap)this.MapImage).Clone(rCropRect, this.MapImage.PixelFormat),
                                        this.pbMapPic.Size);
                    this.m_imgBaseMap = bmpMap;

                    if (this.MiniMap != null)
                    {
                        this.MiniMap.Render();
                    }
                }

                bmpMap = (Bitmap)(this.m_imgBaseMap.Clone());

                if (objExtraDraw != null)
                {
                    objExtraDraw(Graphics.FromImage(bmpMap));
                }

                this.pbMapPic.Image = bmpMap;
            }
        }

        #endregion

        #endregion

        #region Private methods

        private void ActionDrag(int nMouseX, int nMouseY)
        {
            this.LeftLocation += (int)((this.m_pPrevMouseLoc.X - nMouseX) / this.ZoomScale);
            this.TopLocation += (int)((this.m_pPrevMouseLoc.Y - nMouseY) / this.ZoomScale);
            this.Render();
            this.m_pPrevMouseLoc.X = nMouseX;
            this.m_pPrevMouseLoc.Y = nMouseY;
        }

        private void ActionZoomOutMove(int nMouseX, int nMouseY)
        {
            this.Render(g =>
            {
                int nStartX = (int)Math.Min(this.m_pInitialMouseLoc.X, nMouseX);
                int nStartY = (int)Math.Min(this.m_pInitialMouseLoc.Y, nMouseY);
                int nEndX = (int)Math.Max(this.m_pInitialMouseLoc.X, nMouseX);
                int nEndY = (int)Math.Max(this.m_pInitialMouseLoc.Y, nMouseY);
                g.DrawRectangle(ZOOM_OUT,
                                nStartX,
                                nStartY,
                                nEndX - nStartX,
                                nEndY - nStartY);
                g.FillRectangle(new SolidBrush(Color.FromArgb(90, ZOOM_OUT.Color)),
                                nStartX + 1,
                                nStartY + 1,
                                nEndX - nStartX - 1,
                                nEndY - nStartY - 1);
            }, false);
        }

        private void ActionZoomOutEnd(int nMouseX, int nMouseY)
        {
            double dLeftLoc = Math.Min(nMouseX, this.m_pInitialMouseLoc.X);
            double dTopLoc = Math.Min(nMouseY, this.m_pInitialMouseLoc.Y);
            double dWidth = Math.Abs(nMouseX - m_pInitialMouseLoc.X);
            double dHeight = Math.Abs(nMouseY - m_pInitialMouseLoc.Y);
            double dRelation = dWidth > dHeight
                                            ? (dWidth / this.Width)
                                            : (dHeight / this.Height);
            double dOldZoom = this.ZoomScale;
            this.ZoomScale = this.ZoomScale * dRelation;
            this.LeftLocation -= (int)(dLeftLoc / dOldZoom);
            this.TopLocation -= (int)(dTopLoc / dOldZoom);
            this.Render();
        }

        private void ActionZoomInMove(int nMouseX, int nMouseY)
        {
            this.Render(g =>
                {
                    int nStartX = (int)Math.Min(this.m_pInitialMouseLoc.X, nMouseX);
                    int nStartY = (int)Math.Min(this.m_pInitialMouseLoc.Y, nMouseY);
                    int nEndX = (int)Math.Max(this.m_pInitialMouseLoc.X, nMouseX);
                    int nEndY = (int)Math.Max(this.m_pInitialMouseLoc.Y, nMouseY);
                    g.DrawRectangle(ZOOM_IN,
                                    nStartX,
                                    nStartY,
                                    nEndX - nStartX,
                                    nEndY - nStartY);
                    g.FillRectangle(new SolidBrush(Color.FromArgb(90, ZOOM_IN.Color)),
                                    nStartX + 1,
                                    nStartY + 1,
                                    nEndX - nStartX - 1,
                                    nEndY - nStartY - 1);
                }, false);
        }

        private void ActionZoomInEnd(int nMouseX, int nMouseY)
        {
            double dLeftLoc = Math.Min(nMouseX, this.m_pInitialMouseLoc.X);
            double dTopLoc = Math.Min(nMouseY, this.m_pInitialMouseLoc.Y);
            double dWidth = Math.Abs(nMouseX - m_pInitialMouseLoc.X);
            double dHeight = Math.Abs(nMouseY - m_pInitialMouseLoc.Y);
            double dRelation = dWidth > dHeight 
                                            ? (dWidth / this.Width)
                                            : (dHeight / this.Height);
            double dOldZoom = this.ZoomScale;
            this.ZoomScale = this.ZoomScale / dRelation;
            this.LeftLocation += (int)(dLeftLoc / dOldZoom);
            this.TopLocation += (int)(dTopLoc / dOldZoom);
            this.Render();
        }

        #endregion

        #region Event handling

        private void MyMapControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.MapImage != null)
            {
                this.m_bMouseDown = true;
                this.m_pPrevMouseLoc = new Point(e.X, e.Y);
                this.m_pInitialMouseLoc = new Point(e.X, e.Y);
            }
        }

        private void MyMapControl_MouseUp(object sender, MouseEventArgs e)
        {
            this.m_bMouseDown = false;
            switch (this.CurrentAction)
            {
                case MapAction.ZoomIn:
                {
                    this.ActionZoomInEnd(e.X, e.Y);
                    break;
                }
                case MapAction.ZoomOut:
                {
                    this.ActionZoomOutEnd(e.X, e.Y);
                    break;
                }
            }
        }

        private void MyMapControl_MouseMove(object sender, MouseEventArgs e)
        {
            if ((this.m_bMouseDown) && (this.MapImage != null))
            {
                switch (this.m_objCurrAction)
                {
                    case MapAction.None:
                        break;
                    case MapAction.Drag:
                        this.ActionDrag(e.X, e.Y);
                        break;
                    case MapAction.ZoomIn:
                        this.ActionZoomInMove(e.X, e.Y);
                        break;
                    case MapAction.ZoomOut:
                        this.ActionZoomOutMove(e.X, e.Y);
                        break;
                    default:
                        break;
                }
            }
        }

        private void MyMapControl_Load(object sender, EventArgs e)
        {
            // If we are not in design mode, render
            if (!DesignMode)
            {
                this.Render();
            }
            else
            {
                this.BackColor = Color.DarkGray;
            }
        }

        private void MyMapControl_Resize(object sender, EventArgs e)
        {
            // Set map image to reset minimum zoom value
            this.MapImage = this.MapImage;
        }

        #endregion
    }

    public enum MapAction
    {
        None,
        Drag,
        ZoomIn,
        ZoomOut
    }
}
