namespace TukiExp
{
    partial class MyMapControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbMapPic = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbMapPic)).BeginInit();
            this.SuspendLayout();
            // 
            // pbMapPic
            // 
            this.pbMapPic.Location = new System.Drawing.Point(89, 161);
            this.pbMapPic.Name = "pbMapPic";
            this.pbMapPic.Size = new System.Drawing.Size(100, 50);
            this.pbMapPic.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbMapPic.TabIndex = 0;
            this.pbMapPic.TabStop = false;
            this.pbMapPic.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MyMapControl_MouseDown);
            this.pbMapPic.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MyMapControl_MouseMove);
            this.pbMapPic.MouseUp += new System.Windows.Forms.MouseEventHandler(this.MyMapControl_MouseUp);
            // 
            // MyMapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pbMapPic);
            this.DoubleBuffered = true;
            this.Name = "MyMapControl";
            this.Size = new System.Drawing.Size(301, 306);
            this.Load += new System.EventHandler(this.MyMapControl_Load);
            this.Resize += new System.EventHandler(this.MyMapControl_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.pbMapPic)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMapPic;
    }
}
