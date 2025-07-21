using CGMAnalyzerCore.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Helper
{
    public static class Point2DExtensions
    {
        public static PointF ToPointF(this Point2D point)
        {
            return new PointF((float)point.X, (float)point.Y);
        }
    }
}
