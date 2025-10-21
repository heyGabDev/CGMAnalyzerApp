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
    /// POLYGON_SET (case 8) - Dessine un ensemble de polygones
    /// </summary>
    public class PolygonSetCommand : BaseCgmCommand
    {
        private readonly List<List<Point2D>> _polygons = new List<List<Point2D>>(); 

        public IReadOnlyList<List<Point2D>> Polygons => _polygons;

        public PolygonSetCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[PolygonSetCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Contrôle du nombre de points
                int pointSize = argReader.SizeOfPoint();
                int maxPoints = Args.Length / pointSize;
                Debug.WriteLine($"[PolygonSetCommand] pointSize={pointSize}, maxPoints={maxPoints}");

                // OPTION SIMPLE : Tous les points forment UN polygone
                // (À adapter si votre format CGM a des flags de séparation)
                var currentPolygon = new List<Point2D>();

                for (int i = 0; i < maxPoints; i++)
                {
                    Point2D point = argReader.MakePoint();
                    currentPolygon.Add(point);
                    Debug.WriteLine($"[PolygonSetCommand] Point {i+1}: ({point.X}, {point.Y})");
                }

                if (currentPolygon.Count >= 3)
                {
                    _polygons.Add(currentPolygon);
                    Debug.WriteLine($"[PolygonSetCommand] Polygone créé avec {currentPolygon.Count} points");
                }
                else
                {
                    Debug.WriteLine($"[PolygonSetCommand WARNING] Pas assez de points pour un polygone: {currentPolygon.Count}");
                }

                Debug.WriteLine($"[PolygonSetCommand] Total polygons: {_polygons.Count}");
                ValidateArgumentsRead("PolygonSetCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolygonSetCommand ERROR] {ex.Message}");
                _polygons.Clear();
                HasReadErrors = true;   
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (_polygons == null || _polygons.Count == 0)
            {
                Debug.WriteLine("[PolygonSetCommand] Pas de polygones à dessiner");
                return;
            }

            try
            {
                foreach (var polygon in _polygons)
                {
                    if (polygon.Count >= 3)
                    {
                        var points = polygon.Select(p => p.ToPointF()).ToArray();
                        g.DrawPolygon(pen, points);

                        using var brush = new SolidBrush(Color.FromArgb(30, pen.Color));
                        g.FillPolygon(brush, points);
                    }
                    Debug.WriteLine($"[PolygonSetCommand] Polygone dessiné avec {polygon.Count} points");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolygonSetCommand Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            int totalPoints = _polygons.Sum(p => p.Count);
            return $"POLYGON_SET ({_polygons.Count} polygons, {totalPoints} total points)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
