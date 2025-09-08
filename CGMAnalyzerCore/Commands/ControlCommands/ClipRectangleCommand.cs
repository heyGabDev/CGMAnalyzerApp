using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ControlCommands
{
    public class ClipRectangleCommand : CgmCommand
    {
        public Point2D.Double Point1 { get; private set; }
        public Point2D.Double Point2 { get; private set; }
        public Rectangle2D.Double ClipShape { get; private set; }

        public ClipRectangleCommand(int ec, int eid, int l, CgmCommand baseCommand, ExtractedArgumentReader argReader)
            : base(baseCommand, ec, eid, l)
        {
            Point1 = argReader.MakePoint();
            Point2 = argReader.MakePoint();

            // Créer le rectangle de clipping
            ClipShape = new Rectangle2D.Double(
                Point1.X,
                Point1.Y,
                Point2.X - Point1.X,
                Point2.Y - Point1.Y
            );

            System.Diagnostics.Debug.Assert(CurrentArg == Args.Length,
                "Not all arguments were read in ClipRectangle");
        }

        public override string ToString()
        {
            return $"ClipRectangle p1={Point1}, p2={Point2}";
        }

        // Méthode paint équivalente du Java (à adapter selon votre architecture de display)
        public void ApplyToDisplay(object display)
        {
            // Dans le Java original : if (d.getClipFlag()) { g2d.setClip(this.shape); }
            // À implémenter selon votre architecture de rendu
        }
    }

}
