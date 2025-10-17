using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Helper;
using CGMAnalyzerCore.Parser;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class PolyBezierCommand :BaseCgmCommand
    {
        private readonly List<Point2D> _controlPoints = new List<Point2D>();

        public IReadOnlyList<Point2D> ControlPoints => _controlPoints;

        public PolyBezierCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[PolyBezierCommand] ArgsLength={Args?.Length ?? 0}");
           

            try
            {
                var argReader = new ExtractedArgumentReader(this);            
                int pointSize = argReader.SizeOfPoint();
                int pointCount = command.Args.Length / pointSize;

                for (int i = 0; i < pointCount; i++)
                {
                    if (!command.HasMoreArgs()) // Controle des dépassements
                    {
                        Console.WriteLine("[CGM] Tentative d'accès hors des bornes évitée dans PolyBezierCommand.");
                        break;
                    }

                    _controlPoints.Add(argReader.MakePoint());
                    Debug.WriteLine($"[PolyBezierCommand] Control Point {i + 1}: ({_controlPoints[i].X}, {_controlPoints[i].Y})");
                }
                ValidateArgumentsRead("PolyBezierCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolyBezierCommand ERROR] {ex.Message}");
                _controlPoints.Clear();
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (_controlPoints == null || _controlPoints.Count == 0)
            {
                Debug.WriteLine("[PolyBezierCommand] Aucun point de contrôle, dessin ignoré");
                return;
            }

            if (_controlPoints.Count < 4)
            {
                Debug.WriteLine("[PolyBezierCommand] Pas assez de points de contrôle pour une courbe de Bézier (minimum 4)");
                return;
            }

            try
            {
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
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolyBezierCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"POLYBEZIER ({_controlPoints.Count} control points)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

    // Commandes complexes avec implémentation basique
    public class NonUniformBSplineCommand : BaseCgmCommand
    {
        public NonUniformBSplineCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l) 
        { 
            Args = command.Args;
            Debug.WriteLine($"[NonUniformBSplineCommand] ArgsLength={Args?.Length ?? 0}");
            // Implémentation basique - ne lit pas les arguments

            try
            {
                var argReader = new ExtractedArgumentReader(this);
                // Consommation explicite
                int remaining = this.RemainingArgs();
                for (int i = 0; i < remaining; i++)
                {
                    argReader.MakeUInt8(); // Jeter les données
                }
                Debug.WriteLine("[NonUniformBSplineCommand] Commande non supportée - arguments ignorés");
                ValidateArgumentsRead("NonUniformBSplineCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[NonUniformBSplineCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
           return $"NON-UNIFORM B-SPLINE (basic representation)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
