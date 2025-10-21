using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class EllipticalArcCommand : BaseCgmCommand
    {
        /// <summary>
        /// ELLIPTICAL_ARC (case 18) - Arc elliptique
        /// </summary>
        public Point2D.Double Center { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double First { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double Second { get; private set; } = new Point2D.Double(0, 0);
        public double StartAngle { get; private set; } = 0;
        public double ExtentAngle { get; private set; } = 0;

        public EllipticalArcCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EllipticalArcCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {            
                var argReader = new ExtractedArgumentReader(this);

                Center = argReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCommand] Center: ({Center.X}, {Center.Y})");

                First = argReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCommand] First: ({First.X}, {First.Y})");

                Second = argReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCommand] Second: ({Second.X}, {Second.Y})");

                StartAngle = argReader.MakeVdc();
                Debug.WriteLine($"[EllipticalArcCommand] StartAngle: {StartAngle:F2}");

                ExtentAngle = argReader.MakeVdc();
                Debug.WriteLine($"[EllipticalArcCommand] ExtentAngle: {ExtentAngle:F2}");
                ValidateArgumentsRead("EllipticalArcCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipticalArcCommand ERROR] {ex.Message}");
                Center = new Point2D.Double(0, 0);
                First = new Point2D.Double(0, 0);
                Second = new Point2D.Double(0, 0);
                StartAngle = 0;
                ExtentAngle = 0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            float rx = (float)Math.Sqrt(
                Math.Pow(First.X - Center.X, 2) +
                Math.Pow(First.Y - Center.Y, 2)
            );
            float ry = (float)Math.Sqrt(
                Math.Pow(Second.X - Center.X, 2) +
                Math.Pow(Second.Y - Center.Y, 2)
            );

            if (rx <= 0 || ry <= 0)
            {
                Debug.WriteLine($"[EllipticalArcCommand] Rayons invalides (rx={rx:F2}, ry={ry:F2}), dessin ignoré");
                return;
            }

            Debug.WriteLine($"[EllipticalArcCommand] Dessin arc elliptique centre ({Center.X}, {Center.Y}), rx={rx:F2}, ry={ry:F2}, angles {StartAngle:F2}° à {ExtentAngle:F2}°");

            try
            {
                float left = (float)Center.X - rx;
                float top = (float)Center.Y - ry;
                float width = 2 * rx;
                float height = 2 * ry;

                using GraphicsPath path = new GraphicsPath();
                path.AddArc(left, top, width, height, (float)StartAngle, (float)ExtentAngle);
                g.DrawPath(pen, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipticalArcCommand Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"ELLIPTICAL_ARC center=({Center.X}, {Center.Y}) " +
                   $"startAngle={StartAngle:F2} extentAngle={ExtentAngle:F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
