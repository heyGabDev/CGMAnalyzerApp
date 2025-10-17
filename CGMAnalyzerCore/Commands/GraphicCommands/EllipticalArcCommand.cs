using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class EllipticalArcCommand : BaseCgmCommand
    {
        public Point2D.Double Center => _center;
        public double StartAngle => _startAngle;
        public double ExtentAngle => _extentAngle;

        private readonly Point2D.Double _center;
        private readonly Point2D.Double _first;
        private readonly Point2D.Double _second;
        private readonly double _startAngle;
        private readonly double _extentAngle;

        public EllipticalArcCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EllipticalArcCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {            
                var argumentReader = new ExtractedArgumentReader(this);
                _center = argumentReader.MakePoint();
                _first = argumentReader.MakePoint();
                _second = argumentReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCommand] Read Center=({_center.X}, {_center.Y})");

                _startAngle = argumentReader.MakeVdc();
                _extentAngle = argumentReader.MakeVdc();
                Debug.WriteLine($"[EllipticalArcCommand] Read StartAngle={_startAngle:F2}, ExtentAngle={_extentAngle:F2}");
                ValidateArgumentsRead("EllipticalArcCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipticalArcCommand ERROR] {ex.Message}");
                _center = new Point2D.Double(0, 0);
                _first = new Point2D.Double(0, 0);
                _second = new Point2D.Double(0, 0);
                _startAngle = 0;
                _extentAngle = 0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            float rx = (float)Math.Abs(_first.X - _center.X);
            float ry = (float)Math.Abs(_second.Y - _center.Y);

            if (rx == 0 || ry == 0)
            {
                Debug.WriteLine("[EllipticalArcCommand] Rayons invalides, dessin ignoré");
                return;
            }

            try
            {
                float left = (float)_center.X - rx;
                float top = (float)_center.Y - ry;
                float width = 2 * rx;
                float height = 2 * ry;

                using GraphicsPath path = new GraphicsPath();
                path.AddArc(left, top, width, height, (float)_startAngle, (float)_extentAngle);
                g.DrawPath(pen, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipticalArcCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"ELLIPTICAL_ARC" +
                   $" center={_center} " +
                   $" startAngle={_startAngle:F2} " +
                   $" extentAngle={_extentAngle:F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
