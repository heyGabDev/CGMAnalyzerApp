using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Render;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class DisjointPolylineCommand : BaseCgmCommand
    {
        private readonly List<(Point2D Start, Point2D End)> _lines = new();

        public IReadOnlyList<(Point2D Start, Point2D End)> Lines => _lines;

        public DisjointPolylineCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argRerader)
            : base(command.ElementClass, command.ElementId, command.Length)
        {

            int pointCount = command.Args.Length / argRerader.SizeOfPoint(); // SizeOfPoint();
            if (pointCount % 2 != 0)
                throw new InvalidDataException("DisjointPolyline must have an even number of points");

            for (int i = 0; i < pointCount / 2; i++)
            {
                Point2D p1 = argRerader.MakePoint(command.ElementClass, command.ElementId);
                Point2D p2 = argRerader.MakePoint(command.ElementClass, command.ElementId);
                _lines.Add((p1, p2));
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            foreach (var line in _lines)
            {
                g.DrawLine(pen,
                    line.Start.ToPointF(),
                    line.End.ToPointF());
                //g.DrawLine(pen,
                            //new PointF((float)line.Start.X, (float)line.Start.Y),
                            //new PointF((float)line.End.X, (float)line.End.Y));
            }
        }

        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("DisjointPolyline [");
            foreach (var line in _lines)
            {
                sb.AppendFormat("({0},{1},{2},{3})", line.Start.X, line.Start.Y, line.End.X, line.End.Y);
            }
            sb.Append("]");
            return sb.ToString();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
