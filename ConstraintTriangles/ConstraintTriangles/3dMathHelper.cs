using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/////三维用到的一些数学计算方法
namespace ConstraintTriangles
{
    class _3dMathHelper
    {
        //判断三点共线
        public static bool JudgeInLine(DVertex dv1, DVertex dv2, DVertex dv3)
        {
            return JudgeInLine(dv1.dx, dv1.dy, dv2.dx, dv2.dy, dv3.dx, dv3.dy);
        }
        public static bool JudgeInLine(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            int res = (x3 - x1) * (y2 - y1) - (x2 - x1)*(y3 - y1);
            if (res == 0)
                return true;
            return false;
        }
    }
}
