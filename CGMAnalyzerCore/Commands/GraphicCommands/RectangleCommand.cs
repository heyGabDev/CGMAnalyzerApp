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
    /// RECTANGLE (case 11) - Dessine un rectangle défini par deux coins opposés
    /// </summary>
    public class RectangleCommand : BaseCgmCommand
    {
        public Point2D Corner1 { get; private set; }
        public Point2D Corner2 { get; private set; }

        public RectangleCommand(int ec, int eid, CgmCommand command, ExtractedArgumentReader argReader)
            : base(ec, eid, command.Length)
        {
            // Un rectangle est défini par 2 points (coins opposés)
            Corner1 = argReader.MakePoint(ec, eid);
            Corner2 = argReader.MakePoint(ec, eid);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // Calculer les coordonnées du rectangle
            float x = (float)Math.Min(Corner1.X, Corner2.X);
            float y = (float)Math.Min(Corner1.Y, Corner2.Y);
            float width = (float)Math.Abs(Corner2.X - Corner1.X);
            float height = (float)Math.Abs(Corner2.Y - Corner1.Y);

            if (width > 0 && height > 0)
            {
                g.DrawRectangle(pen, x, y, width, height);
            }
        }

        public override void ReadArguments(BinaryReader reader)
        {
            throw new NotImplementedException("Utiliser le constructeur avec ExtractedArgumentReader");
        }

        public override string ToString()
        {
            return $"RECTANGLE from {Corner1} to {Corner2}";
        }
    }
}
