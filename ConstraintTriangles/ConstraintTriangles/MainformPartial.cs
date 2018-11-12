using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///在实际的应用之中，坐标应该是double类型的，这里是有int是因为画布只能捕捉整形像素
///在三维计算中也是同样的原理，映射到一个平面上即可计算出三角网
namespace ConstraintTriangles
{
    //////主窗体伙伴类，用到的部分结构体与内置方法
    partial class MainForm
    {
        protected readonly double precision = 0.01; //默认的阈值--c#里面画图似乎只有整数
        private readonly int threshold = 3;//绘制点的阈值
        enum enumStackType
        {
            consVertType = 1<<1,//约束点
            consLineType = 1<<2//约束线
        }
    }
    //计算凸包时的临时结构体
    struct Pnt_ID
    {
        public Pnt_ID(int index , int fvalue)
        {
            this.index = index;
            this.fvalue = fvalue;
        }
        public int fvalue;
        public int index;
    }
    struct DVertex
    {
        public DVertex(int dx,int dy, int isHullVertex,bool isFastigiumPointFlag = false)
        {
            this.dx = dx;
            this.dy = dy;
            this.isHullVertex = isHullVertex;
            this.isFastigiumPointFlag = isFastigiumPointFlag;
        }
        public int dx;
        public int dy;

        public int isHullVertex;//点的类型标记--暂定
        public bool isFastigiumPointFlag;//屋脊点标记

        public static bool operator ==(DVertex v1, DVertex v2)
        {
            return v1.dx==v2.dx&&v1.dy==v2.dy;
        }
        public static bool operator !=(DVertex v1, DVertex v2)
        {
            return v1.dx != v2.dx || v1.dy != v2.dy;
        }
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
    struct DEdge
    {
        public DEdge(int dEdge_index1,int dEdge_index2,bool isHullEdge,int AdjDTriangle1_index,int AdjDTriangle2_index)
        {
            this.dEdge_index1 = dEdge_index1;
            this.dEdge_index2 = dEdge_index2;
            this.isHullEdge = isHullEdge;
            this.AdjDTriangle1_index = AdjDTriangle1_index;
            this.AdjDTriangle2_index = AdjDTriangle2_index;
        }
        public int dEdge_index1;//首点索引
        public int dEdge_index2;

        public bool isHullEdge;//是否是外包边
        public int AdjDTriangle1_index;//边关联三角形
        public int AdjDTriangle2_index;
    }
    struct DTriangle
    {
        public DTriangle(int dTriangle_index1, int dTriangle_index2, int dTriangle_index3, bool isDelete, bool edge1_isHull, bool edge2_isHull, bool edge3_isHull, int AdjDTriangleIndex1, int AdjDTriangleIndex2, int AdjDTriangleIndex3)
        {
            this.dTriangle_index1 = dTriangle_index1;
            this.dTriangle_index2 = dTriangle_index2;
            this.dTriangle_index3 = dTriangle_index3;
            this.isDelete = isDelete;
            this.edge1_isHull = edge1_isHull;
            this.edge2_isHull = edge2_isHull;
            this.edge3_isHull = edge3_isHull;
            this.AdjDTriangleIndex1 = AdjDTriangleIndex1;
            this.AdjDTriangleIndex2 = AdjDTriangleIndex2;
            this.AdjDTriangleIndex3 = AdjDTriangleIndex3;
        }
        public int dTriangle_index1;//首点索引 
        public int dTriangle_index2;//首点索引 
        public int dTriangle_index3;//首点索引 

        bool isDelete;//标志在三角形集合中是否被删除-----暂定使用

        bool edge1_isHull;//(v1,v2)判断当前的边是否为凸壳的边
        bool edge2_isHull;//(v1,v2)
        bool edge3_isHull;//(v1,v2)

        public int AdjDTriangleIndex1;//edge1的邻近三角形索引
        public int AdjDTriangleIndex2;
        public int AdjDTriangleIndex3;
    }


}
