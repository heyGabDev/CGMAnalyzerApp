using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    // <summary>
    /// CIRCULAR_ARC_CENTRE (case 15) - Arc défini par centre, 2 points et rayon
    /// </summary>
    public class CircularArcCentreCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; } = new Point2D(0, 0);
        public Point2D StartPoint { get; private set; } = new Point2D(0, 0);
        public Point2D EndPoint { get; private set; } = new Point2D(0, 0);
        public double Radius { get; private set; } = 0;

        public CircularArcCentreCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CircularArcCentreCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Center = argReader.MakePoint();
                Debug.WriteLine($"[CircularArcCentreCommand] Read Center=({Center.X}, {Center.Y})");

                StartPoint = argReader.MakePoint();
                Debug.WriteLine($"[CircularArcCentreCommand] Read StartPoint=({StartPoint.X}, {StartPoint.Y})");

                EndPoint = argReader.MakePoint();
                Debug.WriteLine($"[CircularArcCentreCommand] Read EndPoint=({EndPoint.X}, {EndPoint.Y})");

                Radius = argReader.MakeVdc();
                Debug.WriteLine($"[CircularArcCentreCommand] Read Radius={Radius:F2}");
                ValidateArgumentsRead("CircularArcCentreCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CircularArcCentreCommand ERROR] {ex.Message}");
                Center = new Point2D(0, 0);
                StartPoint = new Point2D(0, 0);
                EndPoint = new Point2D(0, 0);
                Radius = 0;
                HasReadErrors = true;
            }
            
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if(Radius <= 0)
            {
                Debug.WriteLine("[CircularArcCentreCommand] Rayon invalide, dessin ignoré");
                return;
            }

            try
            {
                // Calculer les angles de début et fin
                var startAngle = Math.Atan2(StartPoint.Y - Center.Y, StartPoint.X - Center.X) * 180 / Math.PI;
                var endAngle = Math.Atan2(EndPoint.Y - Center.Y, EndPoint.X - Center.X) * 180 / Math.PI;
               
                // Calculer l'angle de balayage
                var sweepAngle = endAngle - startAngle;
                if (sweepAngle < 0) sweepAngle += 360;
                if (sweepAngle > 180) sweepAngle -= 360; // Prendre le chemin le plus court

                // Rectangle englobant
                var rect = new RectangleF(
                    (float)(Center.X - Radius),
                    (float)(Center.Y - Radius),
                    (float)(Radius * 2),
                    (float)(Radius * 2)
                );

                g.DrawArc(pen, rect, (float)startAngle, (float)sweepAngle);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CircularArcCentreCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"CIRCULAR_ARC_CENTRE center={Center} radius={Radius:F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
