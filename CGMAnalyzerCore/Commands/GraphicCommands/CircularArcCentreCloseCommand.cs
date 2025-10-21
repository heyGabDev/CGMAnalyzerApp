using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// CIRCULAR_ARC_CENTRE_CLOSE (case 16) - Arc circulaire fermé défini par centre, rayon et points
    /// </summary>
    public class CircularArcCentreCloseCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; } = new Point2D(0, 0);
        public Point2D StartPoint { get; private set; } = new Point2D(0, 0);
        public Point2D EndPoint { get; private set; } = new Point2D(0, 0);
        public double Radius { get; private set; } = 0;
        public int ClosureType { get; private set; } // 0=pie, 1=chord

        public CircularArcCentreCloseCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CircularArcCentreCloseCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                Center = argReader.MakePoint();
                Debug.WriteLine($"[CircularArcCentreCloseCommand] Read Center=({Center.X}, {Center.Y})");

                StartPoint = argReader.MakePoint();
                Debug.WriteLine($"[CircularArcCentreCloseCommand] Read StartPoint=({StartPoint.X}, {StartPoint.Y})");

                EndPoint = argReader.MakePoint();
                Debug.WriteLine($"[CircularArcCentreCloseCommand] Read Center=({Center.X}, {Center.Y})");

                Radius = argReader.MakeVdc();
                Debug.WriteLine($"[CircularArcCentreCloseCommand] Read Radius={Radius:F2}");

                ClosureType = argReader.MakeEnum();
                Debug.WriteLine($"[CircularArcCentreCloseCommand] Read ClosureType={ClosureType} (0=pie, 1=chord)");

                ValidateArgumentsRead("CircularArcCentreCloseCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CircularArcCentreCloseCommand ERROR] {ex.Message}");
                Center = new Point2D(0, 0);
                StartPoint = new Point2D(0, 0);
                EndPoint = new Point2D(0, 0);
                Radius = 0;
                ClosureType = 0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Center == null || StartPoint == null || EndPoint == null)
            {
                Debug.WriteLine("[CircularArcCentreCloseCommand] Points invalides, dessin ignoré");
                return;
            }

            if (Radius <= 0)
            {
                Debug.WriteLine("[CircularArcCentreCloseCommand] Rayon invalide, dessin ignoré");
                return;
            }

            Debug.WriteLine($"[CircularArcCentreCloseCommand] Dessin arc fermé ({(ClosureType == 0 ? "pie" : "chord")}) avec centre ({Center.X}, {Center.Y}), rayon {Radius:F2}");

            try
            {
                var startAngle = Math.Atan2(StartPoint.Y - Center.Y, StartPoint.X - Center.X) * 180 / Math.PI;
                var endAngle = Math.Atan2(EndPoint.Y - Center.Y, EndPoint.X - Center.X) * 180 / Math.PI;
                var sweepAngle = endAngle - startAngle;
                if (sweepAngle < 0) sweepAngle += 360;

                Debug.WriteLine($"[CircularArcCentreCloseCommand] Angles - Start: {startAngle:F2}°, End: {endAngle:F2}°, Sweep: {sweepAngle:F2}°");

                var rect = new RectangleF(
                    (float)(Center.X - Radius),
                    (float)(Center.Y - Radius),
                    (float)(Radius * 2),
                    (float)(Radius * 2)
                );

                using var path = new GraphicsPath();
                path.AddArc(rect, (float)startAngle, (float)sweepAngle);

                if (ClosureType == 0) // Pie
                {
                    path.AddLine(path.GetLastPoint(), new PointF((float)Center.X, (float)Center.Y));
                }
                // Si chord, CloseFigure() ajoute automatiquement la ligne droite

                path.CloseFigure();
                g.DrawPath(pen, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CircularArcCentreCloseCommand Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"CIRCULAR_ARC_CENTRE_CLOSE ({(ClosureType == 0 ? "pie" : "chord")})";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
