using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
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
        public Point2D Center { get; private set; }
        public Point2D StartPoint { get; private set; }
        public Point2D EndPoint { get; private set; }
        public double Radius { get; private set; }

        public CircularArcCentreCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Center = argReader.MakePoint(ec, eid);
            StartPoint = argReader.MakePoint(ec, eid);
            EndPoint = argReader.MakePoint(ec, eid);
            Radius = argReader.MakeVdc();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Radius <= 0) return;

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

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"CIRCULAR_ARC_CENTRE center={Center} radius={Radius:F2}";
        }
    }
}
