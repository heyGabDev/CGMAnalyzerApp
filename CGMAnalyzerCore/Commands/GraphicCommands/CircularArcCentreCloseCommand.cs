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
    public class CircularArcCentreCloseCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; }
        public Point2D StartPoint { get; private set; }
        public Point2D EndPoint { get; private set; }
        public double Radius { get; private set; }
        public int ClosureType { get; private set; }

        public CircularArcCentreCloseCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Center = argReader.MakePoint(ec, eid);
            StartPoint = argReader.MakePoint(ec, eid);
            EndPoint = argReader.MakePoint(ec, eid);
            Radius = argReader.MakeVdc();
            ClosureType = argReader.MakeEnum();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Radius <= 0) return;

            var startAngle = Math.Atan2(StartPoint.Y - Center.Y, StartPoint.X - Center.X) * 180 / Math.PI;
            var endAngle = Math.Atan2(EndPoint.Y - Center.Y, EndPoint.X - Center.X) * 180 / Math.PI;
            var sweepAngle = endAngle - startAngle;
            if (sweepAngle < 0) sweepAngle += 360;

            var rect = new RectangleF((float)(Center.X - Radius), (float)(Center.Y - Radius),
                                     (float)(Radius * 2), (float)(Radius * 2));

            using var path = new GraphicsPath();
            path.AddArc(rect, (float)startAngle, (float)sweepAngle);

            if (ClosureType == 0) // Pie
            {
                path.AddLine(path.GetLastPoint(), new PointF((float)Center.X, (float)Center.Y));
            }
            path.CloseFigure();

            g.DrawPath(pen, path);
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"CIRCULAR_ARC_CENTRE_CLOSE ({(ClosureType == 0 ? "pie" : "chord")})";
    }
}
