namespace TukiExp
{
    partial class TukiMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.myMapControl1 = new TukiExp.MyMapControl();
            this.radDrag = new System.Windows.Forms.RadioButton();
            this.radZoomIn = new System.Windows.Forms.RadioButton();
            this.radZoomOut = new System.Windows.Forms.RadioButton();
            this.myMinimapControl1 = new TukiExp.MyMinimapControl();
            this.SuspendLayout();
            // 
            // myMapControl1
            // 
            this.myMapControl1.BackColor = System.Drawing.Color.DarkGray;
            this.myMapControl1.CurrentAction = TukiExp.MapAction.ZoomOut;
            this.myMapControl1.Cursor = System.Windows.Forms.Cursors.Cross;
            this.myMapControl1.Dock = System.Windows.Forms.DockStyle.Right;
            this.myMapControl1.LeftLocation = 0;
            this.myMapControl1.Location = new System.Drawing.Point(265, 0);
            this.myMapControl1.MapImage = global::TukiExp.Properties.Resources.Map;
            this.myMapControl1.MiniMap = null;
            this.myMapControl1.Name = "myMapControl1";
            this.myMapControl1.Size = new System.Drawing.Size(1009, 999);
            this.myMapControl1.TabIndex = 0;
            this.myMapControl1.TopLeftLocation = new System.Drawing.Point(0, 0);
            this.myMapControl1.TopLocation = 0;
            this.myMapControl1.ZoomScale = 10D;
            // 
            // radDrag
            // 
            this.radDrag.Appearance = System.Windows.Forms.Appearance.Button;
            this.radDrag.BackColor = System.Drawing.Color.Gold;
            this.radDrag.Location = new System.Drawing.Point(175, 82);
            this.radDrag.Name = "radDrag";
            this.radDrag.Size = new System.Drawing.Size(68, 22);
            this.radDrag.TabIndex = 1;
            this.radDrag.TabStop = true;
            this.radDrag.Text = "Drag";
            this.radDrag.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radDrag.UseVisualStyleBackColor = false;
            this.radDrag.CheckedChanged += new System.EventHandler(this.radDrag_CheckedChanged);
            // 
            // radZoomIn
            // 
            this.radZoomIn.Appearance = System.Windows.Forms.Appearance.Button;
            this.radZoomIn.BackColor = System.Drawing.Color.Gold;
            this.radZoomIn.Location = new System.Drawing.Point(175, 110);
            this.radZoomIn.Name = "radZoomIn";
            this.radZoomIn.Size = new System.Drawing.Size(68, 22);
            this.radZoomIn.TabIndex = 2;
            this.radZoomIn.TabStop = true;
            this.radZoomIn.Text = "Zoom In";
            this.radZoomIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radZoomIn.UseVisualStyleBackColor = false;
            this.radZoomIn.CheckedChanged += new System.EventHandler(this.radZoomIn_CheckedChanged);
            // 
            // radZoomOut
            // 
            this.radZoomOut.Appearance = System.Windows.Forms.Appearance.Button;
            this.radZoomOut.BackColor = System.Drawing.Color.Gold;
            this.radZoomOut.Location = new System.Drawing.Point(175, 138);
            this.radZoomOut.Name = "radZoomOut";
            this.radZoomOut.Size = new System.Drawing.Size(68, 22);
            this.radZoomOut.TabIndex = 3;
            this.radZoomOut.TabStop = true;
            this.radZoomOut.Text = "Zoom Out";
            this.radZoomOut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.radZoomOut.UseVisualStyleBackColor = false;
            this.radZoomOut.CheckedChanged += new System.EventHandler(this.radZoomOut_CheckedChanged);
            // 
            // myMinimapControl1
            // 
            this.myMinimapControl1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.myMinimapControl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.myMinimapControl1.Location = new System.Drawing.Point(271, 0);
            this.myMinimapControl1.Name = "myMinimapControl1";
            this.myMinimapControl1.ObservedMap = null;
            this.myMinimapControl1.Size = new System.Drawing.Size(163, 296);
            this.myMinimapControl1.TabIndex = 4;
            // 
            // TukiMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1274, 999);
            this.Controls.Add(this.myMinimapControl1);
            this.Controls.Add(this.radZoomOut);
            this.Controls.Add(this.radZoomIn);
            this.Controls.Add(this.radDrag);
            this.Controls.Add(this.myMapControl1);
            this.Name = "TukiMain";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.TukiMain_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private MyMapControl myMapControl1;
        private System.Windows.Forms.RadioButton radDrag;
        private System.Windows.Forms.RadioButton radZoomIn;
        private System.Windows.Forms.RadioButton radZoomOut;
        private MyMinimapControl myMinimapControl1;
    }
}

