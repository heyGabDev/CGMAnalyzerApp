using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class CircularArcCentreReversedCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; }
        public Point2D StartPoint { get; private set; }
        public Point2D EndPoint { get; private set; }
        public double Radius { get; private set; }

        public CircularArcCentreReversedCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length) { }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Radius <= 0) return;

            // Calcul des angles de début et fin (en degrés)
            var startAngle = Math.Atan2(StartPoint.Y - Center.Y, StartPoint.X - Center.X) * 180 / Math.PI;
            var endAngle = Math.Atan2(EndPoint.Y - Center.Y, EndPoint.X - Center.X) * 180 / Math.PI;

            // Calcul de l'angle de balayage (sens inverse = antihoraire)
            var sweepAngle = endAngle - startAngle;
            if (sweepAngle > 0) sweepAngle -= 360;
            if (sweepAngle < -180) sweepAngle += 360; // prendre le chemin inverse le plus court

            // Rectangle englobant
            var rect = new RectangleF(
                (float)(Center.X - Radius),
                (float)(Center.Y - Radius),
                (float)(Radius * 2),
                (float)(Radius * 2)
            );

            // Dessin
            g.DrawArc(pen, rect, (float)startAngle, (float)sweepAngle);
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => "CIRCULAR_ARC_CENTRE_REVERSED";
    }
}
