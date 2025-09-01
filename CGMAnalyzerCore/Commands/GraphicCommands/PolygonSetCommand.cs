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
    public class PolygonSetCommand : BaseCgmCommand
    {
        private readonly List<List<Point2D>> _polygons = new();

        public IReadOnlyList<List<Point2D>> Polygons => _polygons;

        public PolygonSetCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Un PolygonSet contient plusieurs polygons avec des indicateurs de fermeture
            int totalArgs = command.Args.Length;
            int currentArg = 0;

            var currentPolygon = new List<Point2D>();

            while (currentArg < totalArgs - argReader.SizeOfPoint())
            {
                var point = argReader.MakePoint(ec, eid);
                currentPolygon.Add(point);

                // Vérifier s'il y a un indicateur de fermeture/nouveau polygone
                // (Implémentation simplifiée - peut nécessiter ajustements selon format exact)
                currentArg += argReader.SizeOfPoint();

                if (currentPolygon.Count > 2) // Au moins 3 points pour un polygone
                {
                    _polygons.Add(new List<Point2D>(currentPolygon));
                    currentPolygon.Clear();
                }
            }

            if (currentPolygon.Count > 2)
                _polygons.Add(currentPolygon);
        }

        public override void Draw(Graphics g, Pen pen)
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
            }
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"POLYGON_SET ({_polygons.Count} polygons)";
    }
}
