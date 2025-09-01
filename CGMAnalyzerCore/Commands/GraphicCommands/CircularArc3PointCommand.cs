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
    /// CIRCULAR_ARC_3_POINT (case 13) - Arc défini par 3 points
    /// </summary>
    public class CircularArc3PointCommand : BaseCgmCommand
    {
        public Point2D StartPoint { get; private set; }
        public Point2D IntermediatePoint { get; private set; }
        public Point2D EndPoint { get; private set; }

        public CircularArc3PointCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            StartPoint = argReader.MakePoint(ec, eid);
            IntermediatePoint = argReader.MakePoint(ec, eid);
            EndPoint = argReader.MakePoint(ec, eid);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Calculer le centre et le rayon de l'arc à partir des 3 points
            var center = CalculateCircleCenter(StartPoint, IntermediatePoint, EndPoint);
            if (center == null) return; // Points colinéaires

            var radius = Math.Sqrt(Math.Pow(center.X - StartPoint.X, 2) + Math.Pow(center.Y - StartPoint.Y, 2));

            // Calculer les angles
            
            var startAngle = CalculateAngle(center.X, center.Y, StartPoint);
            var endAngle = CalculateAngle(center.X, center.Y, EndPoint);

            // S'assurer que l'arc passe par le point intermédiaire
            var sweepAngle = CalculateSweepAngle(startAngle, endAngle, CalculateAngle(center.X, center.Y, IntermediatePoint));

            // Dessiner l'arc
            var rect = new RectangleF(
                (float)(center.X - radius),
                (float)(center.Y - radius),
                (float)(radius * 2),
                (float)(radius * 2)
            );

            g.DrawArc(pen, rect, (float)startAngle, (float)sweepAngle);
        }

        private Point2D? CalculateCircleCenter(Point2D p1, Point2D p2, Point2D p3)
        {
            // Algorithme pour calculer le centre d'un cercle passant par 3 points
            var d = 2 * (p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y));

            if (Math.Abs(d) < 0.0001) return null; // Points colinéaires

            var ux = ((p1.X * p1.X + p1.Y * p1.Y) * (p2.Y - p3.Y) +
                      (p2.X * p2.X + p2.Y * p2.Y) * (p3.Y - p1.Y) +
                      (p3.X * p3.X + p3.Y * p3.Y) * (p1.Y - p2.Y)) / d;

            var uy = ((p1.X * p1.X + p1.Y * p1.Y) * (p3.X - p2.X) +
                      (p2.X * p2.X + p2.Y * p2.Y) * (p1.X - p3.X) +
                      (p3.X * p3.X + p3.Y * p3.Y) * (p2.X - p1.X)) / d;

            return new Point2D(ux, uy);
        }

        private double CalculateAngle(double X, double Y, Point2D point)
        {
            return Math.Atan2(point.Y - Y, point.X - X) * 180 / Math.PI;
        }

        private double CalculateSweepAngle(double startAngle, double endAngle, double intermediateAngle)
        {
            // Calculer l'angle de balayage en s'assurant de passer par le point intermédiaire
            var sweep = endAngle - startAngle;

            // Normaliser les angles
            while (sweep > 360) sweep -= 360;
            while (sweep < -360) sweep += 360;

            // Vérifier si l'angle intermédiaire est dans la bonne direction
            var intermediateFromStart = intermediateAngle - startAngle;
            while (intermediateFromStart > 360) intermediateFromStart -= 360;
            while (intermediateFromStart < -360) intermediateFromStart += 360;

            if (Math.Sign(sweep) != Math.Sign(intermediateFromStart) && Math.Abs(sweep) > 180)
            {
                sweep = sweep > 0 ? sweep - 360 : sweep + 360;
            }

            return sweep;
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"CIRCULAR_ARC_3_POINT {StartPoint} -> {IntermediatePoint} -> {EndPoint}";
        }
    }
}
