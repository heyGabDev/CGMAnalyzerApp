using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    /// <summary>
    /// ELLIPSE (case 17) - Dessine une ellipse définie par centre + 2 points conjugués
    /// </summary>
    public class EllipseCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; }
        public Point2D FirstConjugateDiameter { get; private set; }
        public Point2D SecondConjugateDiameter { get; private set; }

        public EllipseCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Une ellipse est définie par le centre et 2 diamètres conjugués
            Center = argReader.MakePoint(ec, eid);
            FirstConjugateDiameter = argReader.MakePoint(ec, eid);
            SecondConjugateDiameter = argReader.MakePoint(ec, eid);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Calculer les rayons à partir des diamètres conjugués
            float rx = (float)Math.Abs(FirstConjugateDiameter.X - Center.X);
            float ry = (float)Math.Abs(SecondConjugateDiameter.Y - Center.Y);

            if (rx > 0 && ry > 0)
            {
                float x = (float)(Center.X - rx);
                float y = (float)(Center.Y - ry);
                float width = rx * 2;
                float height = ry * 2;

                g.DrawEllipse(pen, x, y, width, height);
            }
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"ELLIPSE center={Center} rx={(FirstConjugateDiameter.X - Center.X):F2} ry={(SecondConjugateDiameter.Y - Center.Y):F2}";
        }
    }
}
