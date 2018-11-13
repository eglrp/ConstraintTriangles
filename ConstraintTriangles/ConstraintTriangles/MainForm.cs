using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

///该程序只是在三维中需要建立屋脊做的测试验证代码，实际运用中还需要考虑很多,
///实际复杂的代码与逻辑并没有完全呈现，但是主体约束三角网的核心是不变的
///1.增加了外包多边形的约束，保证了生成的三角网一定是在多边形范围内的
///2.可以在范围内（不包括外包线上）增加约束点（未呈现在代码中，原理与之前的生成三角网的逻辑一致--先全部生成三角网，再外包约束）
///3.在范围内增加约束线（实际上与外包约束线算法是相同的，这里只实现简单的单约束线，多约束线原理相同，但需考虑约束线不相交）
namespace ConstraintTriangles
{
    public partial class MainForm : Form
    {
        private IList<DTriangle> _triangles;//三角形集合
        private IList<DEdge> _edges;
        private IList<DVertex> _vertices;

        private IList<DVertex> hullVertices;//初始外包
        private IList<int> hullPoints;//初始外包矩形索引
        public MainForm()
        {
            InitializeComponent();

            _triangles = new List<DTriangle>();
            _edges = new List<DEdge>();
            _vertices = new List<DVertex>();
            hullVertices = new List<DVertex>();
            hullPoints = new List<int>();

            this.btn_StartTriangle.Click += btn_StartTriangle_Click;
            this.btn_Clear.Click += btn_Clear_Click;
            this.pictureBox.Paint += pictureBox_Paint;
            this.pictureBox.MouseDown += pictureBox_MouseDown;
            this.pictureBox.MouseMove += pictureBox_MouseMove;
        }

        //如果是动态绘制--暂时不考虑
        void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {

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
            Point[] pts = new Point[4] { new Point(e.X - threshold, e.Y - threshold), new Point(e.X + threshold, e.Y - threshold), new Point(e.X + threshold, e.Y + threshold), new Point(e.X - threshold, e.Y + threshold) };
            Graphics g = GetGraphics();
            g.FillPolygon(Brushes.Red, pts);
            DVertex dv = new DVertex(e.X, e.Y, 1);
            this._vertices.Add(dv);
            if (_vertices.Count > 1)
                g.DrawLines(Pens.DeepPink, GetPoints());
            if(_vertices.Count>2)
                ConstructDelaunay();
        }

        void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(Color.FromArgb(255,255,255));
        }

        #region 计算三角网的方法
        //计算delaunay三角网
        private void ConstructDelaunay()
        {
            ClearTriangles();
        }
        //清除之前绘制的信息
        private void ClearTriangles()
        {

        }
        //根据当前的点集计算外包
        private void ComputeHullVertex()
        {
            _PtTemp pMaxMinus, pMinMinus, pMaxAdd, pMinAdd;
            pMinMinus.index = pMinAdd.index = pMaxMinus.index = pMaxAdd.index = 0;
            pMinMinus.value  = pMaxMinus.value = _vertices[0].dx - _vertices[0].dy;
            pMinAdd.value = pMaxAdd.value = _vertices[0].dx + _vertices[0].dy;
            int temp = 0;
            for (int i = 1; i < _vertices.Count; ++i)
            {
                if (_vertices[i].isFastigiumPointFlag)
                    continue;
                temp = _vertices[i].dx - _vertices[i].dy;
                if (temp > pMaxMinus.value)
                {
                    pMaxMinus.value = temp;
                    pMaxMinus.index = i;
                }
                else if (temp < pMinMinus.value)
                {
                    pMinMinus.value = temp;
                    pMinMinus.index = i; 
                }
                temp = _vertices[i].dx + _vertices[i].dy;
                if (temp > pMaxAdd.value)
                {
                    pMaxAdd.value = temp;
                    pMaxAdd.index = i;
                }
                else if (temp < pMinAdd.value)
                {
                    pMinAdd.value = temp;
                    pMinAdd.index = i;
                }
                //逆时针
                hullPoints.Add(pMinMinus.index);
                hullPoints.Add(pMaxAdd.index);
                hullPoints.Add(pMaxMinus.index);
                hullPoints.Add(pMaxMinus.index);

            }
        }

        private Graphics g = null;
        private Graphics GetGraphics()
        {
            if (g == null)
                g = this.pictureBox.CreateGraphics();
            return g;
        }
        //根据节点数组生成点集合
        private Point[] GetPoints()
        {
            IList<Point> arrP = new List<Point>();
            Array.ForEach<DVertex>(_vertices.ToArray(), vertex => arrP.Add(new Point(vertex.dx, vertex.dy)));
            return arrP.ToArray();
        }
        #endregion

    }
}
