using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Pentago
{
    public partial class frmRulez : Form
    {
        public frmRulez(int nX, int nY)
        {
            InitializeComponent();
            this.textBox1.Text = Pentago.Properties.Resources.rules;
            this.textBox1.SelectionStart = 0;
            this.Top = nY;
            this.Left = nX;
        }
    }
}
