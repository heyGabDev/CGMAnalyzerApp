using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Geometry
{
    public class Rectangle2D
    {
        public class Double
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Width { get; set; }
            public double Height { get; set; }

            public Double(double x, double y, double width, double height)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            public override string ToString()
            {
                return $"Rectangle2D.Double[x={X}, y={Y}, width={Width}, height={Height}]";
            }
        }
    }
}
