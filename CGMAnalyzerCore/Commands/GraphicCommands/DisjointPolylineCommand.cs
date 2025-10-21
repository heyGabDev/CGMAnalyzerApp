using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Render;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    // <summary>
    /// DISJOINT POLYLINE (case 2) - Dessine des segments de lignes séparés (paires de points)
    /// </summary>
    public class DisjointPolylineCommand : BaseCgmCommand
    {
        public readonly List<(Point2D Start, Point2D End)> Lines = new List<(Point2D Start, Point2D End)>();

        public DisjointPolylineCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[DisjointPolylineCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Lines = new List<(Point2D Start, Point2D End)>();
                var allPoints = new List<Point2D>();

                // Controle des nb points
                int pointSize = argReader.SizeOfPoint();
                int maxPoints = Args.Length / pointSize;
                Debug.WriteLine($"[DisjointPolyline] pointSize={pointSize}, maxPoints={maxPoints}");
                
                // Lire les points
                for (int i = 0; i < maxPoints; i++)
                {
                    Point2D point = argReader.MakePoint(); 
                    allPoints.Add(point);
                }

                // Vérifier qu'on a un nombre pair de points
                if (allPoints.Count % 2 != 0)
                {
                    Debug.WriteLine($"[DisjointPolyline WARNING] Nombre de points impair: {allPoints.Count}, ignoré le dernier");
                    allPoints.RemoveAt(allPoints.Count - 1); // Ignorer le dernier point
                }

                // Créer les lignes par paires de points
                for (int i = 0; i < allPoints.Count; i += 2)
                {
                    Lines.Add((allPoints[i], allPoints[i + 1]));
                    Debug.WriteLine($"[DisjointPolyline] Ligne {i / 2}: ({allPoints[i].X},{allPoints[i].Y}) -> ({allPoints[i + 1].X},{allPoints[i + 1].Y})");
                }

                Debug.WriteLine($"[DisjointPolyline] Total lignes: {Lines.Count}");
                ValidateArgumentsRead("DisjointPolylineCommand");
            }
            catch (Exception ex)
            {

                Debug.WriteLine($"[DisjointPolyline ERROR] {ex.Message}");
                Lines = new List<(Point2D Start, Point2D End)>(); 
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Lines == null || Lines.Count == 0)
            {
                Debug.WriteLine("[DisjointPolyline] Aucune ligne à dessiner");
                return;
            }

            Debug.WriteLine($"[DisjointPolyline] Dessin de {Lines.Count} lignes");

            try
            {
                foreach (var line in Lines)
                {
                    var start = new PointF((float)line.Start.X, (float)line.Start.Y);
                    var end = new PointF((float)line.End.X, (float)line.End.Y);
                    g.DrawLine(pen, start, end);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DisjointPolyline Draw ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"DISJOINT_POLYLINE: {Lines.Count} segments";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
