using CGMAnalyzerCore.Geometry;
using CGMAnalyzerCore.Parser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGMAnalyzerCore.Commands.ControlCommands
{
    public class ClipRectangleCommand : BaseCgmCommand
    {
        public Point2D.Double Point1 { get; private set; } = new Point2D.Double(0, 0);
        public Point2D.Double Point2 { get; private set; } = new Point2D.Double(0, 0);
        public Rectangle2D.Double ClipShape { get; private set; } = new Rectangle2D.Double(0, 0, 0, 0);

        public ClipRectangleCommand(int ec, int eid, int l, CgmCommand command)
            : base(ec, eid, l)
        {
            Args = command.Args;
            Debug.WriteLine($"[ClipRectangleCommand] ArgsLength={Args?.Length ?? 0}");


            try
            {
                var argReader = new ExtractedArgumentReader(this);
                Point1 = argReader.MakePoint();
                Point2 = argReader.MakePoint();

            // Créer le rectangle de clipping
            ClipShape = new Rectangle2D.Double(
                Point1.X,
                Point1.Y,
                Point2.X - Point1.X,
                Point2.Y - Point1.Y
            );

                Debug.WriteLine($"[ClipRectangleCommand] Point1=({Point1.X}, {Point1.Y}), Point2=({Point2.X}, {Point2.Y})");
                ValidateArgumentsRead("ClipRectangle");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ClipRectangleCommand ERROR] {ex.Message}");
                Point1 = new Point2D.Double(0, 0);
                Point2 = new Point2D.Double(0, 0);
                ClipShape = new Rectangle2D.Double(0, 0, 0, 0);
                HasReadErrors = true;
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            // No drawing
        }

        public override string ToString()
        {
            return $"ClipRectangle p1={Point1}, p2={Point2}";
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Ne plus utiliser cette méthode
            throw new NotImplementedException("Use constructor with ExtractedArgumentReader instead");
        }
    }

}
