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

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// POLYMARKER (case 3) - Dessine des marqueurs à des positions spécifiées
    /// </summary>
    public class PolyMarkerCommand : BaseCgmCommand
    {
        private readonly List<Point2D> _points = new();

        public IReadOnlyList<Point2D> Points => _points;

        public PolyMarkerCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[PolyMarkerCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Contrôle du nombre de points (pattern cohérent)
                int pointSize = argReader.SizeOfPoint();
                int maxPoints = Args.Length / pointSize;
                Debug.WriteLine($"[PolyMarkerCommand] pointSize={pointSize}, maxPoints={maxPoints}");

                // Lecture des points
                for (int i = 0; i < maxPoints; i++)
                {
                    Point2D point = argReader.MakePoint();
                    _points.Add(point);
                    Debug.WriteLine($"[PolyMarkerCommand] Point {i + 1}: ({point.X}, {point.Y})");
                }

                Debug.WriteLine($"[PolyMarkerCommand] Total points: {_points.Count}");
                ValidateArgumentsRead("PolyMarkerCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolyMarkerCommand ERROR] {ex.Message}");
                _points.Clear();
                HasReadErrors = true;
            } 
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (_points == null || _points.Count == 0)
            {
                Debug.WriteLine("[PolyMarkerCommand] Aucun point à dessiner");
                return;
            }

            Debug.WriteLine($"[PolyMarkerCommand] Dessin de {_points.Count} marqueurs");

            try
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolyMarkerCommand Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"POLYMARKER ({_points.Count} markers)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }   
}
