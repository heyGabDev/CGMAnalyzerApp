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
    /// CIRCLE (case 12) - Dessine un cercle défini par centre + point sur la circonférence
    /// </summary>
    public class CircleCommand : BaseCgmCommand
    {
        public Point2D Center { get; private set; }
        public Point2D EdgePoint { get; private set; }
        public double Radius { get; private set; }

        public CircleCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Un cercle est défini par le centre et un point sur le bord
            Center = argReader.MakePoint(ec, eid);
            EdgePoint = argReader.MakePoint(ec, eid);

            // Calculer le rayon à partir de la distance centre-bord
            Radius = Math.Sqrt(Math.Pow(EdgePoint.X - Center.X, 2) + Math.Pow(EdgePoint.Y - Center.Y, 2));
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Radius > 0)
            {
                float diameter = (float)(Radius * 2);
                float x = (float)(Center.X - Radius);
                float y = (float)(Center.Y - Radius);

                g.DrawEllipse(pen, x, y, diameter, diameter);
            }
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"CIRCLE center={Center} radius={Radius:F2}";
        }
    }
}
