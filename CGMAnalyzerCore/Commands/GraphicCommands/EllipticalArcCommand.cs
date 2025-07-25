using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class EllipticalArcCommand : BaseCgmCommand
    {
        private readonly Point2D.Double _center;
        private readonly Point2D.Double _first;
        private readonly Point2D.Double _second;
        private readonly double _startAngle;
        private readonly double _extentAngle;

        public EllipticalArcCommand(int ec, int eid, CgmCommand command, CgmArgumentReader argumentReader)
            : base(ec, eid, command.Length)
        {
            _center = argumentReader.MakePoint(ec, eid);
            _first = argumentReader.MakePoint(ec, eid);
            _second = argumentReader.MakePoint(ec, eid);

            _startAngle = argumentReader.MakeVdc();
            _extentAngle = argumentReader.MakeVdc();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            float rx = (float)Math.Abs(_first.X - _center.X);
            float ry = (float)Math.Abs(_second.Y - _center.Y);

            if (rx == 0 || ry == 0)
                return;

            float left = (float)_center.X - rx;
            float top = (float)_center.Y - ry;
            float width = 2 * rx;
            float height = 2 * ry;

            using GraphicsPath path = new GraphicsPath();
            path.AddArc(left, top, width, height, (float)_startAngle, (float)_extentAngle);
            g.DrawPath(pen, path);
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException();
        }
    }
}
