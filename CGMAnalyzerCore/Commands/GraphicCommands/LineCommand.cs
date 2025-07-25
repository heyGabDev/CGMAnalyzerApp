using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class LineCommand : BaseCgmCommand
    {
        public Point Start { get; private set; }
        public Point End { get; private set; }

        public LineCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length)
        {
            ReadArguments(reader);
        }

        public override void Draw(Graphics g, Pen pen)
        {
            g.DrawLine(pen, Start, End);
        }

        public override void ReadArguments(BinaryReader reader)
        {
            // Hypothèse : coordonnées en Int16
            int x1 = reader.ReadInt16();
            int y1 = reader.ReadInt16();
            int x2 = reader.ReadInt16();
            int y2 = reader.ReadInt16();

            Start = new Point(x1, y1);
            End = new Point(x2, y2);
        }

        public override string ToString()
        {
            return $"LINE from {Start} to {End}";
        }
    }
}
