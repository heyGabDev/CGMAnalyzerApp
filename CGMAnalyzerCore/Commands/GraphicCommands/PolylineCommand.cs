using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// POLYMARKER (case 1) - Dessine une ligne continue passant par plusieurs points
    /// </summary>
    public class PolylineCommand : BaseCgmCommand
    {
        public List<Point2D.Double> Points { get; private set; } = new List<Point2D.Double>();

        public PolylineCommand(int ec, int eid, int l, CgmCommand command)
             : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[PolylineCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Controle des nb points
                int pointSize = argReader.SizeOfPoint();
                int maxPoints = Args.Length / pointSize;
                Debug.WriteLine($"[PolylineCommand] Taille point: {pointSize} octets, Max points: {maxPoints}");

                // Lire les points
                for (int i = 0; i < maxPoints; i++)
                {
                    Point2D.Double point = argReader.MakePoint();
                    Points.Add(point);
                    Debug.WriteLine($"[PolylineCommand] Point {i + 1}: ({point.X}, {point.Y})");
                }

                Debug.WriteLine($"[PolylineCommand] Total points: {Points.Count}");
                ValidateArgumentsRead("PolylineCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolylineCommand ERROR] {ex.Message}");
                Points = new List<Point2D.Double>();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Points == null || Points.Count < 2)
            {
                Debug.WriteLine("[PolylineCommand] Pas assez de points pour dessiner");
                return;
            }

            try
            {
                for (int i = 0; i < Points.Count - 1; i++)
                {
                    var p1 = Points[i];
                    var p2 = Points[i + 1];

                    // Conversion Point2D → PointF
                    var point1 = new PointF((float)p1.X, (float)p1.Y);
                    var point2 = new PointF((float)p2.X, (float)p2.Y);

                    g.DrawLine(pen, point1, point2);
                }

                Debug.WriteLine($"[PolylineCommand] Dessiné {Points.Count} points");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolylineCommand Draw ERROR] {ex.Message}");
            }
        }

        public override string ToString()
        {
            return $"POLYLINE : {Points.Count} points";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
