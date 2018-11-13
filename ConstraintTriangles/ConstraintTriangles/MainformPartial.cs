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

}
