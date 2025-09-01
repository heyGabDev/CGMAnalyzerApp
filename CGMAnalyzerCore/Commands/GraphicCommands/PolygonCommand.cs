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
    // <summary>
    /// POLYGON (case 7) - Dessine un polygone fermé rempli
    /// </summary>
    public class PolygonCommand : BaseCgmCommand
    {
        private readonly List<Point2D> _points = new();

        public IReadOnlyList<Point2D> Points => _points;

        public PolygonCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Calculer le nombre de points
            int pointCount = command.Args.Length / argReader.SizeOfPoint();

            for (int i = 0; i < pointCount; i++)
            {
                Point2D point = argReader.MakePoint(ec, eid);
                _points.Add(point);
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (_points.Count < 3) return; // Un polygone a besoin d'au moins 3 points

            // Convertir en tableau de PointF pour Graphics
            var pointsArray = _points.Select(p => p.ToPointF()).ToArray();

            // Dessiner le contour du polygone
            g.DrawPolygon(pen, pointsArray);

            // Optionnel : remplir le polygone avec une couleur semi-transparente
            using var brush = new SolidBrush(Color.FromArgb(50, pen.Color));
            g.FillPolygon(brush, pointsArray);
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"POLYGON ({_points.Count} vertices)";
        }

        /// <summary>
        /// Méthode utilitaire pour obtenir les limites du polygone (pour debug/analyse)
        /// </summary>
        public Rectangle GetBounds()
        {
            if (!_points.Any()) return Rectangle.Empty;

            var minX = _points.Min(p => p.X);
            var minY = _points.Min(p => p.Y);
            var maxX = _points.Max(p => p.X);
            var maxY = _points.Max(p => p.Y);

            return new Rectangle(
                (int)minX, (int)minY,
                (int)(maxX - minX), (int)(maxY - minY)
            );
        }
    }
}
