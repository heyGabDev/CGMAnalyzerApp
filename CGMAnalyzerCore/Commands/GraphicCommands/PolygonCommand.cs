using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    // <summary>
    /// POLYGON (case 7) - Dessine un polygone fermé rempli
    /// </summary>
    public class PolygonCommand : BaseCgmCommand
    {
        private readonly List<Point2D> _points = new List<Point2D>();

        public IReadOnlyList<Point2D> Points => _points;

        public PolygonCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[PolygonCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                // Contrôle du nombre de points
                int pointSize = argReader.SizeOfPoint();
                int maxPoints = Args.Length / pointSize;
                Debug.WriteLine($"[PolygonCommand] pointSize={pointSize}, maxPoints={maxPoints}");

                // Lire les points

                for (int i = 0; i < maxPoints; i++)
                {
                    Point2D point = argReader.MakePoint();
                    _points.Add(point);
                    Debug.WriteLine($"[PolygonCommand] Point {i + 1}: ({point.X}, {point.Y})");
                }
                Debug.WriteLine($"[PolygonCommand] Total points: {_points.Count}");
                ValidateArgumentsRead("PolygonCommand");

            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolygonCommand ERROR] {ex.Message}");
                _points.Clear();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (_points.Count == 0)
            {
                Debug.WriteLine("[PolygonCommand] Pas de points à dessiner");
                return;
            }

            if (_points.Count < 3)
            {
                Debug.WriteLine("[PolygonCommand] Pas assez de points pour un polygone (minimum 3)");
                return;
            }

            try
            {
                // Convertir en tableau de PointF pour Graphics
                var pointsArray = _points.Select(p => p.ToPointF()).ToArray();

                // Dessiner le contour du polygone
                g.DrawPolygon(pen, pointsArray);

                // Optionnel : remplir le polygone avec une couleur semi-transparente
                using var brush = new SolidBrush(Color.FromArgb(50, pen.Color));
                g.FillPolygon(brush, pointsArray);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolygonCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"POLYGON ({_points.Count} vertices)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
