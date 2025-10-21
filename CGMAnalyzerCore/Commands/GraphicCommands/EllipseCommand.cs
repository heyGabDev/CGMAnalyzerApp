using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// ELLIPSE (case 17) - Dessine une ellipse définie par centre + 2 points conjugués
    /// </summary>
    public class EllipseCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; } = new Point2D(0, 0);
        public Point2D FirstConjugateDiameter { get; private set; } = new Point2D(0, 0);
        public Point2D SecondConjugateDiameter { get; private set; } = new Point2D(0, 0);

        public EllipseCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EllipseCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);

                // Une ellipse est définie par le centre et 2 diamètres conjugués
                Center = argReader.MakePoint();
                Debug.WriteLine($"[EllipseCommand] Read Center=({Center.X}, {Center.Y})");

                FirstConjugateDiameter = argReader.MakePoint();
                Debug.WriteLine($"[EllipseCommand] Read FirstConjugateDiameter=({FirstConjugateDiameter.X}, {FirstConjugateDiameter.Y})");
            
                SecondConjugateDiameter = argReader.MakePoint();
                Debug.WriteLine($"[EllipseCommand] Read SecondConjugateDiameter=({SecondConjugateDiameter.X}, {SecondConjugateDiameter.Y})");
                
                ValidateArgumentsRead("EllipseCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipseCommand ERROR] {ex.Message}");
                Center = new Point2D(0, 0);
                FirstConjugateDiameter = new Point2D(0, 0);
                SecondConjugateDiameter = new Point2D(0, 0);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Distance du centre au point = rayon
            float rx = (float)Math.Sqrt(
                            Math.Pow(FirstConjugateDiameter.X - Center.X, 2) +
                            Math.Pow(FirstConjugateDiameter.Y - Center.Y, 2)
                        );
            float ry = (float)Math.Sqrt(
                Math.Pow(SecondConjugateDiameter.X - Center.X, 2) +
                Math.Pow(SecondConjugateDiameter.Y - Center.Y, 2)
            );

            if (rx <= 0 || ry <= 0)
            {
                Debug.WriteLine($"[EllipseCommand] Rayons invalides (rx={rx:F2}, ry={ry:F2}), dessin ignoré");
                return;
            }

            Debug.WriteLine($"[EllipseCommand] Dessin ellipse centre ({Center.X}, {Center.Y}), rx={rx:F2}, ry={ry:F2}");

            try
            {
                float x = (float)(Center.X - rx);
                float y = (float)(Center.Y - ry);
                float width = rx * 2;
                float height = ry * 2;

                g.DrawEllipse(pen, x, y, width, height);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipseCommand ERROR] Erreur lors du dessin : {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            float rx = (float)Math.Sqrt(Math.Pow(FirstConjugateDiameter.X - Center.X, 2) + Math.Pow(FirstConjugateDiameter.Y - Center.Y, 2));
            float ry = (float)Math.Sqrt(Math.Pow(SecondConjugateDiameter.X - Center.X, 2) + Math.Pow(SecondConjugateDiameter.Y - Center.Y, 2));
            return $"ELLIPSE center=({Center.X}, {Center.Y}) rx={rx:F2} ry={ry:F2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
