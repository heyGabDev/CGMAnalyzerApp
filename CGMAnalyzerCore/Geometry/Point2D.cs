using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Geometry
{
    public abstract class Point2D : ICloneable
    {
        public abstract double X { get; }
        public abstract double Y { get; }

        public abstract void SetLocation(double x, double y);

        public void SetLocation(Point2D p)
        {
            SetLocation(p.X, p.Y);
        }

        public static double DistanceSq(double x1, double y1, double x2, double y2)
        {
            double dx = x1 - x2;
            double dy = y1 - y2;
            return dx * dx + dy * dy;
        }

        public static double Distance(double x1, double y1, double x2, double y2)
        {
            return Math.Sqrt(DistanceSq(x1, y1, x2, y2));
        }

        public double DistanceSq(double px, double py)
        {
            return DistanceSq(this.X, this.Y, px, py);
        }

        public double DistanceSq(Point2D pt)
        {
            return DistanceSq(this.X, this.Y, pt.X, pt.Y);
        }

        public double Distance(double px, double py)
        {
            return Math.Sqrt(DistanceSq(px, py));
        }

        public double Distance(Point2D pt)
        {
            return Math.Sqrt(DistanceSq(pt));
        }

        public override int GetHashCode()
        {
            long bits = BitConverter.DoubleToInt64Bits(X);
            bits ^= BitConverter.DoubleToInt64Bits(Y) * 31;
            return (int)(bits ^ (bits >> 32));
        }

        public override bool Equals(object obj)
        {
            return obj is Point2D other &&
                   X == other.X &&
                   Y == other.Y;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public class Float : Point2D
        {
            public float XValue { get; set; }
            public float YValue { get; set; }

            public Float() { }

            public Float(float x, float y)
            {
                XValue = x;
                YValue = y;
            }

            public override double X => XValue;
            public override double Y => YValue;

            public override void SetLocation(double x, double y)
            {
                XValue = (float)x;
                YValue = (float)y;
            }

            public void SetLocation(float x, float y)
            {
                XValue = x;
                YValue = y;
            }

            public override string ToString()
            {
                return $"Point2D.Float[{XValue}, {YValue}]";
            }
        }

        public class Double : Point2D
        {
            public double XValue { get; set; }
            public double YValue { get; set; }

            public Double() { }

            public Double(double x, double y)
            {
                XValue = x;
                YValue = y;
            }

            public override double X => XValue;
            public override double Y => YValue;

            public override void SetLocation(double x, double y)
            {
                XValue = x;
                YValue = y;
            }

            public override string ToString()
            {
                return $"Point2D.Double[{XValue}, {YValue}]";
            }
        }
    }
}
