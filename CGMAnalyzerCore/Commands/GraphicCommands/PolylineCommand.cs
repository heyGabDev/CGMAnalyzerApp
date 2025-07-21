using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CGMAnalyzerCore.Commands.GraphicCommands
{
    public class PolylineCommand : BaseCgmCommand
    {
        public List<Point> Points { get; private set; } = new();

        public PolylineCommand(int ec, int eid, int length, BinaryReader reader)
            : base(ec, eid, length)
        {
            ReadArguments(reader);
        }

        public override void ReadArguments(BinaryReader reader)
        {
            int pointCount = Length / 4; // Chaque point = 2 x Int16 (2*2 octets)
            for (int i = 0; i < pointCount; i++)
            {
                int x = reader.ReadInt16();
                int y = reader.ReadInt16();
                Points.Add(new Point(x, y));
            }
        }

        public override void Draw(Graphics g, Pen pen)
        {
            if (Points.Count >= 2)
                g.DrawLines(pen, Points.ToArray());
        }

        public override string ToString()
        {
            return $"POLYLINE ({Points.Count} points)";
        }
    }
}
