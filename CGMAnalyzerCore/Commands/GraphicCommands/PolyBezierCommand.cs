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
    public class PolyBezierCommand :BaseCgmCommand
    {
        private readonly List<Point2D> _controlPoints = new();

        public IReadOnlyList<Point2D> ControlPoints => _controlPoints;

        public PolyBezierCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            int pointCount = command.Args.Length / argReader.SizeOfPoint();
            for (int i = 0; i < pointCount; i++)
            {
                _controlPoints.Add(argReader.MakePoint(ec, eid));
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (_controlPoints.Count < 4) return;

            // Dessiner des courbes de Bézier par groupes de 4 points
            for (int i = 0; i <= _controlPoints.Count - 4; i += 3)
            {
                if (i + 3 < _controlPoints.Count)
                {
                    var points = _controlPoints.Skip(i).Take(4).Select(p => p.ToPointF()).ToArray();
                    g.DrawBezier(pen, points[0], points[1], points[2], points[3]);
                }
            }
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => $"POLYBEZIER ({_controlPoints.Count} control points)";
    }

    // Commandes complexes avec implémentation basique
    public class NonUniformBSplineCommand : BaseCgmCommand
    {
        public NonUniformBSplineCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length) { }

        public override void Draw(Graphics g, Pen pen)
        {
            // Implémentation simplifiée - dessiner une ligne brisée
            // Une vraie B-spline nécessiterait un algorithme complexe
        }

        public override void ReadArguments(BinaryReader reader)
            => throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");

        public override string ToString() => "NON_UNIFORM_B_SPLINE (simplified)";
    }
}
