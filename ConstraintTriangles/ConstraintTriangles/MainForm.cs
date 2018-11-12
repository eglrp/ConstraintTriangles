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
        private IList<DTriangle> _triangles;//三角形集合
        private IList<DEdge> _edges;
        private IList<DVertex> _vertices;
        public MainForm()
        {
            InitializeComponent();

            _triangles = new List<DTriangle>();
            _edges = new List<DEdge>();
            _vertices = new List<DVertex>();

            this.rb_AutoTriangle.Checked = true;//加一个点生成则立刻判断三角网的生成
            this.rb_ManualTriangle.Checked = false;//加完点才手动点击按钮生成三角网（画图生成的速度）

            this.btn_StartTriangle.Click += btn_StartTriangle_Click;
            this.btn_Clear.Click += btn_Clear_Click;
            this.pictureBox.Paint += pictureBox_Paint;
            this.pictureBox.MouseDown += pictureBox_MouseDown;
            
        }

        void btn_Clear_Click(object sender, EventArgs e)
        {
            this._triangles.Clear();
            this._edges.Clear();
            this._vertices.Clear();

            this.pictureBox.CreateGraphics().Clear(Color.White);
        }

        void btn_StartTriangle_Click(object sender, EventArgs e)
        {
            
        }

        void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            Point[] pts=new Point[4]{new Point(e.X-threshold, e.Y-threshold),new Point(e.X+threshold, e.Y-threshold),new Point(e.X+threshold, e.Y+threshold),new Point(e.X-threshold, e.Y+threshold)};
            Graphics g =  this.pictureBox.CreateGraphics();
            g.FillPolygon(Brushes.Red,pts);

            DVertex dv = new DVertex(e.X, e.Y, 1);
            this._vertices.Add(dv);
            ConstructDelaunay();
        }

        void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(Color.FromArgb(255,255,255));
        }

        #region 计算三角网的方法
        //计算delaunay三角网
        private void ConstructDelaunay()
        { }

        #endregion

    }
}
