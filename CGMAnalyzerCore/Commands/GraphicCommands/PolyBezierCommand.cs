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
    /// <summary>
    /// POLYBEZIER (case 26) - Courbe de Bézier polynomiale
    /// </summary>
    public class PolyBezierCommand :BaseCgmCommand
    {
        private readonly List<Point2D.Double> _controlPoints = new List<Point2D.Double>();

        public IReadOnlyList<Point2D.Double> ControlPoints => _controlPoints;

        public PolyBezierCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[PolyBezierCommand] ArgsLength={Args?.Length ?? 0}");
           
            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Controle du nombre points
                int pointSize = argReader.SizeOfPoint();
                int pointCount = command.Args.Length / pointSize;
                Debug.WriteLine($"[PolyBezierCommand] pointSize={pointSize}, pointCount={pointCount}");
                
                // Lire les points de contrôle
                for (int i = 0; i < pointCount; i++)
                {
                    Point2D.Double point = argReader.MakePoint();
                    _controlPoints.Add(point);
                }

                Debug.WriteLine($"[PolyBezierCommand] Total control points: {_controlPoints.Count}");
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
                Debug.WriteLine($"[PolyBezierCommand] Pas assez de points de contrôle pour une courbe de Bézier (minimum 4, actuellement {_controlPoints.Count})");
                return;
            }

            Debug.WriteLine($"[PolyBezierCommand] Dessin de courbes de Bézier avec {_controlPoints.Count} points de contrôle");

            try
            {
                // Dessiner des courbes de Bézier par groupes de 4 points
                // Une courbe de Bézier cubique nécessite 4 points : P0 (start), P1 (control1), P2 (control2), P3 (end)
                // Les courbes se chaînent : le dernier point d'une courbe est le premier point de la suivante
                for (int i = 0; i <= _controlPoints.Count - 4; i += 3)
                {
                    if (i + 3 < _controlPoints.Count)
                    {
                        var points = _controlPoints.Skip(i).Take(4).Select(p => p.ToPointF()).ToArray();
                        g.DrawBezier(pen, points[0], points[1], points[2], points[3]);
                        //Debug.WriteLine($"[PolyBezierCommand] Courbe {i / 3 + 1}: points [{i}, {i + 1}, {i + 2}, {i + 3}]");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[PolyBezierCommand Draw ERROR] {ex.Message}");
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

    // ---------------------------------------------------------------------

    // Commandes complexes avec implémentation basique
    
    /// <summary>
    /// NON_UNIFORM_B_SPLINE (case 24) - Courbe B-spline non uniforme (non supportée)
    /// </summary>
    public class NonUniformBSplineCommand : BaseCgmCommand
    {
        public NonUniformBSplineCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l) 
        { 
            Args = command.Args;
            Debug.WriteLine($"[NonUniformBSplineCommand] ArgsLength={Args?.Length ?? 0}");
           
            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Consommation explicite
                int remaining = this.RemainingArgs();
                Debug.WriteLine($"[NonUniformBSplineCommand] {remaining} bytes à ignorer");
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
           return $"NON_UNIFORM_B_SPLINE (unsupported)";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
