using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// POLYMARKER (case 3) - Dessine des marqueurs à des positions spécifiées
    /// </summary>
    public class PolyMarkerCommand : BaseCgmCommand
    {
        private readonly List<Point2D> _points = new();

        public IReadOnlyList<Point2D> Points => _points;

        public PolyMarkerCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Calculer le nombre de points à partir de la taille des arguments
            int pointCount = command.Args.Length / argReader.SizeOfPoint();

            for (int i = 0; i < pointCount; i++)
            {
                Point2D point = argReader.MakePoint(ec, eid);
                _points.Add(point);
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Dessiner des petits cercles comme marqueurs (taille par défaut 3x3)
            const float markerSize = 3.0f;
            using var brush = new SolidBrush(pen.Color);

            foreach (var point in _points)
            {
                var pointF = point.ToPointF();
                g.FillEllipse(brush,
                    pointF.X - markerSize / 2,
                    pointF.Y - markerSize / 2,
                    markerSize,
                    markerSize);
            }
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"POLYMARKER ({_points.Count} markers)";
        }
    }
    
}
