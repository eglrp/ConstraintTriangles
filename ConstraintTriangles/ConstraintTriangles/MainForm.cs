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
///4.不判断非法外包！！！
///5.绘制非法，直接绘制外包多边形，最后右键结束绘制！！
namespace ConstraintTriangles
{
    public partial class MainForm : Form
    {
        private Graphics g = null;

        private IList<DTriangle> _triangles;//三角形集合
        private IList<DEdge> _edges;
        private IList<DVertex> _vertices;

        private IList<DVertex> hullVertices;//初始外包
        private IList<DEdge> outsideEdges;//外包边
        private IList<DEdge> insideEdges;
        private IList<int> hullPoints;//初始外包矩形索引
        private IList<int> hullPoints_copy;
        public MainForm()
        {
            InitializeComponent();

            _triangles = new List<DTriangle>();
            _edges = new List<DEdge>();
            _vertices = new List<DVertex>();
            hullVertices = new List<DVertex>();
            hullPoints = new List<int>();
            hullPoints_copy = new List<int>();
            outsideEdges = new List<DEdge>();
            insideEdges = new List<DEdge>()

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
            this.hullPoints.Clear();
            this.hullVertices.Clear();
            this.outsideEdges.Clear();

            this.pictureBox.CreateGraphics().Clear(Color.White);
        }

        void btn_StartTriangle_Click(object sender, EventArgs e)
        {

        }

        void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)//右键设置为闭合
                return;
            Point[] pts = new Point[4] { new Point(e.X - threshold, e.Y - threshold), new Point(e.X + threshold, e.Y - threshold), new Point(e.X + threshold, e.Y + threshold), new Point(e.X - threshold, e.Y + threshold) };
            Graphics g = GetGraphics();
            g.FillPolygon(Brushes.Red, pts);
            DVertex dv = new DVertex(e.X, e.Y, 1);
            this._vertices.Add(dv);
            this.hullVertices.Add(dv);
            if (_vertices.Count > 1)
                g.DrawLines(Pens.DeepPink, GetPoints());
            if (_vertices.Count > 2)
            {
                ClearTriangles();
                BuildDelaunay();
            }
        }

        void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(Color.FromArgb(255,255,255));
        }

        #region 计算三角网的方法
        private void BuildDelaunay()
        {
            CreateConvex();
            CreateHullTriangle();
            InsertOtherVertex();
            RemoveSuperfluousEdge();
        }
        //计算delaunay三角网
        private void CreateConvex()
        {
            ConstructHullLines();
            ComputeHullVertex();
            //Array.ForEach<int>(hullPoints.ToArray(), index => { DVertex dv = _vertices[index]; dv.isHullVertex = -1; _vertices[index] = dv; });//注意c#struct是值类型，不像c++是引用类型
            Array.ForEach(hullPoints.ToArray(), index => _vertices[index].isHullVertex = -1);
            hullPoints = hullPoints.OrderBy(i => i).ToList();
            hullPoints_copy = hullPoints;//应该不用深拷贝？！
            ReconstructHullPointFromDVertices();
        }
        //清除之前绘制的信息
        private void ClearTriangles()
        {
            _vertices.Clear();
            _edges.Clear();
            _triangles.Clear();
            hullPoints.Clear();
            hullVertices.Clear();
            outsideEdges.Clear();
        }
        //建立手绘外包边集合
        private void ConstructHullLines()
        {
            if (outsideEdges.Count > 0)
                outsideEdges.Clear();
            int count = hullVertices.Count;
            for (int i = 0; i < count; ++i)
            {
                DEdge edge = new DEdge(i, (i + 1) / count);
                outsideEdges.Add(edge);
            }
        }
        //根据当前的点集计算外包
        private void ComputeHullVertex()
        {
            _PtTemp pMaxMinus, pMinMinus, pMaxAdd, pMinAdd;
            pMinMinus.index = pMinAdd.index = pMaxMinus.index = pMaxAdd.index = 0;
            pMinMinus.value = pMaxMinus.value = _vertices[0].dx - _vertices[0].dy;
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
            hullPoints.Distinct();//去除重复索引，针对三角形
        }
        //将其他点加入到点集合中
        private void ReconstructHullPointFromDVertices()
        {
            int count = hullPoints_copy.Count;
            for (int i = 0; i < count; ++i)
            {
                ConvexLine(_vertices[hullPoints_copy[i % count]], _vertices[hullPoints_copy[(i + 1) % count]]);
            }
            for (int index = 0; index < hullPoints.Count; ++index)
            {
                _vertices[hullPoints[index]].isHullVertex = 1;//标记凸壳点
            }
        }
        private void ConvexLine(DVertex dv1, DVertex dv2)
        {
            float distance = -1.0f;
            int currentIndex = -1;
            for (int i = 0; i < _vertices.Count; ++i)
            {//这里的冗余非常多，可以考虑传递索引？
                DVertex vecPoint = _vertices[i];
                if (vecPoint.isHullVertex == -1)
                    continue;
                int position = VectorXMultiplyVertex(dv1, dv2, vecPoint);//计算共线情况
                if (position == -1)
                {//在线的左侧，继续搜索
                    continue;
                }
                else if (position == 0)
                {//在线段上，则记下与线的距离
                    if (distance == -1.0f)
                    {
                        distance = 0.0f;
                        currentIndex = i;
                    }
                }
                else if (position == 2)
                {//在线段延长线上，继续搜索
                    continue;
                }
                else if (position == 1)
                {//在线的右侧，记录下与线段距离最远的那个点
                    float dis = VLDistance(dv1, dv2, vecPoint);
                    if (distance < dis)
                    {
                        distance = dis;
                        currentIndex = i;
                    }
                }
            }
            if (distance == -1.0f)
            {
                return;
            }
            else
            {
                if (distance == 0.0f)
                {//三点共线
                    return;
                }
                int index = 0;
                for (int i = 0; i < hullPoints.Count; ++i)
                {
                    DVertex v = _vertices[hullPoints[i]];
                    if (v == dv2)
                    {
                        index = i;
                        break;
                    }
                }
                hullPoints.Insert(index, currentIndex);
                _vertices[currentIndex].isHullVertex = -1;
                ConvexLine(dv1, _vertices[currentIndex]);
                ConvexLine(_vertices[currentIndex], dv2);
            }
        }
        /// 点V3与 点V1,V2组成的线段的位置关系
        /// 采用的算法为面积法。1表示在其右侧，-1表示在其左侧，0表示在线段上，2表示在线段的延长线上
        private int VectorXMultiplyVertex(DVertex V1, DVertex V2, DVertex V3)
        {
            float S = (V2.dx - V1.dx) * (V3.dy - V1.dy) - (V3.dx - V1.dx) * (V2.dy - V1.dy);
            if (S == 0)
            {
                if (V3.dx >= Math.Min(V1.dx, V2.dx) && V3.dx <= Math.Max(V1.dx, V2.dx) && V3.dy >= Math.Min(V1.dy, V2.dy) && V3.dy <= Math.Max(V1.dy, V2.dy))
                {
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else if (S > 0)
            {
                return -1;
            }
            else if (S < 0)
            {
                return 1;
            }
            return -1;
        }
        /// 点V3 与 点V1,V2组成的直线的距离
        private float VLDistance(DVertex V1, DVertex V2, DVertex V3)
        {
            double top = (V2.dy - V1.dy) * V3.dx + (V1.dx - V2.dx) * V3.dy + V1.dy * (V2.dx - V1.dx) - (V2.dy - V1.dy) * V1.dx;
            double bottom = Math.Sqrt((V2.dy - V1.dy) * (V2.dy - V1.dy) + (V2.dx - V1.dx) * (V2.dx - V1.dx));
            return (float)Math.Abs(top / bottom);
        }
        //凸壳三角形剖分
        private void CreateHullTriangle()
        {
            IList<int> tempVec = hullPoints;
            int id1 = -1, id2 = -1, id3 = -1;
            while (tempVec.Count >= 3)
            {
                int tempSize = tempVec.Count;
                for (int i = 0; i < tempSize; ++i)
                {
                    //干净则加入网中
                    id1 = tempVec[i % tempSize];
                    id2 = tempVec[(i + 1) % tempSize];
                    id3 = tempVec[(i + 2) % tempSize];
                    if (IsCleanTriangle(id1, id2, id3))
                    {
                        DTriangle tri = new DTriangle(id1, id2, id3);
                        _triangles.Add(tri);

                        _vertices[id2].isHullVertex = 2;//标记为构三角网节点中间点
                        tempVec.RemoveAt((i + 1) % tempSize);//去除中间点！！！
                        break;
                    }
                }
            }
        }

        //逐点插入其它点构TIN
        private void InsertOtherVertex()
        {
            IList<DEdge> edgesBuf = new List<DEdge>();
            bool isInCircle = false;
            for (int i = 0; i < _vertices.Count; ++i)
            {
                if (_vertices[i].isHullVertex != 0)
                    continue;//去除凸壳点
                edgesBuf.Clear();
                for (int j = 0; j < _triangles.Count; ++j)
                {
                    isInCircle = InTriangleExtCircle(_vertices[i].dx, _vertices[i].dy, _vertices[_triangles[j].dTriangle_index1].dx, _vertices[_triangles[j].dTriangle_index1].dy, _vertices[_triangles[j].dTriangle_index2].dx, _vertices[_triangles[j].dTriangle_index2].dy, _vertices[_triangles[j].dTriangle_index3].dx, _vertices[_triangles[j].dTriangle_index3].dy);
                    if (isInCircle)//当前的点在约束三角形内
                    {
                        IList<DEdge> vecTemp = new List<DEdge>();
                        DEdge edge1 = new DEdge(_triangles[j].dTriangle_index1, _triangles[j].dTriangle_index2);
                        DEdge edge2 = new DEdge(_triangles[j].dTriangle_index2, _triangles[j].dTriangle_index3);
                        DEdge edge3 = new DEdge(_triangles[j].dTriangle_index3, _triangles[j].dTriangle_index1);
                        vecTemp.Add(edge1);
                        vecTemp.Add(edge2);
                        vecTemp.Add(edge3);
                        //存储除公共边外的三角形边
                        bool isNotCommonEdge;
                        for (int k = 0; k < 3; ++k)
                        {
                            isNotCommonEdge = true;
                            for (int n = 0; n < edgesBuf.Count; ++n)
                            {
                                if (vecTemp[k] == edgesBuf[n])//如果为公共边
                                {
                                    edgesBuf[n] = edgesBuf[edgesBuf.Count - 1];
                                    edgesBuf.RemoveAt(edgesBuf.Count - 1);//删除公共边
                                    isNotCommonEdge = false;
                                    break;
                                }
                            }
                            if (isNotCommonEdge)
                                edgesBuf.Add(vecTemp[k]);
                        }
                        _triangles[j] = _triangles[_triangles.Count - 1];
                        _triangles.RemoveAt(_triangles.Count - 1);//删除当前无效三角形
                        --j;
                    }
                }
                //构建新的三角形
                for (int j = 0; j < edgesBuf.Count; ++j)
                {
                    DTriangle tri = new DTriangle(edgesBuf[j].dEdge_index1, edgesBuf[j].dEdge_index2, i);
                    _triangles.Add(tri);
                }
            }
        }
        //删除多余的三角形
        void RemoveSuperfluousEdge()
        {
            	IList<DEdge> vecTemp=ReturnNoExisitHullEdge(outsideEdges);
	ConstructAllTriangleProperty();
	ConstructAllEdgeProperty();
	//为集合中的外包边建立约束
	for(int m=0;m<vecTemp.Count;++m)
	{
		GetDelaunayByHullLine(vecTemp.at(m));
	}
	insideEdges.clear();
	bool isInFlag1,isInFlag2,isInFlag3;
	for(size_t i=0;i<DTraingles.size();++i)
	{
		isInFlag1=isInFlag2=isInFlag3=true;
		DTraingles.at(i).edge1_isHull=ExistEdge(staticOutsideEdges,DTraingles.at(i).dTriangle_index1,DTraingles.at(i).dTriangle_index2);
		DTraingles.at(i).edge2_isHull=ExistEdge(staticOutsideEdges,DTraingles.at(i).dTriangle_index2,DTraingles.at(i).dTriangle_index3);
		DTraingles.at(i).edge3_isHull=ExistEdge(staticOutsideEdges,DTraingles.at(i).dTriangle_index3,DTraingles.at(i).dTriangle_index1);

		if(!DTraingles.at(i).edge1_isHull)//非外包边
		{//12
			isInFlag1=InPolygon(DVertices.at(DTraingles.at(i).dTriangle_index1).dx,DVertices.at(DTraingles.at(i).dTriangle_index1).dy,DVertices.at(DTraingles.at(i).dTriangle_index2).dx,DVertices.at(DTraingles.at(i).dTriangle_index2).dy,hullDVertices_Origin);
		}
		if(!DTraingles.at(i).edge2_isHull)
		{//23
			isInFlag2=InPolygon(DVertices.at(DTraingles.at(i).dTriangle_index2).dx,DVertices.at(DTraingles.at(i).dTriangle_index2).dy,DVertices.at(DTraingles.at(i).dTriangle_index3).dx,DVertices.at(DTraingles.at(i).dTriangle_index3).dy,hullDVertices_Origin);
		}
		if(!DTraingles.at(i).edge3_isHull)
		{//31
			isInFlag3=InPolygon(DVertices.at(DTraingles.at(i).dTriangle_index3).dx,DVertices.at(DTraingles.at(i).dTriangle_index3).dy,DVertices.at(DTraingles.at(i).dTriangle_index1).dx,DVertices.at(DTraingles.at(i).dTriangle_index1).dy,hullDVertices_Origin);
		}
		if(isInFlag1&&isInFlag2&&isInFlag3)
			DTraingles.at(i).isDelete=false;
		else 
			DTraingles.at(i).isDelete=true;
	}
	//剔除掉标记为delete的三角形
	vector<DTraingle>::const_iterator iter=DTraingles.cbegin();
	while(iter!=DTraingles.cend())
	{
		if(iter->isDelete)
		{
			iter = DTraingles.erase(iter);
		}
		else 
			++iter;
	}
	ConstructTriangleProperty(staticOutsideEdges);
	ConstructEdgeProperty(staticOutsideEdges);
        }
        private IList<DEdge> ReturnNoExisitHullEdge(IList<DEdge> edges)
        {
            IList<DEdge> vecTemp = new List<DEdge>();
            for (int k = 0; k < edges.Count; ++k)
            {
                if (!EdgeConstructTriangle(edges[k]))
                    vecTemp.Add(edges[k]);//将没有构成三角形的外包边保存下来
            }
            return vecTemp;
        }
        private bool EdgeConstructTriangle(DEdge edge)
        {
            for (int i = 0; i < _triangles.Count; ++i)
            {
                if (IsConstructTriangle(_triangles[i], edge.dEdge_index1) && IsConstructTriangle(_triangles[i], edge.dEdge_index2))
                    return true;
            }
            return false;
        }
        private bool IsConstructTriangle(DTriangle triangle, int vertIndex)
        {
            if (vertIndex == triangle.dTriangle_index1)
                return true;
            else if (vertIndex == triangle.dTriangle_index2)
                return true;
            else if (vertIndex == triangle.dTriangle_index3)
                return true;
            return false;
        }
        private void ConstructAllTriangleProperty()
        {
            for (int i = 0; i < _triangles.Count; ++i)
            {
                for (int j = 0; j < _triangles.Count; ++j)
                {
                    if (i != j)
                    {
                        if (CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index1, _triangles[j].dTriangle_index2) || CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index2, _triangles[j].dTriangle_index3) || CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index3, _triangles[j].dTriangle_index1))
                            _triangles[i].AdjDTriangleIndex1 = j;

                        if (CompareEdge(_triangles[i].dTriangle_index2, _triangles[i].dTriangle_index3, _triangles[j].dTriangle_index1, _triangles[j].dTriangle_index2) || CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index2, _triangles[j].dTriangle_index3) || CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index3, _triangles[j].dTriangle_index1))
                            _triangles[i].AdjDTriangleIndex2 = j;

                        if (CompareEdge(_triangles[i].dTriangle_index3, _triangles[i].dTriangle_index1, _triangles[j].dTriangle_index1, _triangles[j].dTriangle_index2) || CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index2, _triangles[j].dTriangle_index3) || CompareEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2, _triangles[j].dTriangle_index3, _triangles[j].dTriangle_index1))
                            _triangles[i].AdjDTriangleIndex3 = j;
                    }
                }
            }
        }

        private bool CompareEdge(DEdge edge, int index1, int index2)
        {
            return CompareEdge(edge.dEdge_index1, edge.dEdge_index2, index1, index2);
        }
        private bool CompareEdge(int index_X1, int index_Y1, int index_X2, int index_Y2)
        {
            return ((index_X1 == index_X2) && (index_Y1 == index_Y2)) || ((index_X1 == index_Y2) && (index_Y1 == index_X2));
        }
        private void ConstructAllEdgeProperty()
        {
            for (int i = 0; i < _triangles.Count; ++i)
            {
                if (!ExistEdge(outsideEdges, _triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2))
                {
                    //12
                    DEdge edge = new DEdge(_triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2);
                    edge.AdjDTriangle1_index = i;
                    edge.AdjDTriangle2_index = _triangles[i].AdjDTriangleIndex1;

                    int index = ExistEdgeIndex(insideEdges, _triangles[i].dTriangle_index1, _triangles[i].dTriangle_index2);
                    if (index != -1)
                    {
                        insideEdges[index].AdjDTriangle1_index = i;
                        insideEdges[index].AdjDTriangle2_index = _triangles[i].AdjDTriangleIndex1;
                    }
                    else
                        insideEdges.Add(edge);
                }
                if (!ExistEdge(outsideEdges, _triangles[i].dTriangle_index2, _triangles[i].dTriangle_index3))
                {
                    //23
                    DEdge edge = new DEdge(_triangles[i].dTriangle_index2, _triangles[i].dTriangle_index3);
                    edge.AdjDTriangle1_index = i;
                    edge.AdjDTriangle2_index = _triangles[i].AdjDTriangleIndex2;

                    int index = ExistEdgeIndex(insideEdges, edge.dEdge_index1, edge.dEdge_index2);
                    if (index != -1)
                    {
                        insideEdges[index].AdjDTriangle1_index = i;
                        insideEdges[index].AdjDTriangle2_index = _triangles[i].AdjDTriangleIndex2;
                    }
                    else
                        insideEdges.Add(edge);
                }
                if (!ExistEdge(outsideEdges, _triangles[i].dTriangle_index3, _triangles[i].dTriangle_index1))
                {
                    //31
                    DEdge edge = new DEdge(_triangles[i].dTriangle_index3, _triangles[i].dTriangle_index1);
                    edge.AdjDTriangle1_index = i;
                    edge.AdjDTriangle2_index = _triangles[i].AdjDTriangleIndex3;

                    int index = ExistEdgeIndex(insideEdges, edge.dEdge_index1, edge.dEdge_index2);
                    if (index != -1)
                    {
                        insideEdges[index].AdjDTriangle1_index = i;
                        insideEdges[index].AdjDTriangle2_index = _triangles[i].AdjDTriangleIndex3;
                    }
                    else
                        insideEdges.Add(edge);
                }
            }
        }

        private bool ExistEdge(IList<DEdge> edges, DEdge edge)
        {
            for (int i = 0; i < edges.Count; ++i)
            {
                if (edges[i] == edge)
                    return true;
            }
            return false;
        }
        private bool ExistEdge(IList<DEdge> edges, int index1, int index2)
        {
            for (int i = 0; i < edges.Count; ++i)
            {
                if (CompareEdge(edges[i], index1, index2))
                    return true;
            }
            return false;
        }
        private int ExistEdgeIndex(IList<DEdge> edges, int edgeIndex1, int edgeIndex2)
        {
            int index = -1;
            for (int i = 0; i < edges.Count; ++i)
            {
                if (CompareEdge(edges[i], edgeIndex1, edgeIndex2))
                    return i;
            }
            return index;
        }





        private bool IsCleanTriangle(int id1, int id2, int id3)
        {
            for (int i = 0; i < hullPoints.Count; ++i)
            {
                //跳过已构网的点和三角形顶点
                if (_vertices[hullPoints[i]].isHullVertex == 2 || hullPoints[i] == id1 || hullPoints[i] == id2 || hullPoints[i] == id3)
                    continue;
                if (InTriangleExtCircle(_vertices[hullPoints[i]].dx, _vertices[hullPoints[i]].dy, _vertices[hullPoints[id1]].dx, _vertices[hullPoints[id1]].dy, _vertices[hullPoints[id2]].dx, _vertices[hullPoints[id2]].dy, _vertices[hullPoints[id3]].dx, _vertices[hullPoints[id3]].dy))
                    return false;
            }
            return true;
        }
        //判断三点共直线，x3,y3为判断点
        private bool JudgeInLine(float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float res = (x3 - x1) * (y2 - y1) - (x2 - x1) * (y3 - y1);
            if (res == 0.0f)
                return true;
            return false;
        }
        //求外接圆的圆心-2维
        bool GetTriangleBarycnt(float x1, float y1, float x2, float y2, float x3, float y3, float BaryCntX, float BaryCntY)
        {
            float k1, k2; //两条中垂线斜率

            //三点共线
            if (JudgeInLine(x1, y1, x2, y2, x3, y3))
                return false;

            //边的中点，使用直线方程斜率计算
            float MidX1 = (x1 + x2) / 2;
            float MidY1 = (y1 + y2) / 2;
            float MidX2 = (x2 + x3) / 2;
            float MidY2 = (y2 + y3) / 2;
            if (Math.Abs(y2 - y1) < precision) //p1p2平行于X轴
            {
                k2 = -(x3 - x2) / (y3 - y2);
                BaryCntX = MidX1;
                BaryCntY = k2 * (BaryCntX - MidX2) + MidY2;
            }
            else if (Math.Abs(y3 - y2) < precision) //p2p3平行于X轴
            {
                k1 = -(x2 - x1) / (y2 - y1);
                BaryCntX = MidX2;
                BaryCntY = k1 * (BaryCntX - MidX1) + MidY1;
            }
            else
            {//二元一次方程
                k1 = -(x2 - x1) / (y2 - y1);
                k2 = -(x3 - x2) / (y3 - y2);
                BaryCntX = (k1 * MidX1 - k2 * MidX2 + MidY2 - MidY1) / (k1 - k2);
                BaryCntY = k1 * (BaryCntX - MidX1) + MidY1;
            }
            return true;
        }

        //判断点是否在三角形的外接圆中--xp，yp是凸壳点
        private bool InTriangleExtCircle(float xp, float yp, float x1, float y1, float x2, float y2, float x3, float y3)
        {
            float RadiusSquare; //半径的平方
            float DisSquare; //距离的平方
            float BaryCntX = 0.0f, BaryCntY = 0.0f;
            if (!GetTriangleBarycnt(x1, y1, x2, y2, x3, y3, BaryCntX, BaryCntY))
                return false;
            RadiusSquare = (x1 - BaryCntX) * (x1 - BaryCntX) + (y1 - BaryCntY) * (y1 - BaryCntY);
            DisSquare = (xp - BaryCntX) * (xp - BaryCntX) + (yp - BaryCntY) * (yp - BaryCntY);
            if (DisSquare <= RadiusSquare)
                return true;
            else
                return false;
        }
        #endregion


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


    }
}
