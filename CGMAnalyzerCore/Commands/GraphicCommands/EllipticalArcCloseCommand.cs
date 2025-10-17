using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// ELLIPTICAL_ARC_CLOSE (case 19) - Arc elliptique fermé (secteur)
    /// </summary>
    public class EllipticalArcCloseCommand : BaseCgmCommand
    {
        public Point2D.Double Center { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double FirstConjugateDiameter { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double SecondConjugateDiameter { get; private set; } = new Point2D.Double(0, 0);
        public double StartAngle { get; private set; } = 0;
        public double ExtentAngle { get; private set; } = 0;

        public EllipticalArcCloseCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[EllipticalArcCloseCommand] ArgsLength={Args?.Length ?? 0}");

            try
            {
                var argReader = new ExtractedArgumentReader(this);


                Center = argReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCloseCommand] Read Center=({Center.X}, {Center.Y})");

                FirstConjugateDiameter = argReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCloseCommand] Read FirstConjugateDiameter=({FirstConjugateDiameter.X}, {FirstConjugateDiameter.Y})");

                SecondConjugateDiameter = argReader.MakePoint();
                Debug.WriteLine($"[EllipticalArcCloseCommand] Read SecondConjugateDiameter=({SecondConjugateDiameter.X}, {SecondConjugateDiameter.Y})");

                StartAngle = argReader.MakeVdc();
                Debug.WriteLine($"[EllipticalArcCloseCommand] Read StartAngle={StartAngle:F2}");

                ExtentAngle = argReader.MakeVdc();
                Debug.WriteLine($"[EllipticalArcCloseCommand] Read ExtentAngle={ExtentAngle:F2}");
                ValidateArgumentsRead("EllipticalArcCloseCommand");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipticalArcCloseCommand ERROR] {ex.Message}");
                Center = new Point2D.Double(0, 0);
                FirstConjugateDiameter = new Point2D.Double(0, 0);
                SecondConjugateDiameter = new Point2D.Double(0, 0);
                StartAngle = 0;
                ExtentAngle = 0;
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            float rx = (float)Math.Abs(FirstConjugateDiameter.X - Center.X);
            float ry = (float)Math.Abs(SecondConjugateDiameter.Y - Center.Y);

            if(rx <= 0 || ry <= 0)
            {
                Debug.WriteLine("[EllipticalArcCloseCommand] Rayons invalides, dessin ignoré");
                return;
            }

            try
            {
                float left = (float)(Center.X - rx);
                float top = (float)(Center.Y - ry);
                float width = 2 * rx;
                float height = 2 * ry;

                using var path = new GraphicsPath();

                // Ajouter l'arc
                path.AddArc(left, top, width, height, (float)StartAngle, (float)ExtentAngle);

                // Fermer vers le centre pour créer un secteur
                var centerF = new PointF((float)Center.X, (float)Center.Y);
                path.AddLine(path.GetLastPoint(), centerF);
                path.CloseFigure();

                // Dessiner le secteur
                g.DrawPath(pen, path);

                // Optionnel : remplir avec une couleur transparente
                using var brush = new SolidBrush(Color.FromArgb(30, pen.Color));
                g.FillPath(brush, path);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[EllipticalArcCloseCommand ERROR] {ex.Message}");
                HasReadErrors = true;
            }
        }

        public override string ToString()
        {
            return $"ELLIPTICAL_ARC_CLOSE center={Center} angles={StartAngle:F1}°-{ExtentAngle:F1}°";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }
}
