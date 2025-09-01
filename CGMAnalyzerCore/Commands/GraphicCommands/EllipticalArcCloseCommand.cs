using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
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
        public Point2D.Double Center { get; private set; }
        public Point2D.Double FirstConjugateDiameter { get; private set; }
        public Point2D.Double SecondConjugateDiameter { get; private set; }
        public double StartAngle { get; private set; }
        public double ExtentAngle { get; private set; }

        public EllipticalArcCloseCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            Center = argReader.MakePoint(ec, eid);
            FirstConjugateDiameter = argReader.MakePoint(ec, eid);
            SecondConjugateDiameter = argReader.MakePoint(ec, eid);
            StartAngle = argReader.MakeVdc();
            ExtentAngle = argReader.MakeVdc();
        }

        public override void Draw(Graphics g, Pen pen)
        {
            float rx = (float)Math.Abs(FirstConjugateDiameter.X - Center.X);
            float ry = (float)Math.Abs(SecondConjugateDiameter.Y - Center.Y);

            if (rx == 0 || ry == 0) return;

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

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"ELLIPTICAL_ARC_CLOSE center={Center} angles={StartAngle:F1}°-{ExtentAngle:F1}°";
        }
    }
}
