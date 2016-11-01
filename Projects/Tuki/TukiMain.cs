using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TukiExp
{
    public partial class TukiMain : BaseForm
    {
        public TukiMain()
        {
            InitializeComponent();
        }

        private void radDrag_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radDrag.Checked)
            {
                this.myMapControl1.CurrentAction = MapAction.Drag;
            }
        }

        private void radZoomIn_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radZoomIn.Checked)
            {
                this.myMapControl1.CurrentAction = MapAction.ZoomIn;
            }
        }

        private void radZoomOut_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radZoomOut.Checked)
            {
                this.myMapControl1.CurrentAction = MapAction.ZoomOut;
            }
        }

        private void TukiMain_Load(object sender, EventArgs e)
        {
            this.myMinimapControl1.ObservedMap = this.myMapControl1;
        }
    }
}
