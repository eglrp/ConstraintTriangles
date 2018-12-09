using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstraintTriangles
{
    //计算凸包时的临时结构体--结构体在属性复制操作时不是很方便，将结构体改成class
    struct _PtTemp
    {
        public _PtTemp(int index, int value)
        {
            this.index = index;
            this.value = value;
        }
        public int value;
        public int index;
    }
    class DVertex
    {
        public DVertex(int dx, int dy, int isHullVertex, bool isFastigiumPointFlag = false)
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
            return v1.dx == v2.dx && v1.dy == v2.dy;
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
    class DEdge
    {
        public DEdge(int dEdge_index1, int dEdge_index2, bool isHullEdge = false, int AdjDTriangle1_index = -1, int AdjDTriangle2_index = -1)
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

        public static bool operator ==(DEdge e1, DEdge e2)
        {
            return ((e1.dEdge_index1 == e2.dEdge_index1) && (e1.dEdge_index2 == e2.dEdge_index2)) || ((e1.dEdge_index2 == e2.dEdge_index1 && e1.dEdge_index1 == e2.dEdge_index2));
        }

        public static bool operator !=(DEdge e1, DEdge e2)
        {
            return (e1.dEdge_index1 != e2.dEdge_index1) || (e1.dEdge_index1 != e2.dEdge_index2);
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
    class DTriangle
    {
        public DTriangle(int dTriangle_index1, int dTriangle_index2, int dTriangle_index3, bool isDelete = false, bool edge1_isHull = false, bool edge2_isHull = false, bool edge3_isHull = false, int AdjDTriangleIndex1 = -1, int AdjDTriangleIndex2 = -1, int AdjDTriangleIndex3 = -1)
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

        public bool isDelete;//标志在三角形集合中是否被删除-----暂定使用

        public bool edge1_isHull;//(v1,v2)判断当前的边是否为凸壳的边
        public bool edge2_isHull;//(v1,v2)
        public bool edge3_isHull;//(v1,v2)

        public int AdjDTriangleIndex1;//edge1的邻近三角形索引
        public int AdjDTriangleIndex2;
        public int AdjDTriangleIndex3;
    }

}
