using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ConstraintTriangles
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            this.rb_AutoTriangle.Checked = true;
            this.rb_ManualTriangle.Checked = false;
        }
    }
}
