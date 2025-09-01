using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class CircularArc3PointCloseCommand : BaseCgmCommand
    {
        public Point2D StartPoint { get; private set; }
        public Point2D IntermediatePoint { get; private set; }
        public Point2D EndPoint { get; private set; }
        public int ClosureType { get; private set; } // 0=pie, 1=chord

        public CircularArc3PointCloseCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            StartPoint = argReader.MakePoint(ec, eid);
            IntermediatePoint = argReader.MakePoint(ec, eid);
            EndPoint = argReader.MakePoint(ec, eid);
            ClosureType = argReader.MakeEnum();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Utiliser la même logique que CircularArc3PointCommand mais fermer la forme
            var center = CalculateCircleCenter(StartPoint, IntermediatePoint, EndPoint);
            if (center == null) return;

            var radius = Math.Sqrt(Math.Pow(center.X - StartPoint.X, 2) + Math.Pow(center.Y - StartPoint.Y, 2));
            var startAngle = Math.Atan2(StartPoint.Y - center.Y, StartPoint.X - center.X) * 180 / Math.PI;
            var endAngle = Math.Atan2(EndPoint.Y - center.Y, EndPoint.X - center.X) * 180 / Math.PI;
            var sweepAngle = endAngle - startAngle;

            var rect = new RectangleF((float)(center.X - radius), (float)(center.Y - radius),
                                     (float)(radius * 2), (float)(radius * 2));

            using var path = new GraphicsPath();
            path.AddArc(rect, (float)startAngle, (float)sweepAngle);

            if (ClosureType == 0) // Pie (secteur)
            {
                path.AddLine(path.GetLastPoint(), new PointF((float)center.X, (float)center.Y));
            }
            path.CloseFigure();

            g.DrawPath(pen, path);
        }

        private Point2D? CalculateCircleCenter(Point2D p1, Point2D p2, Point2D p3)
        {
            var d = 2 * (p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y));
            if (Math.Abs(d) < 0.0001) return null;

            var ux = ((p1.X * p1.X + p1.Y * p1.Y) * (p2.Y - p3.Y) +
                      (p2.X * p2.X + p2.Y * p2.Y) * (p3.Y - p1.Y) +
                      (p3.X * p3.X + p3.Y * p3.Y) * (p1.Y - p2.Y)) / d;

            var uy = ((p1.X * p1.X + p1.Y * p1.Y) * (p3.X - p2.X) +
                      (p2.X * p2.X + p2.Y * p2.Y) * (p1.X - p3.X) +
                      (p3.X * p3.X + p3.Y * p3.Y) * (p2.X - p1.X)) / d;

            return new Point2D(ux, uy);
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"CIRCULAR_ARC_3_POINT_CLOSE ({(ClosureType == 0 ? "pie" : "chord")})";
    }
}
