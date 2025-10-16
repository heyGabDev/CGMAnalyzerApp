using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using CGMAnalyzerCore.Render;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    // <summary>
    /// DISJOINT POLYLINE (case 2) - Dessine des segments de lignes séparés (paires de points)
    /// </summary>
    public class DisjointPolylineCommand : BaseCgmCommand
    {
        private readonly List<(Point2D Start, Point2D End)> _lines = new();

        public IReadOnlyList<(Point2D Start, Point2D End)> Lines => _lines;

        public DisjointPolylineCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Args = command.Args;
            command.ResetPosition();

            int pointSize = argReader.SizeOfPoint();
            int availableBytes = command.RemainingArgs();
            int pointCount = availableBytes / pointSize;

            if (pointCount % 2 != 0) // Controle pair avant lecture
            {
                Debug.WriteLine($"[DisjointPolyline WARNING] Nombre de points impair: {pointCount}, ignoré le dernier");
                pointCount = (pointCount / 2) * 2; // Arrondir au nombre pair inférieur
            }

            // Lire les paires de points
            for (int i = 0; i < pointCount / 2; i++)
            {
                Point2D start = argReader.MakePoint(ec, eid);
                Point2D end = argReader.MakePoint(ec, eid);
                _lines.Add((start, end));

                Debug.WriteLine($"[DisjointPolyline] Ligne {i}: ({start.X},{start.Y}) -> ({end.X},{end.Y})");
            }

            ValidateArgumentsRead("DisjointPolylineCommand");

        }

        public override void Draw(Graphics g, Pen pen)
        {
            Debug.WriteLine($"[DisjointPolyline] Dessin de {_lines.Count} lignes"); 

            foreach (var line in _lines)
            {
                g.DrawLine(pen,
                    line.Start.ToPointF(),
                    line.End.ToPointF());
            }
        }

        public override string ToString()
        {
            return $"DisjointPolyline [{_lines.Count} segments]";
            //var sb = new System.Text.StringBuilder();
            //sb.Append("DisjointPolyline [");
            //foreach (var line in _lines)
            //{
            //    sb.AppendFormat("({0},{1},{2},{3})", line.Start.X, line.Start.Y, line.End.X, line.End.Y);
            //}
            //sb.Append("]");
            //return sb.ToString();
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
