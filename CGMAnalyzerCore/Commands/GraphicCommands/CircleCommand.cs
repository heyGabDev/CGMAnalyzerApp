using CGMAnalyzerCore.Geometry;
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
    /// CIRCLE (case 12) - Dessine un cercle défini par centre + point sur la circonférence
    /// </summary>
    public class CircleCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; } = new Point2D(0, 0);    
        public Point2D EdgePoint { get; private set; } = new Point2D(0, 0);
        public double Radius { get; private set; } = 0;

        public CircleCommand(int ec, int eid,int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[CircleCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Un cercle est défini par le centre et un point sur le bord
                Center = argReader.MakePoint();
                Debug.WriteLine($"[CircleCommand] Read Center=({Center.X}, {Center.Y})");

                EdgePoint = argReader.MakePoint();
                Debug.WriteLine($"[CircleCommand] Read EdgePoint=({EdgePoint.X}, {EdgePoint.Y})");

                // Calculer le rayon à partir de la distance centre-bord
                Radius = Math.Sqrt(Math.Pow(EdgePoint.X - Center.X, 2) + Math.Pow(EdgePoint.Y - Center.Y, 2));
                Debug.WriteLine($"[CircleCommand] Calculated Radius={Radius:F2}");
                ValidateArgumentsRead("CircleCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CircleCommand ERROR] {ex.Message}");
                Center = new Point2D(0, 0);
                EdgePoint = new Point2D(0, 0);
                Radius = 0;
                HasReadErrors = true;   
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Radius <=0)
            {
                Debug.WriteLine("[CircleCommand] Rayon invalide, dessin ignoré");
                return;
            }

            Debug.WriteLine($"[CircleCommand] Dessin du cercle avec centre ({Center.X}, {Center.Y}) et rayon {Radius:F2}");
            try
            {
                float diameter = (float)(Radius * 2);
                float x = (float)(Center.X - Radius);
                float y = (float)(Center.Y - Radius);

                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CircleCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }        
        }

        public override string ToString()
        {
            return $"CIRCLE center=({Center.X}, {Center.Y}) radius={Radius:F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
